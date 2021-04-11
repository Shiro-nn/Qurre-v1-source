using MEC;
using System.IO;
namespace Qurre
{
    public class MainLoader
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
                    "\nQurre_spawn_blood: true\nQurre_ScpTrigger173: false\nQurre_AllUnit: true\nQurre_OnlyTutorialUnit: false\nQurre_banned: banned\nQurre_kicked: kicked" +
                    "\nQurre_BanOrKick_msg: You have been %bok%.\nQurre_reason: Reason");
            }
            Plugin.Config = new YamlConfig(PluginManager.ConfigsPath);
            Log.debug = Plugin.Config.GetBool("Qurre_debug", false);
            CustomNetworkManager.Modded = true;
            Timing.RunCoroutine(PluginManager.LoadPlugins());
            Events.modules.Etc.Load();
        }
    }
}