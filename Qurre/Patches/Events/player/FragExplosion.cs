using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;
using Qurre.API;
using Qurre.API.Events;
using System;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
	[HarmonyPatch(typeof(ExplosionGrenade), nameof(ExplosionGrenade.Explode))]
	internal static class FragExplosionPatch
	{
		private static bool Prefix(ExplosionGrenade __instance)
		{
			try
			{
				Player thrower = Player.Get(__instance.PreviousOwner.Hub);
				Vector3 position = __instance.transform.position;
				var ev = new FragExplosionEvent(thrower, __instance, position);
				Qurre.Events.Invoke.Player.FragExplosion(ev);
				return ev.Allowed;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching Player [FragExplosion]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}