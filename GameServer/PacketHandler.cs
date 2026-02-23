using UnityServer;
using UnityServer.Data;
using UnityServer.Models;
using UnityServer.Player;
using UnityServer.Share;
using UnityServer.Share.Packets;
using UnityServer.Share.Packets.Request;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;

static class PacketHandler
{
    public static void Handle(Session session, Packet packet)
    {
        bool isLoggedIn = session.Info.AccountId != 0;

        switch (packet)
        {
            case TokenLoginRequest tokenLogin when !isLoggedIn:
                HandleTokenLogin(session, tokenLogin);
                break;
            case ChatRequest chat when isLoggedIn:
                HandleChat(session, chat);
                break;
            case PlayerMoveRequest move when isLoggedIn:
                HandlePlayerMove(session, move);
                break;
            case PlayerLeaveRequest leave when isLoggedIn:
                HandlePlayerLeave(session, leave);
                break;
            case RespawnRequest respawn when isLoggedIn:
                HandleRespawn(session, respawn);
                break;
            case AttendanceRequest attendance when isLoggedIn:
                HandleAttendance(session, attendance);
                break;
            case AttackRequest attack when isLoggedIn:
                HandleAttack(session, attack);
                break;
            case MoveMapRequest moveMap when isLoggedIn:
                HandleMoveMap(session, moveMap);
                break;
            case ItemMoveRequest itemMove when isLoggedIn:
                HandleItemMove(session, itemMove);
                break;
            case ItemUseRequest itemUse when isLoggedIn:
                HandleItemUse(session, itemUse);
                break;
            case ItemDropRequest itemDrop when isLoggedIn:
                HandleItemDrop(session, itemDrop);
                break;
            case GroundItemGetRequest groundItemGet when isLoggedIn:
                HandleGroundItemGet(session, groundItemGet);
                break;
        }
    }

    private static void HandleTokenLogin(Session session, TokenLoginRequest packet)
    {
        JwtPayload? payload = JWTValidator.Validate(packet.Token);
        if (payload == null)
        {
            session.SendAsync(PacketWriter.LoginResponse(false, Constants.ERROR_AUTH_FAILED));
            return;
        }

        int accountId = Convert.ToInt32(payload["accountId"]);
        string nickname = payload["nickname"].ToString();

        Program.DBManager.LoadPlayerData(session, accountId, nickname);
    }

    private static void HandleChat(Session session, ChatRequest packet)
    {
        byte[] bytes = PacketWriter.ChatNotify(session.Info.Nickname, packet.Message);
        session.Broadcast(bytes);
    }

    private static void HandlePlayerMove(Session session, PlayerMoveRequest packet)
    {
        session.Position.SetMovementInput(packet.DirectionX, packet.DirectionZ, packet.IsMoving, packet.RotationY);

        byte[] moveNotify = PacketWriter.PlayerMoveNotify(
            session.PlayerId, session.Position.Pos, session.Position.RotationY,
            packet.DirectionX, packet.DirectionZ, packet.IsMoving, Constants.MaxPlayerMoveSpeed);

        session.SendAsync(moveNotify);
        session.UpdateVisiblePlayers(Constants.MaxVisibleDistance, moveNotify);
    }

    private static void HandlePlayerLeave(Session session, PlayerLeaveRequest packet)
    {
        session.Leave();
    }

    private static void HandleRespawn(Session session, RespawnRequest packet)
    {
        if (!session.Stats.IsDead)
            return;

        session.Stats.Respawn();

        MapData? defaultMap = DataManager.GetMapData(Constants.DefaultMapId);
        Vector3 pos = new Vector3(Constants.RespawnX, Constants.RespawnY, Constants.RespawnZ);

        if (defaultMap != null)
        {
            pos.X = defaultMap.SpawnX;
            pos.Y = defaultMap.SpawnY;
            pos.Z = defaultMap.SpawnZ;
        }

        if (session.Position.MapId != Constants.DefaultMapId)
        {
            session.ClearVisiblePlayers();
            session.Position.SetMapId(Constants.DefaultMapId);
        }

        session.Position.Teleport(pos, 0f);

        byte[] bytes = PacketWriter.RespawnNotify(session.PlayerId, session.Info.Nickname, Constants.DefaultMapId, pos, 0f);
        session.SendAsync(bytes);
        session.UpdateVisiblePlayers(Constants.MaxVisibleDistance, bytes);

        Program.RedisManager.SaveCharacterFireAndForget(session.Info.AccountId, session.Stats, session.Position.MapId);
        session.MarkDirty();

        SendMonsterSpawnData(session);
        SendGroundItemSpawnData(session);
    }

