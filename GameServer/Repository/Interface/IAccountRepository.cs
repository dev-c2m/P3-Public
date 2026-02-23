using UnityServer.Models;

namespace UnityServer.Repository.Interface
{
    public interface IAccountRepository
    {
        bool IsLoginIdExists(string loginId);
        int CreateAccount(string loginId, string password, string nickname);
        LoginResult? ValidateLogin(string loginId, string password);
        bool IsNicknameExists(string nickname);
    }
}
