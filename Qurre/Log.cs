using System;
using System.IO;
using System.Reflection;
namespace Qurre
{
	public static class Log
	{
		internal static bool debug;
		internal static bool Logging => Plugin.Config.GetBool("Qurre_logging", true) || Plugin.Config.GetBool("qurre_logging", true);
		internal static bool AllLogging => Plugin.Config.GetBool("Qurre_all_logging", false) || Plugin.Config.GetBool("qurre_all_logging", false);
		internal static bool AntiFlood => Plugin.Config.GetBool("Qurre_console_anti_flood", true) || Plugin.Config.GetBool("qurre_console_anti_flood", true);
		public static void Info(object message)
		{
			Assembly assembly = Assembly.GetCallingAssembly();
			ServerConsole.AddLog($"[INFO] [{assembly.GetName().Name}] {message}", ConsoleColor.Yellow);
		}
		public static void Custom(object message, string prefix = "Custom", ConsoleColor color = ConsoleColor.Black)
		{
			Assembly assembly = Assembly.GetCallingAssembly();
			ServerConsole.AddLog($"[{prefix}] [{assembly.GetName().Name}] {message}", color);
		}
		public static void Debug(object message)
		{
			if (!debug) return;
			Assembly assembly = Assembly.GetCallingAssembly();
			ServerConsole.AddLog($"[DEBUG] [{assembly.GetName().Name}] {message}", ConsoleColor.DarkGreen);
		}
		public static void Warn(object message)
		{
			Assembly assembly = Assembly.GetCallingAssembly();
			ServerConsole.AddLog($"[WARN] [{assembly.GetName().Name}] {message}", ConsoleColor.DarkYellow);
			LogTxt($"[WARN] [{assembly.GetName().Name}] {message}");
		}
		public static void Error(object message)
		{
			Assembly assembly = Assembly.GetCallingAssembly();
			ServerConsole.AddLog($"[ERROR] [{assembly.GetName().Name}] {message}", ConsoleColor.Red);
			LogTxt($"[ERROR] [{assembly.GetName().Name}] {message}");
		}
		internal static void LogTxt(object message)
		{
			if (!Directory.Exists(PluginManager.LogsDirectory) && Logging)
			{
				Directory.CreateDirectory(PluginManager.LogsDirectory);
				Warn($"Logs directory not found - creating: {PluginManager.LogsDirectory}");
			}
			var log = Path.Combine(PluginManager.LogsDirectory, $"{QurreModLoader.ModLoader.Port}-log.txt");
			if (!File.Exists(log)) File.Create(log).Close();
			File.AppendAllText(log, $"[{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}] {message}\n");
		}
		internal static void AllLogsTxt(object message)
        {
			if (AllLogging)
			{
				if (!Directory.Exists(PluginManager.LogsDirectory))
				{
					Directory.CreateDirectory(PluginManager.LogsDirectory);
					Warn($"Logs directory not found - creating: {PluginManager.LogsDirectory}");
				}
				var log = Path.Combine(PluginManager.LogsDirectory, $"{QurreModLoader.ModLoader.Port}-all-logs.txt");
				if (!File.Exists(log)) File.Create(log).Close();
				File.AppendAllText(log, $"[{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}] {message}\n");
			}
		}
	}
}