    private static void HandleAttendance(Session session, AttendanceRequest packet)
    {
        int attendanceId = packet.AttendanceId;

        if (!DataManager.IsAttendanceAvailable(attendanceId))
        {
            session.SendAsync(PacketWriter.AttendanceResponse(0, 0, 0, 0, false, Constants.ERROR_ATTENDANCE_NOT_FOUND));
            return;
        }

        PlayerAttendance? target = null;
        foreach (PlayerAttendance attendance in session.Attendances)
        {
            if (attendance.AttendanceId == attendanceId)
            {
                target = attendance;
                break;
            }
        }

        if (target == null)
        {
            session.SendAsync(PacketWriter.AttendanceResponse(0, 0, 0, 0, false, Constants.ERROR_ATTENDANCE_DATA_NOT_FOUND));
            return;
        }

        if (target.DayCount >= DataManager.GetAttendanceTotalDays(attendanceId))
        {
            session.SendAsync(PacketWriter.AttendanceResponse(0, 0, 0, 0, false, Constants.ERROR_ATTENDANCE_COMPLETED));
            return;
        }

        if (!target.CanCheckIn)
        {
            session.SendAsync(PacketWriter.AttendanceResponse(0, 0, 0, 0, false, Constants.ERROR_ATTENDANCE_ALREADY_CHECKED));
            return;
        }

        target.CheckIn();
        Program.RedisManager.SaveAttendanceFireAndForget(session.Info.AccountId, attendanceId, target.DayCount, target.CanCheckIn);

        AttendanceRewardResult? reward = DataManager.GetAttendanceReward(attendanceId, target.DayCount - 1);

        if (reward != null)
        {
            int itemId = reward.Value.ItemId;
            int quantity = reward.Value.Quantity;

            InventoryItem? inventoryItem = InventoryManager.AddItem(session.Inventory, itemId, quantity);
            if (inventoryItem == null)
            {
                session.SendAsync(PacketWriter.AttendanceResponse(0, 0, 0, 0, false, Constants.ERROR_INVENTORY_FULL));
                return;
            }

            Program.RedisManager.UpdateInventorySlot(session.Info.AccountId, inventoryItem.SlotIndex, inventoryItem.ItemId, inventoryItem.Quantity);
            session.MarkDirty();

            session.SendAsync(PacketWriter.ItemAddNotify(itemId, inventoryItem.Quantity, inventoryItem.SlotIndex));
            session.SendAsync(PacketWriter.AttendanceResponse(attendanceId, target.DayCount, itemId, quantity, true));
        }
        else
        {
            session.SendAsync(PacketWriter.AttendanceResponse(attendanceId, target.DayCount, 0, 0, true));
        }

    }

    private static void HandleMoveMap(Session session, MoveMapRequest packet)
    {
        MapData? targetMap = DataManager.GetMapData(packet.MapId);
        if (targetMap == null)
        {
            session.SendAsync(PacketWriter.MoveMapResponse(false, 0, 0, 0, 0, Constants.ERROR_MAP_NOT_FOUND));
            return;
        }

        if (!DataManager.CanMoveToMap(session.Position.MapId, packet.MapId))
        {
            Logger.Warning($"{session.Position.MapId} -> {packet.MapId} 이동");
            session.SendAsync(PacketWriter.MoveMapResponse(false, 0, 0, 0, 0, Constants.ERROR_MAP_MOVE_NOT_ALLOWED));
            return;
        }

        if (session.Stats.IsDead)
        {
            session.SendAsync(PacketWriter.MoveMapResponse(false, 0, 0, 0, 0, Constants.ERROR_DEAD_MOVE));
            return;
        }

        session.ClearVisiblePlayers();
        session.Position.SetMapId(packet.MapId);
        session.Position.Teleport(new Vector3(targetMap.SpawnX, targetMap.SpawnY, targetMap.SpawnZ), 0f);
        session.SendAsync(PacketWriter.MoveMapResponse(true, packet.MapId, targetMap.SpawnX, targetMap.SpawnY, targetMap.SpawnZ));
        session.UpdateVisiblePlayers(Constants.MaxVisibleDistance);

        SendMonsterSpawnData(session);
        SendGroundItemSpawnData(session);
    }

