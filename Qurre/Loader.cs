using MEC;
using Qurre.API;
using System.IO;
namespace Qurre
{
    internal class Loader
    {
        public static void QurreLoad()
        {
            Log.Info($"Initializing Qurre...");
            if (!Directory.Exists(PluginManager.ConfigsDirectory))
            {
                Log.Custom($"Cfg directory not found - creating: {PluginManager.ConfigsDirectory}", "Warn", System.ConsoleColor.DarkYellow);
                Directory.CreateDirectory(PluginManager.ConfigsDirectory);
            }
            PluginManager.ConfigsPath = Path.Combine(PluginManager.ConfigsDirectory, $"{QurreModLoader.ModLoader.Port}-cfg.yml");
            if (!File.Exists(PluginManager.ConfigsPath))
            {
                File.Create(PluginManager.ConfigsPath).Close();
                File.WriteAllText(PluginManager.ConfigsPath, "Qurre_debug: false\nQurre_logging: true\nQurre_all_logging: false\nQurre_console_anti_flood: true" +
                    "\nqurre_spawn_blood: true\nQurre_ScpTrigger173: false\nQurre_AllUnit: false\nQurre_OnlyTutorialUnit: false" +
                    "\n#SCP 079 and SCP 096 will not see the wearer of SCP 268\nQurre_Better268: false\nQurre_banned: banned\nQurre_kicked: kicked" +
                    "\nQurre_BanOrKick_msg: You have been %bok%.\nQurre_reason: Reason\n");
            }
            Plugin.Config = new Config();
            Log.debug = Plugin.Config.GetBool("Qurre_debug", false);
            Server.DataBase = new API.DataBase.DataBase();
            CustomNetworkManager.Modded = true;
            Timing.RunCoroutine(PluginManager.LoadPlugins());
            Events.modules.Etc.Load();
        }
    }
}