using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class SessionManager
{
    private Dictionary<int, Session> sessions = new Dictionary<int, Session>();
    private Dictionary<int, Session> signInSessions = new Dictionary<int, Session>();
    private int nextPlayerId = 1;

    public Session Create(TcpClient client)
    {
        Session session = new Session(client, nextPlayerId);
        sessions.Add(nextPlayerId, session);
        nextPlayerId++;
        return session;
    }

    public void Remove(Session session)
    {
        sessions.Remove(session.PlayerId);
        if (session.Info.AccountId != 0)
        {
            signInSessions.Remove(session.Info.AccountId);
        }
    }

    public bool IsLoggedIn(int accountId)
    {
        return signInSessions.ContainsKey(accountId);
    }

    public void RegisterLogin(int accountId, Session session)
    {
        signInSessions[accountId] = session;
    }

    public IEnumerable<Session> GetAllSessions()
    {
        return sessions.Values;
    }

    public IEnumerable<Session> GetAllSessionsWithOutSelf(Session self)
    {
        return sessions.Values.Where(s => s != self);
    }

    public Session? GetSessionById(int playerId)
    {
        sessions.TryGetValue(playerId, out Session? session);
        return session;
    }

    public IEnumerable<Session> GetLoggedInSessionsInMap(int mapId)
    {
        return signInSessions.Values.Where(s => s.Position.MapId == mapId);
    }
}
