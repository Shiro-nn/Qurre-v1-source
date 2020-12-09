using MEC;
using System.IO;

namespace Qurre
{
	public class MainLoader
	{
		public static void QurreLoad()
        {
            Log.Info($"Initalizing Mod Loader");
            if (!Directory.Exists(PluginManager.ConfigsDirectory))
            {
                Log.Warn($"Cfg directory not found - creating: {PluginManager.ConfigsDirectory}");
                Directory.CreateDirectory(PluginManager.ConfigsDirectory);
            }
            PluginManager.ConfigsPath = Path.Combine(PluginManager.ConfigsDirectory, $"{QurreModLoader.ModLoader.Port}-cfg.yml");
            if (!File.Exists(PluginManager.ConfigsPath))
                File.Create(PluginManager.ConfigsPath).Close();

            Plugin.Config = new YamlConfig(PluginManager.ConfigsPath);
            Log.debug = Plugin.Config.GetBool("Qurre_debug", false);

            CustomNetworkManager.Modded = true;
            Timing.RunCoroutine(PluginManager.LoadPlugins());
        }
    }
}