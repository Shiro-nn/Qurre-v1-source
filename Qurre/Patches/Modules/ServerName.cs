using HarmonyLib;
namespace Qurre.Patches.Modules
{
	[HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.ReloadServerName))]
	public static class ServerNamePatch
	{
		internal static void Postfix()
		{
			if (ServerConsole._serverName.Contains("<color=#00000000>"))
			{
				bool del = false;
				string[] spearator = { "<color=#00000000>" };
				string[] strlist = ServerConsole._serverName.Split(spearator, 2, System.StringSplitOptions.RemoveEmptyEntries);
				foreach (string s in strlist)
				{
					if (del) ServerConsole._serverName = ServerConsole._serverName.Replace(s, "").Replace("<color=#00000000>", "");
					del = true;
				}
			}
			ServerConsole._serverName += $" <color=#00000000><size=1>Qurre v{PluginManager.Version}</size></color>";
			Log.Info($"Qurre v{PluginManager.Version}");
		}
	}
}