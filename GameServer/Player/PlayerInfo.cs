using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityServer.Player
{
    public class PlayerInfo
    {
        public int AccountId { get; private set; }
        public string Nickname { get; private set; } = string.Empty;
        public bool IsLoggedIn => AccountId != 0;

        public void SetAccountId(int accountId)
        {
            AccountId = accountId;
        }

        public void SetNickname(string nickname)
        {
            Nickname = nickname;
        }
    }
}
