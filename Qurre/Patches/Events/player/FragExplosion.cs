using Grenades;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using System;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
	[HarmonyPatch(typeof(FragGrenade), nameof(FragGrenade.ServersideExplosion))]
	internal static class FragExplosionPatch
	{
		private static bool Prefix(FragGrenade __instance, ref bool __result)
		{
			try
			{
				Player thrower = Player.Get(__instance.thrower.gameObject);
				Vector3 position = __instance.transform.position;
				var ev = new FragExplosionEvent(thrower, position);
				Qurre.Events.Invoke.Player.FragExplosion(ev);
				if (ev.Allowed) return true;
				__result = false;
				return false;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching Player [FragExplosion]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}