    private static void HandleAttack(Session session, AttackRequest packet)
    {
        if (session.Stats.IsDead)
        {
            session.SendAsync(PacketWriter.AttackResponse(false, packet.SkillId, Constants.ERROR_DEAD_ATTACK));
            return;
        }

        SkillData? skillData = DataManager.GetSkillData(packet.SkillId);
        if (skillData == null)
        {
            session.SendAsync(PacketWriter.AttackResponse(false, packet.SkillId, Constants.ERROR_SKILL_NOT_FOUND));
            return;
        }

        if (session.Stats.Mp < skillData.MpCost)
        {
            session.SendAsync(PacketWriter.AttackResponse(false, packet.SkillId, Constants.ERROR_MP_NOT_ENOUGH));
            return;
        }

        if (session.IsSkillOnCooldown(packet.SkillId))
        {
            session.SendAsync(PacketWriter.AttackResponse(false, packet.SkillId, Constants.ERROR_SKILL_COOLDOWN));
            return;
        }

        session.Stats.UseMp(skillData.MpCost);
        session.SendAsync(PacketWriter.MpChangeNotify(session.Stats.Mp, session.Stats.MaxMp));
        session.SetSkillCooldown(packet.SkillId);

        Vector3 startPos = session.Position.Pos;
        Vector3 direction = new Vector3(packet.DirectionX, packet.DirectionY, packet.DirectionZ);

        Program.ProjectileManager.AddProjectile(session.PlayerId, session.Position.MapId, skillData, startPos, direction);
        session.SendAsync(PacketWriter.AttackResponse(true, packet.SkillId));

        byte[] notify = PacketWriter.AttackNotify(session.PlayerId, packet.SkillId, direction, session.Position.Pos);
        session.BroadcastInRange(notify, Constants.MaxVisibleDistance);

        Program.RedisManager.SaveCharacterFireAndForget(session.Info.AccountId, session.Stats, session.Position.MapId);
        session.MarkDirty();
    }


    private static void SendMonsterSpawnData(Session session)
    {
        List<byte[]> spawnNotifies = Program.MonsterManager.GetSpawnNotifiesForMap(session.Position.MapId);
        foreach (byte[] notify in spawnNotifies)
        {
            session.SendAsync(notify);
        }
    }

    private static void HandleItemMove(Session session, ItemMoveRequest packet)
    {
        if (packet.prevSlotIndex < 0 || packet.prevSlotIndex >= Constants.MaxInventorySlots ||
            packet.newSlotIndex < 0 || packet.newSlotIndex >= Constants.MaxInventorySlots)
        {
            session.SendAsync(PacketWriter.ItemMoveResponse(false, packet.prevSlotIndex, packet.newSlotIndex, Constants.ERROR_INVALID_SLOT));
            return;
        }

        bool success = InventoryManager.MoveItem(session.Inventory, packet.prevSlotIndex, packet.newSlotIndex);
        if (!success)
        {
            session.SendAsync(PacketWriter.ItemMoveResponse(false, packet.prevSlotIndex, packet.newSlotIndex, Constants.ERROR_ITEM_MOVE_FAILED));
            return;
        }

        InventoryItem? fromItem = InventoryManager.GetItem(session.Inventory, packet.prevSlotIndex);

        if (fromItem != null)
        {
            Program.RedisManager.SwapInventorySlots(session.Info.AccountId, packet.prevSlotIndex, packet.newSlotIndex);
        }
        else
        {
            Program.RedisManager.MoveInventorySlot(session.Info.AccountId, packet.prevSlotIndex, packet.newSlotIndex);
        }

        session.MarkDirty();
        session.SendAsync(PacketWriter.ItemMoveResponse(true, packet.prevSlotIndex, packet.newSlotIndex));
    }

