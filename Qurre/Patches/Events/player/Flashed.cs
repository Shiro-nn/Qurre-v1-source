using CustomPlayerEffects;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using System;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
	[HarmonyPatch(typeof(Flashed), nameof(Flashed.Flashable))]
	internal static class FlashedPatch
	{
		private static bool Prefix(Flashed __instance, ref bool __result, ReferenceHub throwerPlayerHub, Vector3 sourcePosition, int ignoreMask)
		{
			try
			{
				Player thrower = Player.Get(throwerPlayerHub);
				Player target = Player.Get(__instance.Hub);
				bool allowed = __instance.Hub != throwerPlayerHub && throwerPlayerHub.weaponManager.GetShootPermission(__instance.Hub.characterClassManager.CurRole.team)
					&& !Physics.Linecast(sourcePosition, __instance.Hub.PlayerCameraReference.position, ignoreMask);
				var ev = new FlashedEvent(thrower, target, sourcePosition, ignoreMask, allowed);
				Qurre.Events.Invoke.Player.Flashed(ev);
				__result = ev.Allowed;
				return false;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching Player [Flashed]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}