using MEC;
using Qurre.API;
using Qurre.API.Addons;
using RemoteAdmin;
using System.IO;
namespace Qurre
{
    internal class Loader : ICharacterLoader
    {
        internal static ushort Port => global::Loader.Port;
        internal static JsonConfig Config { get; private set; }
        internal static bool AllUnits { get; private set; } = true;
        internal static bool OnlyTutorialUnit { get; private set; } = false;
        internal static bool SpawnBlood { get; private set; } = false;
        internal static bool Better268 { get; private set; } = false;
        internal static bool BetterHints { get; private set; } = false;
        internal static string[] ReloadAccess { get; private set; } = new string[] { };

        public void Init()
        {
            Log.Info("Initializing Qurre...");

            if (!Directory.Exists(PluginManager.ConfigsDirectory))
            {
                Log.Custom($"Configs directory not found. Creating: {PluginManager.ConfigsDirectory}", "WARN", System.ConsoleColor.DarkYellow);
                Directory.CreateDirectory(PluginManager.ConfigsDirectory);
            }

            try { ConfigSetup(); } catch { }

            Plugin.Config = new(); //outdate

            Server.DataBase = new API.DataBase.Client();
            CustomNetworkManager.Modded = true;
            Timing.RunCoroutine(PluginManager.LoadPlugins());

            Events.Modules.Etc.Load();
            Prefabs.Init();

            CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(Events.Modules.Commands.Reload.Instance);
            GameCore.Console.singleton.ConsoleCommandHandler.RegisterCommand(Events.Modules.Commands.Reload.Instance);
        }

        private static void ConfigSetup()
        {
            JsonConfig.Init();
            Config = new("Qurre");

            Log.Debugging = Config.SafeGetValue("Debug", false, "Are Debug logs enabled?");
            Log.Logging = Config.SafeGetValue("Logging", true, "Are errors saved to the log file?");
            Log.AllLogging = Config.SafeGetValue("AllLogging", false, "Are all console output being saved to a log file?");

            AllUnits = Config.SafeGetValue("AllUnit", true, "Should I show the Qurre version on Units for all roles?");
            OnlyTutorialUnit = Config.SafeGetValue("OnlyTutorialsUnit", false, "Should I show the Qurre version on Units only for the Tutorial role?");
            SpawnBlood = Config.SafeGetValue("SpawnBlood", true, "Allow the appearance of blood?");
            Better268 = Config.SafeGetValue("Better268", false, "SCP 079 & SCP 096 will not see the wearer of SCP 268");
            BetterHints = Config.SafeGetValue("BetterHints", false, "Enable Addon [BetterHints]?");
            ReloadAccess = Config.SafeGetValue("ReloadAccess", new string[] { "owner", "UserId64@steam", "UserDiscordId@discord" }, "Those who can use the \"reload\" command");

            try { Patches.Events.Player.BanAndKick.SetUpConfigs(); } catch { }

            JsonConfig.UpdateFile();
        }
    }
}