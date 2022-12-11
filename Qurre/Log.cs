using System;
using System.IO;
using System.Reflection;
namespace Qurre
{
	public static class Log
	{
		internal static bool Debugging { get; set; } = true;
		internal static bool Logging { get; set; } = false;
		internal static bool AllLogging { get; set; } = false;
		public static void Info(object message)
		{
			string caller = "█████";
			try { caller = Assembly.GetCallingAssembly().GetName().Name; } catch { }
			ServerConsole.AddLog($"[INFO] [{caller}] {message}", ConsoleColor.Yellow);
		}
		public static void Debug(object message)
		{
			if (!Debugging) return;
			string caller = "█████";
			try { caller = Assembly.GetCallingAssembly().GetName().Name; } catch { }
			ServerConsole.AddLog($"[DEBUG] [{caller}] {message}", ConsoleColor.DarkGreen);
		}
		public static void Warn(object message)
		{
			string caller = "█████";
			try { caller = Assembly.GetCallingAssembly().GetName().Name; } catch { }
			string text = $"[WARN] [{caller}] {message}";
			ServerConsole.AddLog(text, ConsoleColor.DarkYellow);
			LogTxt(text);
		}
		public static void Error(object message)
		{
			string caller = "█████";
			try { caller = Assembly.GetCallingAssembly().GetName().Name; } catch { }
			string text = $"[ERROR] [{caller}] {message}";
			ServerConsole.AddLog(text, ConsoleColor.Red);
			LogTxt(text);
		}
		public static void Custom(object message, string prefix = "Custom", ConsoleColor color = ConsoleColor.Gray)
		{
			string caller = "█████";
			try { caller = Assembly.GetCallingAssembly().GetName().Name; } catch { }
			ServerConsole.AddLog($"[{prefix}] [{caller}] {message}", color);
		}
		internal static void LogTxt(object message)
		{
			if (!Logging) return;
			if (!Directory.Exists(PluginManager.LogsDirectory))
			{
				Directory.CreateDirectory(PluginManager.LogsDirectory);
				Custom($"Logs directory not found - creating: {PluginManager.LogsDirectory}", "WARN", ConsoleColor.DarkYellow);
			}
			File.AppendAllText(Path.Combine(PluginManager.LogsDirectory, $"{Loader.Port}-log.txt"), $"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] {message}\n");
		}
		internal static void AllLogsTxt(object message)
		{
			if (!AllLogging) return;
			if (!Directory.Exists(PluginManager.LogsDirectory))
			{
				Directory.CreateDirectory(PluginManager.LogsDirectory);
				Custom($"Logs directory not found - creating: {PluginManager.LogsDirectory}", "WARN", ConsoleColor.DarkYellow);
			}
			File.AppendAllText(Path.Combine(PluginManager.LogsDirectory, $"{Loader.Port}-all-logs.txt"), $"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] {message}\n");
		}
	}
}