using InventorySystem;
using RoundRestarting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Qurre.API
{
    public static class Server
    {
        private static Player _host;
        private static Inventory _hostInventory;

        public static ServerConsole ServerConsole => ServerConsole.singleton;
        public static DataBase.Client DataBase { get; internal set; }
        public static ushort Port => global::Loader.Port;
        public static string Ip => ServerConsole.Ip;
        ///<summary>
        ///<para>if true, then no items will be issued during the escape and the escaped person will not change his location.</para>
        ///</summary>
        public static bool RealEscape { get; set; } = false;
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
                ServerConfigSynchronizer.Singleton.RefreshMainBools();
                PlayerStatsSystem.AttackerDamageHandler.RefreshConfigs();

                foreach (Player pl in Player.List) 
                    pl.FriendlyFire = value;
            }
        }
        public static bool HeavilyModded
        {
            get => CustomNetworkManager.HeavilyModded;
            set => CustomNetworkManager.HeavilyModded = value;
        }
        public static float SpawnProtectDuration
        {
            get => CharacterClassManager.SProtectedDuration;
            set => CharacterClassManager.SProtectedDuration = value;
        }
        public static float LaterJoinTime
        {
            get => CharacterClassManager.LaterJoinTime;
            set => CharacterClassManager.LaterJoinTime = value;
        }
        public static bool LaterJoinEnabled
        {
            get => CharacterClassManager.LaterJoinEnabled;
            set => CharacterClassManager.LaterJoinEnabled = value;
        }
        public static Player Host
        {
            get
            {
                if (_host == null || _host.ReferenceHub == null)
                {
                    _host = new Player(PlayerManager.hostHub);
                }
                return _host;
            }
        }
        public static Inventory InventoryHost
        {
            get
            {
                if (_hostInventory == null)
                {
                    _hostInventory = _host.Inventory;
                }

                return _hostInventory;
            }
        }
        public static int MaxConnections
        {
            get => CustomNetworkManager.singleton.maxConnections;
            set => CustomNetworkManager.singleton.maxConnections = value;
        }
        public static List<TObject> GetObjectsOf<TObject>() where TObject : UnityEngine.Object => UnityEngine.Object.FindObjectsOfType<TObject>().ToList();
        public static TObject GetObjectOf<TObject>() where TObject : UnityEngine.Object => UnityEngine.Object.FindObjectOfType<TObject>();
        public static void Restart()
        {
            ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
            RoundRestart.ChangeLevel(true);
        }
        public static void Exit() => Shutdown.Quit();
        public static void InvokeStaticMethod(this Type type, string methodName, object[] param)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public;
            MethodInfo info = type.GetMethod(methodName, flags);
            info?.Invoke(null, param);
        }
    }
}