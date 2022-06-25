using HarmonyLib;
namespace Qurre.Patches.Modules
{
	[HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.ReloadServerName))]
	internal static class ServerNamePatch
	{
		internal static void Postfix()
			=> ServerConsole._serverName += $" <color=#00000000><size=1>Qurre v{PluginManager.Version}</size></color>";
	}
}