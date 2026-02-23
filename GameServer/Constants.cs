
using System.Numerics;

namespace UnityServer
{
    static class Constants
    {
        public enum ReceivedPacketType
        {
            Packet,
            Create,
            Leave,
            DbResult,
        }

        // 서버 설정
        public const int ServerPort = 7777;

        // 플레이어 스탯
        public const float MaxPlayerMoveSpeed = 10f;
        public const float MaxVisibleDistance = 30f;
        public const int MaxLevel = 100;

        public const int DefaultMapId = 1;

        // 기본 리스폰 위치
        public const float RespawnX = 0f;
        public const float RespawnY = 1f;
        public const float RespawnZ = 0f;

        public const int SendTick = 33;
        public const int PositionSyncInterval = 50;

        // 몬스터
        public const int MonsterRespawnInterval = 15;
        public const int MonsterMoveInterval = 2;
        public const int MonsterMoveMinWait = 1; 
        public const int MonsterMoveMaxWait = 3;
        public const float MonsterMoveSpeed = 4f;

        // 아이템
        public const float GroundItemDropOffset = 1.5f;
        public const float GroundItemPickupDistance = 3f;
        public const int MaxInventorySlots = 30;

        // 투사체
        public const float ProjectileHitRadius = 2.0f;

        // DB 
        public const string DBConnectionString = "";
        public const string RedisConnectionString = "localhost:6379";
        public const int DbSaveInterval = 180; // 3분

        // 에러 메시지
        public const string ERROR_AUTH_FAILED = "인증에 실패했습니다.";

        public const string ERROR_ATTENDANCE_NOT_FOUND = "존재하지 않는 출석입니다.";
        public const string ERROR_ATTENDANCE_DATA_NOT_FOUND = "출석 데이터가 없습니다.";
        public const string ERROR_ATTENDANCE_COMPLETED = "더 이상 출석할 수 없습니다.";
        public const string ERROR_ATTENDANCE_ALREADY_CHECKED = "이미 출석을 한 상태입니다.";

        public const string ERROR_MAP_NOT_FOUND = "존재하지 않는 맵입니다.";
        public const string ERROR_MAP_MOVE_NOT_ALLOWED = "이동할 수 없는 맵입니다.";
        public const string ERROR_DEAD_MOVE = "사망 상태에서는 이동할 수 없습니다.";

        public const string ERROR_DEAD_ATTACK = "사망 상태에서는 공격할 수 없습니다.";
        public const string ERROR_SKILL_NOT_FOUND = "존재하지 않는 스킬입니다.";
        public const string ERROR_MP_NOT_ENOUGH = "MP가 부족합니다.";
        public const string ERROR_SKILL_COOLDOWN = "쿨다운 중입니다.";

        public const string ERROR_INVALID_SLOT = "유효하지 않은 슬롯입니다.";
        public const string ERROR_ITEM_MOVE_FAILED = "아이템 이동에 실패했습니다.";
        public const string ERROR_DEAD_ITEM_USE = "사망 상태에서는 사용할 수 없습니다.";
        public const string ERROR_ITEM_NOT_FOUND = "아이템이 존재하지 않습니다.";
        public const string ERROR_ITEM_MISMATCH = "아이템 정보가 일치하지 않습니다.";
        public const string ERROR_ITEM_QUANTITY_NOT_ENOUGH = "수량이 부족합니다.";
        public const string ERROR_ITEM_DATA_NOT_FOUND = "아이템 데이터가 존재하지 않습니다.";
        public const string ERROR_ITEM_NOT_USABLE = "사용할 수 없는 아이템입니다.";
        public const string ERROR_ITEM_DIFFERENT_MAP = "같은 맵에 있지 않습니다.";
        public const string ERROR_ITEM_TOO_FAR = "너무 멀리 있습니다.";
        public const string ERROR_INVENTORY_FULL = "인벤토리가 가득 찼습니다.";
    }
}
