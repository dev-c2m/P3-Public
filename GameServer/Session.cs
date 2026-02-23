using System.Net.Sockets;
using UnityServer;
using UnityServer.Models;
using UnityServer.Player;
using UnityServer.Share;
using UnityServer.Share.Packets;

public class Session
{
    private TcpClient client;
    private NetworkStream stream;
    private SendContext sendContext;
    private byte[] recvBuffer;
    private bool isClosed;
    private bool isLeft;
    private HashSet<int> visiblePlayers = new HashSet<int>();

    public int PlayerId { get; private set; }
    public PlayerInfo Info { get; }
    public PlayerPosition Position { get; }
    public PlayerStats Stats { get; }
    public List<PlayerAttendance> Attendances { get; }
    public Dictionary<int, InventoryItem> Inventory { get; } = new();
    private Dictionary<int, DateTime> skillCooldowns = new();
    private bool isDirty = false;

    public bool IsDirty => isDirty;

    public void MarkDirty()
    {
        isDirty = true;
    }

    public void ClearDirty()
    {
        isDirty = false;
    }

    public Session(TcpClient client, int playerId)
    {
        this.client = client;
        this.stream = client.GetStream();
        this.sendContext = new SendContext(stream);
        this.recvBuffer = new byte[1024];
        this.isClosed = false;
        this.PlayerId = playerId;

        this.Info = new PlayerInfo();
        this.Position = new PlayerPosition();
        this.Stats = new PlayerStats();
        this.Attendances = new List<PlayerAttendance>();
    }

    public async Task StartAsync(CancellationToken token)
    {
        try
        {
            Logger.Info($"[Server] Client {PlayerId} connected");

            ReceiveBuffer receiveBuffer = new ReceiveBuffer();

            while (!token.IsCancellationRequested && !isClosed)
            {
                int bytesRead = await stream.ReadAsync(recvBuffer, 0, recvBuffer.Length, token);

                if (bytesRead == 0)
                    break;

                receiveBuffer.Write(recvBuffer, bytesRead);

                byte[] packet;
                while (receiveBuffer.TryReadPacket(out packet))
                {                    
                    HandlePacket(packet);
                }
            }
        }
        catch (Exception ex)
        {
            if (!isClosed)
            {
                Logger.Error($"[Session Error] {ex.Message}");
            }
        }
        finally
        {
            isClosed = true;
            sendContext.Close();
            stream.Close();
            client.Close();

            Program.GameLoop.EnqueueLeave(this);
        }
    }

    public void Broadcast(byte[] message)
    {
        IEnumerable<Session> sessionsToSend = Program.SessionManager.GetAllSessionsWithOutSelf(this);

        foreach (var s in sessionsToSend)
        {
            s.SendAsync(message);
        }
    }

    public void BroadcastInRange(byte[] message, float range)
    {
        IEnumerable<Session> otherSessions = Program.SessionManager.GetAllSessionsWithOutSelf(this);

        foreach (Session other in otherSessions)
        {
            if (Position.MapId == other.Position.MapId && Position.DistanceTo(other.Position) <= range)
            {
                other.SendAsync(message);
            }
        }
    }

    public void SendPlayerSpawn(Session target)
    {
        SendAsync(PacketWriter.PlayerSpawnNotify(target.PlayerId, target.Info.Nickname, target.Position));
        if (target.Position.IsMoving)
        {
            SendAsync(PacketWriter.PlayerMoveNotify(
                target.PlayerId, target.Position.Pos, target.Position.RotationY,
                target.Position.DirectionX, target.Position.DirectionZ, true, Constants.MaxPlayerMoveSpeed));
        }
    }

    public void SendAsync(byte[] message)
    {
        if (isClosed) 
            return;

        sendContext.Enqueue(message);
    }

    public void Flush()
    {
        if (isClosed) 
            return;

        sendContext.Flush();
    }

    public void SendInitialData()
    {
        SendAsync(PacketWriter.AssignPlayerIdNotify(PlayerId));

        foreach (var Attendance in Attendances)
        {
            SendAsync(PacketWriter.AttendanceNotify(Attendance.AttendanceId, Attendance.DayCount, Attendance.CanCheckIn));
        }

        SendAsync(PacketWriter.PlayerStatsNotify(Stats));

        foreach (var item in Inventory.Values)
        {
            SendAsync(PacketWriter.ItemAddNotify(item.ItemId, item.Quantity, item.SlotIndex));
        }

        foreach (var session in Program.SessionManager.GetAllSessionsWithOutSelf(this))
        {
            if (!session.Info.IsLoggedIn)
                continue;

            if (Position.MapId == session.Position.MapId && Position.DistanceTo(session.Position) <= Constants.MaxVisibleDistance)
            {
                bool result = AddVisiblePlayer(session.PlayerId);
                if (result)
                {
                    SendPlayerSpawn(session);
                }

                result = session.AddVisiblePlayer(PlayerId);
                if (result)
                {
                    session.SendPlayerSpawn(this);
                }
            }
        }

        var spawnNotifies = Program.MonsterManager.GetSpawnNotifiesForMap(Position.MapId);
        foreach (var notify in spawnNotifies)
        {
            SendAsync(notify);
        }

        var groundItemNotifies = Program.GroundItemManager.GetSpawnNotirysInMap(Position.MapId);
        foreach (var notify in groundItemNotifies)
        {
            SendAsync(notify);
        }

        SendAsync(PacketWriter.EnterCompleteNotify());
    }

