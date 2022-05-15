using HarmonyLib;
using PlayerStatsSystem;
using Qurre.API;
namespace Qurre.Patches.Fixes
{
	[HarmonyPatch(typeof(HealthStat), nameof(HealthStat.MaxValue), MethodType.Getter)]
	internal static class MaxHp
	{
		public static bool Prefix(HealthStat __instance, ref float __result)
		{
			try
			{
				if (__instance is null) return true;
				Player pl = null;
				try { pl = Player.Get(__instance.Hub); } catch { return true; }
				if (pl is null) return true;
				__result = pl.MaxHp;
				return false;
			}
			catch (System.Exception e)
			{
				Log.Error($"umm, error in patching Fixes [MaxHp]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}