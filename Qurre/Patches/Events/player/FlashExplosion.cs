using Grenades;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using QurreModLoader;
using System;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
	[HarmonyPatch(typeof(FlashGrenade), nameof(FlashGrenade.ServersideExplosion))]
	internal static class FlashExplosionPatch
	{
		private static bool Prefix(FlashGrenade __instance, ref bool __result)
		{
			try
			{
				Player thrower = Player.Get(__instance.thrower().gameObject);
				Vector3 position = __instance.transform.position;
				var ev = new FlashExplosionEvent(thrower, position);
				Qurre.Events.Invoke.Player.FlashExplosion(ev);
				if (ev.Allowed) return true;
				__result = false;
				return false;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching Player [FlashExplosion]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}