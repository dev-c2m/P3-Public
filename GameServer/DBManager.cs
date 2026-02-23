using UnityServer.Models;
using UnityServer.Player;
using UnityServer.Repository.Interface;

namespace UnityServer
{
    public class DBManager
    {
        private readonly IUserRepository userRepository;
        private readonly IInventoryRepository inventoryRepository;
        private readonly IAttendanceRepository attendanceRepository;
        private readonly RedisManager redisManager;

        public DBManager(
            IUserRepository userRepository,
            IInventoryRepository inventoryRepository,
            IAttendanceRepository attendanceRepository,
            RedisManager redisManager)
        {
            this.userRepository = userRepository;
            this.inventoryRepository = inventoryRepository;
            this.attendanceRepository = attendanceRepository;
            this.redisManager = redisManager;
        }

        public void LoadPlayerData(Session session, int accountId, string nickname)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    CharacterData? character = userRepository.GetCharacterByAccountId(accountId);
                    List<InventoryItem> items = inventoryRepository.GetItems(accountId);

                    List<int> availableAttendances = DataManager.GetAllAvailableAttendance();
                    List<PlayerAttendance> attendances = new List<PlayerAttendance>();
                    foreach (int attendanceId in availableAttendances)
                    {
                        AttendanceResult attendanceResult = attendanceRepository.GetAttendanceDayCountAndCanCheckIn(accountId, attendanceId);
                        attendances.Add(new PlayerAttendance(attendanceId, attendanceResult.DayCount, attendanceResult.CanCheckIn));
                    }

                    if (character != null)
                    {
                        PlayerStats tempStats = new PlayerStats(character.Value.Level, character.Value.Exp, character.Value.Hp, character.Value.Mp);
                        redisManager.SaveCharacter(accountId, tempStats, character.Value.Map);
                    }

                    Dictionary<int, InventoryItem> inventoryDict = new Dictionary<int, InventoryItem>();
                    foreach (InventoryItem item in items)
                    {
                        inventoryDict[item.SlotIndex] = item;
                    }
                    redisManager.SaveInventory(accountId, inventoryDict);

                    foreach (PlayerAttendance att in attendances)
                    {
                        redisManager.SaveAttendance(accountId, att.AttendanceId, att.DayCount, att.CanCheckIn);
                    }

                    LoginResult loginResult = new LoginResult(accountId, nickname);
                    DbResult successResult = DbResult.LoginSuccess(session, loginResult, character, items, attendances);
                    Program.GameLoop.EnqueueDbResult(successResult);
                }
                catch (Exception ex)
                {
                    Logger.Error($"[DBManager] 플레이어 데이터 로드 실패: {ex.Message}");
                    DbResult result = DbResult.LoginFail(session, "서버 오류가 발생했습니다.");
                    Program.GameLoop.EnqueueDbResult(result);
                }
            });
        }


        #region Redis -> DB 저장

        public void SavePlayerToDb(int accountId)
        {
            try
            {
                CharacterData? character = redisManager.LoadCharacter(accountId);
                if (character != null)
                {
                    PlayerStats tempStats = new PlayerStats(character.Value.Level, character.Value.Exp, character.Value.Hp, character.Value.Mp);
                    userRepository.SaveCharacterStats(accountId, tempStats, character.Value.Map);
                }

                List<InventoryItem> items = redisManager.LoadInventory(accountId);
                SaveInventoryToDb(accountId, items);

                List<PlayerAttendance> attendances = redisManager.LoadAttendance(accountId);
                foreach (PlayerAttendance att in attendances)
                {
                    if (!att.CanCheckIn)
                        attendanceRepository.TryCheckAttendance(accountId, att.AttendanceId, att.DayCount);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[DBManager] 유저 {accountId} 동기 DB 저장 실패: {ex.Message}");
            }
        }

        public void SaveAllPlayersToDb()
        {
            IEnumerable<Session> sessions = Program.SessionManager.GetAllSessions();
            Logger.Info($"[DBManager] 전체 유저 DB 저장 시작 ({sessions.Count()}명)");

            CountdownEvent countdown = new CountdownEvent(sessions.Count());

            foreach (Session session in sessions)
            {
                if (session.Info.AccountId == 0)
                {
                    countdown.Signal();
                    continue;
                }

                int accountId = session.Info.AccountId;

                redisManager.SaveCharacter(accountId, session.Stats, session.Position.MapId);
                redisManager.SaveInventory(accountId, session.Inventory);
                foreach (PlayerAttendance att in session.Attendances)
                {
                    redisManager.SaveAttendance(accountId, att.AttendanceId, att.DayCount, att.CanCheckIn);
                }

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        SavePlayerToDb(accountId);
                        redisManager.DeletePlayerData(accountId);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"[DBManager] 전체 저장 중 유저 {accountId} 실패: {ex.Message}");
                    }
                    finally
                    {
                        countdown.Signal();
                    }
                });
            }

            countdown.Wait(TimeSpan.FromSeconds(30));
            Logger.Success($"[DBManager] 전체 유저 DB 저장 완료");
        }

        public void SavePlayerOnLogout(Session session)
        {
            int accountId = session.Info.AccountId;
            if (accountId == 0)
                return;

            redisManager.SaveCharacterFireAndForget(accountId, session.Stats, session.Position.MapId);
            redisManager.SaveInventoryFireAndForget(accountId, session.Inventory);
            foreach (PlayerAttendance att in session.Attendances)
            {
                redisManager.SaveAttendanceFireAndForget(accountId, att.AttendanceId, att.DayCount, att.CanCheckIn);
            }

            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    SavePlayerToDb(accountId);
                    redisManager.DeletePlayerData(accountId);
                    Logger.Debug($"[DBManager] 유저 {accountId} 로그아웃 DB 저장 완료");
                }
                catch (Exception ex)
                {
                    Logger.Error($"[DBManager] 유저 {accountId} 로그아웃 DB 저장 실패: {ex.Message}");
                }
            });
        }

        #endregion


        public void FlushDirtyPlayers()
        {
            foreach (Session session in Program.SessionManager.GetAllSessions())
            {
                if (session.Info.AccountId == 0)
                    continue;

                if (!session.IsDirty)
                    continue;

                session.ClearDirty();
                int accountId = session.Info.AccountId;

                ThreadPool.QueueUserWorkItem(_ => SavePlayerToDb(accountId));
            }
        }

        private void SaveInventoryToDb(int accountId, List<InventoryItem> items)
        {
            foreach (InventoryItem item in items)
            {
                inventoryRepository.InsertItem(accountId, item.SlotIndex, item.ItemId, item.Quantity);
            }
        }
    }
}
