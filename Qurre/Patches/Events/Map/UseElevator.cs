using HarmonyLib;
using MEC;
using Qurre.API.Events;
using System;
namespace Qurre.Patches.Events.Map
{
	[HarmonyPatch(typeof(Lift), "UseLift")]
	public class UseElevator
	{
		private static bool Prefix(Lift __instance, ref bool __result)
		{
			try
			{
				var ev = new UseLiftEvent(API.Extensions.GetLift(__instance), __result);
				if (!ev.Lift.Operative || API.Controllers.Alpha.TimeToDetonation == 0 || ev.Lift.Locked) ev.Allowed = false;
				else ev.Allowed = true;
				Qurre.Events.Invoke.Map.UseLift(ev);
				__result = ev.Allowed;
				if (ev.Allowed)
				{
					Timing.RunCoroutine(ev.Lift.Elevator._LiftAnimation(), Segment.FixedUpdate);
					ev.Lift.Operative = false;
				}
				return false;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching Map [UseLift]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}