    private static void HandleItemUse(Session session, ItemUseRequest packet)
    {
        if (session.Stats.IsDead)
        {
            session.SendAsync(PacketWriter.ItemUseResponse(false, packet.SlotIndex, 0, Constants.ERROR_DEAD_ITEM_USE));
            return;
        }

        InventoryItem? item = InventoryManager.GetItem(session.Inventory, packet.SlotIndex);
        if (item == null)
        {
            session.SendAsync(PacketWriter.ItemUseResponse(false, packet.SlotIndex, 0, Constants.ERROR_ITEM_NOT_FOUND));
            return;
        }

        if (item.ItemId != packet.ItemId)
        {
            session.SendAsync(PacketWriter.ItemUseResponse(false, packet.SlotIndex, 0, Constants.ERROR_ITEM_MISMATCH));
            return;
        }

        if (item.Quantity < packet.Quantity)
        {
            session.SendAsync(PacketWriter.ItemUseResponse(false, packet.SlotIndex, 0, Constants.ERROR_ITEM_QUANTITY_NOT_ENOUGH));
            return;
        }

        ItemData? itemData = DataManager.GetItemData(item.ItemId);
        if (itemData == null)
        {
            session.SendAsync(PacketWriter.ItemUseResponse(false, packet.SlotIndex, 0, Constants.ERROR_ITEM_DATA_NOT_FOUND));
            return;
        }

        if (itemData.Type != ItemType.Consumable)
        {
            session.SendAsync(PacketWriter.ItemUseResponse(false, packet.SlotIndex, 0, Constants.ERROR_ITEM_NOT_USABLE));
            return;
        }

        int totalValue = itemData.Value * packet.Quantity;

        switch (item.ItemId)
        {
            case 1:
                session.Stats.Heal(totalValue);
                session.SendAsync(PacketWriter.HpChangeNotify(session.Stats.Hp, session.Stats.MaxHp));
                break;

            case 2:
                session.Stats.RecoverMp(totalValue);
                session.SendAsync(PacketWriter.MpChangeNotify(session.Stats.Mp, session.Stats.MaxMp));
                break;

            case 3:
                bool leveledUp = session.Stats.AddExp(totalValue);
                session.SendAsync(PacketWriter.ExpChangeNotify(session.Stats.Exp, DataManager.GetNextLevelRequiredExp(session.Stats.Level)));

                if (leveledUp)
                {
                    byte[] levelUpNotify = PacketWriter.LevelUpNotify(session.PlayerId, session.Stats.Level);
                    session.SendAsync(levelUpNotify);
                    session.BroadcastInRange(levelUpNotify, Constants.MaxVisibleDistance);

                    session.SendAsync(PacketWriter.HpChangeNotify(session.Stats.Hp, session.Stats.MaxHp));
                    session.SendAsync(PacketWriter.MpChangeNotify(session.Stats.Mp, session.Stats.MaxMp));
                }
                break;
        }

        int remaining = InventoryManager.RemoveItem(session.Inventory, packet.SlotIndex, packet.Quantity);

        if (remaining == 0)
        {
            Program.RedisManager.DeleteInventorySlot(session.Info.AccountId, packet.SlotIndex);
        }
        else
        {
            Program.RedisManager.UpdateInventorySlot(session.Info.AccountId, packet.SlotIndex, item.ItemId, remaining);
        }

        Program.RedisManager.SaveCharacterFireAndForget(session.Info.AccountId, session.Stats, session.Position.MapId);
        session.MarkDirty();
        session.SendAsync(PacketWriter.ItemUseResponse(true, packet.SlotIndex, remaining));
    }

