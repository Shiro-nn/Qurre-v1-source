using CustomPlayerEffects;
using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;
using Qurre.API;
using Qurre.API.Events;
using System;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
	[HarmonyPatch(typeof(FlashbangGrenade), nameof(FlashbangGrenade.ProcessPlayer))]
	internal static class FlashedPatch
	{
		private static bool Prefix(FlashbangGrenade __instance, ReferenceHub hub)
		{
			try
			{
				Player thrower = Player.Get(__instance.PreviousOwner.Hub);
				Player target = Player.Get(hub);
				Vector3 vector = __instance.transform.position - hub.PlayerCameraReference.position;
				float num = vector.magnitude;
				float num2 = __instance._deafenDurationOverDistance.Evaluate(num);
				if (num2 > __instance._minimalEffectDuration)
				{
					hub.playerEffectsController.EnableEffect<CustomPlayerEffects.Deafened>(num2, true);
				}
				if (Physics.Linecast(__instance.transform.position, hub.PlayerCameraReference.position, __instance._blindingMask))
				{
					return false;
				}
				if (hub.transform.position.y > 900f)
				{
					num /= __instance._surfaceZoneDistanceIntensifier;
				}
				float num3 = __instance._blindingOverDistance.Evaluate(num) * __instance._blindingOverDot.Evaluate(Vector3.Dot(hub.PlayerCameraReference.forward, vector.normalized));
				bool allowed = false;
				if (num3 > __instance._minimalEffectDuration) allowed = true;
				var ev = new FlashedEvent(thrower, target, __instance.transform.position, allowed);
				Qurre.Events.Invoke.Player.Flashed(ev);
				if (ev.Allowed)
				{
					target.EnableEffect<Flashed>(num3, true);
					target.EnableEffect<Blinded>(num3 + __instance._additionalBlurDuration, true);
				}
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