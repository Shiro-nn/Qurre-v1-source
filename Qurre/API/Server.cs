using System.Collections.Generic;
using System.Linq;
using QurreModLoader;
namespace Qurre.API
{
    public static class Server
    {
        private static Player host;
        private static Inventory hinv;
        public static ServerConsole ServerConsole => ServerConsole.singleton;
        public static DataBase.DataBase DataBase { get; internal set; }
        public static ushort Port => ModLoader.Port;
        public static string Ip => ServerConsole.Ip;
        public static string Name
        {
            get => ServerConsole._serverName;
            set
            {
                ServerConsole._serverName = value;
                ServerConsole.RefreshServerName();
            }
        }
        public static int Slots
        {
            get => CustomNetworkManager.slots;
            set => CustomNetworkManager.slots = value;
        }
        public static bool FriendlyFire
        {
            get => ServerConsole.FriendlyFire;
            set
            {
                ServerConsole.FriendlyFire = value;
                foreach (Player pl in Player.List) pl.FriendlyFire = value;
            }
        }
        public static Player Host
        {
            get
            {
                if (host == null || host.ReferenceHub == null) host = new Player(PlayerManager.localPlayer);
                return host;
            }
        }
        public static Inventory InventoryHost
        {
            get
            {
                if (hinv == null) hinv = ReferenceHub.GetHub(PlayerManager.localPlayer).inventory;
                return hinv;
            }
        }
        public static int MaxPlayers
        {
            get
            {
                CustomNetworkManager nm = new CustomNetworkManager();
                return nm.maxConnections;
            }
            set
            {
                CustomNetworkManager nm = new CustomNetworkManager();
                nm.maxConnections = value;
            }
        }
        public static List<TObject> GetObjectsOf<TObject>() where TObject : UnityEngine.Object => UnityEngine.Object.FindObjectsOfType<TObject>().ToList();
        public static TObject GetObjectOf<TObject>() where TObject : UnityEngine.Object => UnityEngine.Object.FindObjectOfType<TObject>();
        public static void Restart()
        {
            ServerConsole.AddOutputEntry(default(ServerOutput.ExitActionRestartEntry));
            umm.SetNextRoundAction(ServerStatic.NextRoundAction.Restart);
            umm.StaticChangeLevel(true);
        }
    }
}