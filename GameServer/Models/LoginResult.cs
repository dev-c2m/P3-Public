namespace UnityServer.Models
{
    public struct LoginResult
    {
        public int AccountId { get; }
        public string Nickname { get; }

        public LoginResult(int accountId, string nickname)
        {
            AccountId = accountId;
            Nickname = nickname;
        }
    }
}
