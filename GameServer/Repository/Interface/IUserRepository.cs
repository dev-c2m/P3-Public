using UnityServer.Models;
using UnityServer.Player;

namespace UnityServer.Repository.Interface
{
    public interface IUserRepository
    {
        bool HasCharacter(int accountId);
        void CreateCharacter(int accountId);
        CharacterData? GetCharacterByAccountId(int accountId);
        void SaveCharacterStats(int accountId, PlayerStats stats, int map);
    }
}
