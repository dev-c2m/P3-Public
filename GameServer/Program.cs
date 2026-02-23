using System.Net;
using System.Net.Sockets;
using UnityServer;
using UnityServer.Repository;

class Program
{
    public static GameLoop GameLoop = new GameLoop();

    public static SessionManager SessionManager = new SessionManager();
    public static MonsterManager MonsterManager = new MonsterManager();
    public static ProjectileManager ProjectileManager = new ProjectileManager();
    public static GroundItemManager GroundItemManager = new GroundItemManager();
    public static CancellationTokenSource CTS = new CancellationTokenSource();

    public static RedisManager RedisManager;
    public static DBManager DBManager;

    private static async Task Main(string[] args)
    {
        Init();

        TcpListener listener = new TcpListener(IPAddress.Any, Constants.ServerPort);
        listener.Start();
        Logger.Info($"서버 시작, 포트: {Constants.ServerPort}");

        try
        {
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync(CTS.Token);
                GameLoop.EnqueueCreate(client);
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            DBManager.SaveAllPlayersToDb();
            RedisManager.Close();

            listener.Stop();
            Logger.Info("서버 종료 완료");
        }
    }

    private static void Init()
    {
        // Ctrl+C 이벤트
        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            CTS.Cancel();
            Logger.Info("서버 종료 시작");
        };

        DataManager.LoadAll();
        InitDatabase();
        MonsterManager.Initialize();
        GameLoop.Start();
    }

    private static void InitDatabase()
    {
        UserRepository userRepository = new UserRepository(Constants.DBConnectionString);
        AttendanceRepository attendanceRepository = new AttendanceRepository(Constants.DBConnectionString);
        InventoryRepository inventoryRepository = new InventoryRepository(Constants.DBConnectionString);

        RedisManager = new RedisManager(Constants.RedisConnectionString);
        DBManager = new DBManager(userRepository, inventoryRepository, attendanceRepository, RedisManager);
    }
}
