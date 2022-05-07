using MEC;
using Qurre.API;
using RemoteAdmin;
using System.IO;
namespace Qurre
{
    internal static class Loader
    {
        internal static ushort Port => global::Loader.Port;
        internal static bool AllUnits => Plugin.Config.GetBool("Qurre_AllUnit", false, "Should I show the Qurre version on Units for all roles?");
        internal static bool OnlyTutorialUnit => Plugin.Config.GetBool("Qurre_OnlyTutorialUnit", false, "Should I show the Qurre version on Units only for the Tutorial role?");
        internal static bool SpawnBlood => Plugin.Config.GetBool("Qurre_Spawn_Blood", true, "Allow the appearance of blood?");
        internal static bool ScpTrigger173 => Plugin.Config.GetBool("Qurre_ScpTrigger173", false, "Can other SCPs look at SCP-173?");
        internal static bool Better268 => Plugin.Config.GetBool("Qurre_Better268", false, "SCP 079 & SCP 096 will not see the wearer of SCP 268");
        internal static bool LateJoinSpawn => Plugin.Config.GetBool("Qurre_LateJoinSpawn", true, "If enabled, will spawn those who entered after the start of the round");
        internal static string ReloadAccess => Plugin.Config.GetString("Qurre_ReloadAccess", "owner, 746538986@steam,309800126721@discord", "Those who can use the \"reload\" command");
        public static void QurreLoad()
        {
            Log.Info("Initializing Qurre...");
            if (!Directory.Exists(PluginManager.ConfigsDirectory))
            {
                Log.Warn($"Configs directory not found - creating: {PluginManager.ConfigsDirectory}", false);
                Directory.CreateDirectory(PluginManager.ConfigsDirectory);
            }
            PluginManager.ConfigsPath = Path.Combine(PluginManager.ConfigsDirectory, $"{Port}-cfg.yml");
            if (!File.Exists(PluginManager.ConfigsPath))
            {
                File.Create(PluginManager.ConfigsPath).Close();
                Plugin.Config = new Config();
                _ = Log.Debugging;
                _ = Log.Logging;
                _ = Log.AllLogging;
                _ = AllUnits;
                _ = OnlyTutorialUnit;
                _ = SpawnBlood;
                _ = ScpTrigger173;
                _ = Better268;
                _ = LateJoinSpawn;
                _ = ReloadAccess;
                using StreamWriter sw = new(PluginManager.ConfigsPath, true, System.Text.Encoding.Default);
                sw.Write("Qurre_Banned: banned\nQurre_Kicked: kicked\nQurre_BanOrKick_msg: You have been %bok%.\nQurre_Reason: Reason\n");
                sw.Close();
            }
            else Plugin.Config = new Config();
            Server.DataBase = new API.DataBase.Client();
            CustomNetworkManager.Modded = true;
            Timing.RunCoroutine(PluginManager.LoadPlugins());
            Events.Modules.Etc.Load();
            API.Addons.Prefabs.Init();
            CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(Events.Modules.Commands.Reload.Instance);
            GameCore.Console.singleton.ConsoleCommandHandler.RegisterCommand(Events.Modules.Commands.Reload.Instance);
        }
    }
}
