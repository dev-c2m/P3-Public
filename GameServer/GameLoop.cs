using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Numerics;
using UnityServer.Data;
using UnityServer.Models;
using UnityServer.Player;
using UnityServer.Share;
using UnityServer.Share.Packets;

namespace UnityServer
{
    class GameLoop
    {
        private BlockingCollection<ReceivedSessionPacket> recvQueue = new BlockingCollection<ReceivedSessionPacket>(new ConcurrentQueue<ReceivedSessionPacket>());
        private Thread loopThread;
        private DateTime lastMonsterMove = DateTime.Now;
        private DateTime lastMonsterRespawn = DateTime.Now;
        private DateTime lastDbSave = DateTime.Now;

        public void Start()
        {
            loopThread = new Thread(LoopProcess);
            loopThread.IsBackground = true;
            loopThread.Start();
        }

        private void LoopProcess()
        {
            int elapsed = 0;
            int sleepTime = 0;

            while (true)
            {
                ProcessReceivedPacket();
                ProcessPlayerMovement();
                Program.ProjectileManager.ProcessProjectiles();

                DateTime now = DateTime.Now;

                if ((now - lastMonsterMove).TotalSeconds >= Constants.MonsterMoveInterval)
                {
                    Program.MonsterManager.ProcessMonsterMovement();
                    lastMonsterMove = now;
                }

                if ((now - lastMonsterRespawn).TotalSeconds >= Constants.MonsterRespawnInterval)
                {
                    Program.MonsterManager.RespawnMonsters();
                    lastMonsterRespawn = now;
                }

                if ((now - lastDbSave).TotalSeconds >= Constants.DbSaveInterval)
                {
                    Program.DBManager.FlushDirtyPlayers();
                    lastDbSave = now;
                }

                foreach (Session s in Program.SessionManager.GetAllSessions())
                {
                    s.Flush();
                }

                elapsed = (int)(DateTime.Now - now).TotalMilliseconds;
                sleepTime = Constants.SendTick - elapsed;
                //Logger.Debug($"GameLoop Tick: Elapsed={elapsed}ms, Sleep={sleepTime}ms");

                if (sleepTime > 0)
                {
                    Thread.Sleep(sleepTime);
                }
            }
        }

        private void ProcessPlayerMovement()
        {
            float deltaTime = Constants.SendTick / 1000f;

            foreach (Session session in Program.SessionManager.GetAllSessions())
            {
                if (session.Info.AccountId == 0)
                    continue;

                if (!session.Position.IsMoving)
                    continue;

                MapData? mapData = DataManager.GetMapData(session.Position.MapId);
                float mapSize = mapData != null ? mapData.MapSize : 0f;
                bool isMapOver = session.Position.TickMovement(deltaTime, mapSize);

                if (isMapOver)
                {
                    session.Position.StopMovement();
                    session.SendAsync(PacketWriter.PlayerMoveNotify(session.PlayerId, session.Position.Pos, session.Position.RotationY, 0f, 0f, false, 0f));
                }

                session.UpdateVisiblePlayers(Constants.MaxVisibleDistance);
            }
        }

        private void ProcessReceivedPacket()
        {
            while (recvQueue.TryTake(out ReceivedSessionPacket sp))
            {
                switch (sp.Type)
                {
                    case Constants.ReceivedPacketType.Create:
                        Session session = Program.SessionManager.Create(sp.TcpClient);
                        _ = session.StartAsync(Program.CTS.Token);
                        continue;
                    case Constants.ReceivedPacketType.Leave:
                        sp.Session.Leave();
                        break;
                    case Constants.ReceivedPacketType.Packet:
                        PacketHandler.Handle(sp.Session, sp.Packet);
                        break;
                    case Constants.ReceivedPacketType.DbResult:
                        HandleDbResult(sp.DbResult);
                        break;
                }
            }
        }

        private void HandleDbResult(DbResult dbResult)
        {
            switch (dbResult.Type)
            {
                case DbResultType.LoginSuccess:
                    HandleLoginSuccess(dbResult);
                    break;
                case DbResultType.LoginFailed:
                    dbResult.Session.SendAsync(PacketWriter.LoginResponse(false, dbResult.Message));
                    break;
            }
        }

        private void HandleLoginSuccess(DbResult dbResult)
        {
            Session session = dbResult.Session;
            LoginResult loginData = dbResult.LoginData.Value;

            if (Program.SessionManager.IsLoggedIn(loginData.AccountId))
            {
                session.SendAsync(PacketWriter.LoginResponse(false, "이미 로그인 중인 계정입니다."));
                return;
            }

            session.Info.SetAccountId(loginData.AccountId);
            session.Info.SetNickname(loginData.Nickname);
            Program.SessionManager.RegisterLogin(loginData.AccountId, session);

            if (dbResult.Character != null)
            {
                session.Stats.Load(dbResult.Character.Value.Level, dbResult.Character.Value.Exp, dbResult.Character.Value.Hp, dbResult.Character.Value.Mp);
                session.Position.SetMapId(dbResult.Character.Value.Map);
            }

            foreach (PlayerAttendance attendance in dbResult.Attendances)
            {
                session.Attendances.Add(attendance);
            }

            foreach (InventoryItem item in dbResult.Items)
            {
                session.Inventory[item.SlotIndex] = item;
            }

            int mapId = dbResult.Character != null ? dbResult.Character.Value.Map : Constants.DefaultMapId;
            MapData? defaultMap = DataManager.GetMapData(mapId);
            if (defaultMap != null)
            {
                Vector3 spawnPos = new Vector3(defaultMap.SpawnX, defaultMap.SpawnY, defaultMap.SpawnZ);
                session.Position.Teleport(spawnPos, 0f);
            }

            session.SendAsync(PacketWriter.LoginResponse(true, "로그인 성공", loginData.Nickname, session.Position.MapId));
            session.SendInitialData();
            Logger.Info($"유저 로그인 {loginData.AccountId} / {loginData.Nickname}");
        }



        public void Enqueue(Session session, Packet packet)
        {
            ReceivedSessionPacket sp = new ReceivedSessionPacket(session, packet);
            recvQueue.Add(sp);
        }

        public void EnqueueLeave(Session session)
        {
            ReceivedSessionPacket sp = new ReceivedSessionPacket(session);
            recvQueue.Add(sp);
        }

        public void EnqueueCreate(TcpClient client)
        {
            ReceivedSessionPacket sp = new ReceivedSessionPacket(client);
            recvQueue.Add(sp);
        }

        public void EnqueueDbResult(DbResult dbResult)
        {
            ReceivedSessionPacket sp = new ReceivedSessionPacket(dbResult);
            recvQueue.Add(sp);
        }
    }
}
