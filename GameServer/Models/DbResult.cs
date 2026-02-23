using UnityServer.Player;

namespace UnityServer.Models
{
    public enum DbResultType
    {
        LoginSuccess,
        LoginFailed,
    }

    public class DbResult
    {
        public Session Session { get; }
        public DbResultType Type { get; }

        public LoginResult? LoginData { get; private set; }
        public CharacterData? Character { get; private set; }
        public List<InventoryItem>? Items { get; private set; }
        public List<PlayerAttendance>? Attendances { get; private set; }

        public string Message { get; private set; }

        private DbResult(Session session, DbResultType type, string message = "")
        {
            Session = session;
            Type = type;
            Message = message;
        }

        public static DbResult LoginSuccess(Session session, LoginResult loginData, CharacterData? character, List<InventoryItem> items, List<PlayerAttendance> attendances)
        {
            DbResult result = new DbResult(session, DbResultType.LoginSuccess);
            result.LoginData = loginData;
            result.Character = character;
            result.Items = items;
            result.Attendances = attendances;
            return result;
        }

        public static DbResult LoginFail(Session session, string message)
        {
            return new DbResult(session, DbResultType.LoginFailed, message);
        }

    }
}
