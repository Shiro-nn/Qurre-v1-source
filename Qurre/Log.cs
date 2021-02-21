using System;
using System.Reflection;
namespace Qurre
{
	public static class Log
	{
		internal static bool debug;
		public static void Info(string message)
		{
			Assembly assembly = Assembly.GetCallingAssembly();
			ServerConsole.AddLog($"[INFO] [{assembly.GetName().Name}] {message}", ConsoleColor.Yellow);
		}
		public static void Custom(string message, string prefix = "Custom", ConsoleColor color = ConsoleColor.Black)
		{
			Assembly assembly = Assembly.GetCallingAssembly();
			ServerConsole.AddLog($"[{prefix}] [{assembly.GetName().Name}] {message}", color);
		}
		public static void Debug(string message)
		{
			if (!debug) return;
			Assembly assembly = Assembly.GetCallingAssembly();
			ServerConsole.AddLog($"[DEBUG] [{assembly.GetName().Name}] {message}", ConsoleColor.DarkGreen);
		}
		public static void Warn(string message)
		{
			Assembly assembly = Assembly.GetCallingAssembly();
			ServerConsole.AddLog($"[WARN] [{assembly.GetName().Name}] {message}", ConsoleColor.DarkYellow);
		}
		public static void Error(string message)
		{
			Assembly assembly = Assembly.GetCallingAssembly();
			ServerConsole.AddLog($"[ERROR] [{assembly.GetName().Name}] {message}", ConsoleColor.Red);
		}
	}
}