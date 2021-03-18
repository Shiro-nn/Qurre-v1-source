using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QurreModLoader;
namespace Qurre.API
{
    public static class Server
    {
        private static Player host;
        private static Inventory hinv;
        public static ServerConsole ServerConsole => ServerConsole.singleton;
        public static ushort Port => ModLoader.Port;
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
                if (host == null)
                    host = Player.Get(PlayerManager.localPlayer);
                return host;
            }
        }
        public static Inventory InventoryHost
        {
            get
            {
                if (hinv == null)
                    hinv = Player.Get(PlayerManager.localPlayer).Inventory;
                return hinv;
            }
        }
        public static int MaxPlayers()
        {
            CustomNetworkManager nm = new CustomNetworkManager();
            return nm.maxConnections;
        }
        public static void MaxPlayers(int amount)
        {
            CustomNetworkManager nm = new CustomNetworkManager();
            nm.maxConnections = amount;
        }
        public static List<TObject> GetObjectsOf<TObject>() where TObject : UnityEngine.Object => UnityEngine.Object.FindObjectsOfType<TObject>().ToList();
        public static TObject GetObjectOf<TObject>() where TObject : UnityEngine.Object => UnityEngine.Object.FindObjectOfType<TObject>();
    }
}
