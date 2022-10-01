using Footprinting;
using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;
using Qurre.API.Events;
using System;
using UnityEngine;
namespace Qurre.Patches.Events.Player
{
	using Qurre.API;
	[HarmonyPatch(typeof(ExplosionGrenade), nameof(ExplosionGrenade.Explode))]
	internal static class FragExplosionPatch
	{
		private static bool Prefix(Footprint attacker, Vector3 position, ExplosionGrenade settingsReference)
		{
			try
			{
				var ev = new FragExplosionEvent(Player.Get(attacker.Hub), settingsReference, position);
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