    private void HandlePacket(byte[] packetBytes)
    {
        Packet? packet = PacketReader.Read(packetBytes);

        if (packet != null)
        {
            Program.GameLoop.Enqueue(this, packet);
        }
    }

    public void Leave()
    {
        if (isLeft)
            return;

        isLeft = true;
        isClosed = true;
        Program.DBManager.SavePlayerOnLogout(this);
        Program.SessionManager.Remove(this);
        ClearVisiblePlayers();
    }

    public void UpdateVisiblePlayers(float range, byte[]? broadcastBytes = null)
    {
        IEnumerable<Session> otherSessions = Program.SessionManager.GetAllSessionsWithOutSelf(this);
        HashSet<int> currentlyVisible = new HashSet<int>();

        foreach (Session other in otherSessions)
        {
            if (!other.Info.IsLoggedIn)
                continue;

            if (Position.MapId == other.Position.MapId && Position.DistanceTo(other.Position) <= range)
            {
                currentlyVisible.Add(other.PlayerId);

                if (!visiblePlayers.Contains(other.PlayerId))
                {
                    SendPlayerSpawn(other);

                    if (other.AddVisiblePlayer(PlayerId))
                    {
                        other.SendPlayerSpawn(this);
                    }
                }
                else if (broadcastBytes != null)
                {
                    other.SendAsync(broadcastBytes);
                }
            }
        }

        HashSet<int> oldVisible;
        oldVisible = visiblePlayers;
        visiblePlayers = currentlyVisible;

        foreach (int playerId in oldVisible)
        {
            if (!currentlyVisible.Contains(playerId))
            {
                SendAsync(PacketWriter.PlayerLeaveNotify(playerId));

                Session? other = Program.SessionManager.GetSessionById(playerId);
                if (other != null)
                {
                    other.SendAsync(PacketWriter.PlayerLeaveNotify(PlayerId));
                    other.RemoveVisiblePlayer(PlayerId);
                }
            }
        }
    }

    public bool AddVisiblePlayer(int playerId)
    {
        return visiblePlayers.Add(playerId);
    }

    public void ClearVisiblePlayers()
    {
        HashSet<int> oldVisible = visiblePlayers;
        visiblePlayers = new HashSet<int>();

        foreach (int playerId in oldVisible)
        {
            Session? other = Program.SessionManager.GetSessionById(playerId);
            if (other != null)
            {
                other.SendAsync(PacketWriter.PlayerLeaveNotify(PlayerId));
                other.RemoveVisiblePlayer(PlayerId);
            }
        }
    }

    public void RemoveVisiblePlayer(int playerId)
    {
        visiblePlayers.Remove(playerId);
    }

    public bool IsSkillOnCooldown(int skillId)
    {
        if (!skillCooldowns.TryGetValue(skillId, out var lastUsed))
            return false;

        var skillData = DataManager.GetSkillData(skillId);
        if (skillData == null)
            return true;

        return (DateTime.Now - lastUsed).TotalSeconds < skillData.Cooldown;
    }

    public void SetSkillCooldown(int skillId)
    {
        skillCooldowns[skillId] = DateTime.Now;
    }

    public void AddExp(int exp)
    {
        bool leveledUp = Stats.AddExp(exp);
        SendAsync(PacketWriter.ExpChangeNotify(Stats.Exp, DataManager.GetNextLevelRequiredExp(Stats.Level)));

        if (leveledUp)
        {
            byte[] levelUpNotify = PacketWriter.LevelUpNotify(PlayerId, Stats.Level);
            SendAsync(levelUpNotify);
            BroadcastInRange(levelUpNotify, Constants.MaxVisibleDistance);
            SendAsync(PacketWriter.HpChangeNotify(Stats.Hp, Stats.MaxHp));
            SendAsync(PacketWriter.MpChangeNotify(Stats.Mp, Stats.MaxMp));
        }

        Program.RedisManager.SaveCharacterFireAndForget(Info.AccountId, Stats, Position.MapId);
        MarkDirty();
    }

    public void TakeDamage(int damage)
    {
        Stats.TakeDamage(damage);
        SendAsync(PacketWriter.HpChangeNotify(Stats.Hp, Stats.MaxHp));
        Program.RedisManager.SaveCharacterFireAndForget(Info.AccountId, Stats, Position.MapId);
        MarkDirty();
    }
}