    private static void HandleItemDrop(Session session, ItemDropRequest packet)
    {
        InventoryItem? item = InventoryManager.GetItem(session.Inventory, packet.SlotIndex);
        if (item == null)
        {
            session.SendAsync(PacketWriter.ItemDropResponse(false, packet.SlotIndex, 0, Constants.ERROR_ITEM_NOT_FOUND));
            return;
        }

        if (item.ItemId != packet.ItemId)
        {
            session.SendAsync(PacketWriter.ItemDropResponse(false, packet.SlotIndex, 0, Constants.ERROR_ITEM_MISMATCH));
            return;
        }

        if (item.Quantity < packet.Quantity)
        {
            session.SendAsync(PacketWriter.ItemDropResponse(false, packet.SlotIndex, 0, Constants.ERROR_ITEM_QUANTITY_NOT_ENOUGH));
            return;
        }

        int remaining = InventoryManager.RemoveItem(session.Inventory, packet.SlotIndex, packet.Quantity);

        if (remaining == 0)
        {
            Program.RedisManager.DeleteInventorySlot(session.Info.AccountId, packet.SlotIndex);
        }
        else
        {
            Program.RedisManager.UpdateInventorySlot(session.Info.AccountId, packet.SlotIndex, item.ItemId, remaining);
        }

        session.MarkDirty();

        GroundItemInstance groundItem = Program.GroundItemManager.AddItem(packet.ItemId, packet.Quantity, session.Position.MapId, session.Position.Pos);
        byte[] spawnNotify = PacketWriter.GroundItemSpawnNotify(groundItem.UniqueId, groundItem.ItemId, groundItem.Quantity, groundItem.DropPos);
        IEnumerable<Session> sessionsInMap = Program.SessionManager.GetLoggedInSessionsInMap(session.Position.MapId);

        foreach (Session s in sessionsInMap)
        {
            s.SendAsync(spawnNotify);
        }

        session.SendAsync(PacketWriter.ItemDropResponse(true, packet.SlotIndex, remaining));
    }

    private static void HandleGroundItemGet(Session session, GroundItemGetRequest packet)
    {
        GroundItemInstance? groundItem = Program.GroundItemManager.GetItem(packet.GroundItemId);
        if (groundItem == null)
        {
            session.SendAsync(PacketWriter.GroundItemGetResponse(false, 0, 0, 0, Constants.ERROR_ITEM_NOT_FOUND));
            return;
        }

        if (groundItem.MapId != session.Position.MapId)
        {
            session.SendAsync(PacketWriter.GroundItemGetResponse(false, 0, 0, 0, Constants.ERROR_ITEM_DIFFERENT_MAP));
            return;
        }

        float distance = Vector3.Distance(groundItem.DropPos, session.Position.Pos);

        if (distance > Constants.GroundItemPickupDistance)
        {
            session.SendAsync(PacketWriter.GroundItemGetResponse(false, 0, 0, 0, Constants.ERROR_ITEM_TOO_FAR));
            return;
        }

        InventoryItem? inventoryItem = InventoryManager.AddItem(session.Inventory, groundItem.ItemId, groundItem.Quantity);
        if (inventoryItem == null)
        {
            session.SendAsync(PacketWriter.GroundItemGetResponse(false, 0, 0, 0, Constants.ERROR_INVENTORY_FULL));
            return;
        }

        Program.GroundItemManager.RemoveItem(packet.GroundItemId);
        Program.RedisManager.UpdateInventorySlot(session.Info.AccountId, inventoryItem.SlotIndex, inventoryItem.ItemId, inventoryItem.Quantity);
        session.MarkDirty();

        byte[] despawnNotify = PacketWriter.GroundItemDespawnNotify(packet.GroundItemId);
        IEnumerable<Session> sessionsInMap = Program.SessionManager.GetLoggedInSessionsInMap(session.Position.MapId);
        foreach (Session s in sessionsInMap)
        {
            s.SendAsync(despawnNotify);
        }

        session.SendAsync(PacketWriter.GroundItemGetResponse(true, groundItem.ItemId, groundItem.Quantity, inventoryItem.SlotIndex));
    }

    private static void SendGroundItemSpawnData(Session session)
    {
        List<byte[]> spawnNotifies = Program.GroundItemManager.GetSpawnNotirysInMap(session.Position.MapId);
        foreach (byte[] notify in spawnNotifies)
        {
            session.SendAsync(notify);
        }
    }
}
