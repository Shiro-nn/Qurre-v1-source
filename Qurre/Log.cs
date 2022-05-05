using System;
using System.IO;
using System.Reflection;

namespace Qurre
{
    public static class Log
    {
        internal static bool Debugging => Plugin.Config.GetBool("Qurre_Debug", false, "Are Debug logs enabled?");

        internal static bool Logging => Plugin.Config.GetBool("Qurre_Logging", true, "Are errors saved to the log file?");

        internal static bool AllLogging => Plugin.Config.GetBool("Qurre_All_Logging", false, "Are all console output being saved to a log file?");

        public static void Info(object message) => ServerConsole.AddLog($"[INFO] [{Assembly.GetCallingAssembly().GetName().Name}] {message}", ConsoleColor.Yellow);

        public static void Debug(object message)
        {
            if (Debugging)
                ServerConsole.AddLog($"[DEBUG] [{Assembly.GetCallingAssembly().GetName().Name}] {message}", ConsoleColor.DarkGreen);
        }

        public static void Warn(object message, bool logging = true)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            ServerConsole.AddLog($"[WARN] [{assembly.GetName().Name}] {message}", ConsoleColor.DarkYellow);

            if (logging)
                LogTxt($"[WARN] [{assembly.GetName().Name}] {message}");
        }

        public static void Error(object message)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            ServerConsole.AddLog($"[ERROR] [{assembly.GetName().Name}] {message}", ConsoleColor.Red);
            LogTxt($"[ERROR] [{assembly.GetName().Name}] {message}");
        }

        public static void Custom(object message, string prefix = "Custom", ConsoleColor color = ConsoleColor.Gray) => ServerConsole.AddLog($"[{prefix}] [{Assembly.GetCallingAssembly().GetName().Name}] {message}", color);

        internal static void LogTxt(object message)
        {
            if (Logging)
            {
                if (!Directory.Exists(PluginManager.LogsDirectory))
                {
                    Directory.CreateDirectory(PluginManager.LogsDirectory);
                    Warn($"Logs directory not found - creating: {PluginManager.LogsDirectory}");
                }

                File.AppendAllText(Path.Combine(PluginManager.LogsDirectory, $"{Loader.Port}-log.txt"), $"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] {message}\n");
            }
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

                File.AppendAllText(Path.Combine(PluginManager.LogsDirectory, $"{Loader.Port}-all-logs.txt"), $"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] {message}\n");
            }
        }
    }
}