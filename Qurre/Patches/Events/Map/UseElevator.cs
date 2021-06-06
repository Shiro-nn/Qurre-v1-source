using HarmonyLib;
using Qurre.API.Events;
namespace Qurre.Patches.Events.Map
{
	[HarmonyPatch(typeof(Lift), "UseLift")]
	public class UseElevator
	{
		private static bool Prefix(Lift __instance, ref bool __result)
		{
			var ev = new UseLiftEvent(API.Extensions.GetLift(__instance), __result);
			Qurre.Events.Map.useLift(ev);
			if (__result != ev.Result) ev.Allowed = false;
			__result = ev.Result;
			return ev.Allowed;
		}
	}
}