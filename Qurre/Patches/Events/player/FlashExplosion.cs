using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;
using Qurre.API;
using Qurre.API.Events;
using System;
namespace Qurre.Patches.Events.player
{
	[HarmonyPatch(typeof(FlashbangGrenade), nameof(FlashbangGrenade.PlayExplosionEffects))]
	internal static class FlashExplosionPatch
	{
		private static bool Prefix(FlashbangGrenade __instance)
		{
			try
			{
				var ev = new FlashExplosionEvent(Player.Get(__instance.PreviousOwner.Hub), __instance, __instance.transform.position);
				Qurre.Events.Invoke.Player.FlashExplosion(ev);
				return ev.Allowed;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching Player [FlashExplosion]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}