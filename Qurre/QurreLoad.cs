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
                Log.Warn($"Cfg directory not found - creating: {PluginManager.ConfigsDirectory}");
                Directory.CreateDirectory(PluginManager.ConfigsDirectory);
            }
            PluginManager.ConfigsPath = Path.Combine(PluginManager.ConfigsDirectory, $"{QurreModLoader.ModLoader.Port}-cfg.yml");
            if (!File.Exists(PluginManager.ConfigsPath))
            {
                File.Create(PluginManager.ConfigsPath).Close();
                File.WriteAllText(PluginManager.ConfigsPath, "Qurre_debug: false\nQurre_logging: true\nQurre_all_logging: false\nQurre_console_anti_flood: true" +
                    "\nQurre_spawn_blood: true\nQurre_banned: banned\nQurre_kicked: kicked" +
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