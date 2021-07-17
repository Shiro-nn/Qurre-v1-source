using Grenades;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using Qurre.API.Objects;
using QurreModLoader;
using System;
using System.Collections.Generic;
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
				List<Player> targets = new List<Player>();
				Vector3 position = __instance.transform.position;
				foreach (Player pl in Player.List)
				{
					CustomPlayerEffects.Flashed effect = pl.GetEffect<CustomPlayerEffects.Flashed>();
					CustomPlayerEffects.Deafened effect2 = pl.GetEffect<CustomPlayerEffects.Deafened>();
					if (effect != null && __instance.thrower != null && (__instance._friendlyFlash ||
						effect.Flashable(ReferenceHub.GetHub(__instance.thrower.gameObject), position, __instance._ignoredLayers)))
					{
						float num = __instance.powerOverDistance.Evaluate(Vector3.Distance(pl.Position, position) / ((position.y > 900f) ?
							__instance.distanceMultiplierSurface : __instance.distanceMultiplierFacility)) *
							__instance.powerOverDot.Evaluate(Vector3.Dot(pl.CameraTransform.forward, (pl.CameraTransform.position - position).normalized));
						byte b = (byte)Mathf.Clamp(Mathf.RoundToInt(num * 10f * __instance.maximumDuration), 1, 255);
						if (b >= effect.Intensity && num > 0f)
						{
							targets.Add(pl);
						}
					}
				}
				var ev = new FlashExplosionEvent(thrower, targets, position);
				Qurre.Events.Invoke.Player.FlashExplosion(ev);
				if (!ev.Allowed)
				{
					__result = false;
					return false;
				}
				foreach (Player pl in targets)
				{
					float num = __instance.powerOverDistance.Evaluate(Vector3.Distance(pl.Position, position) / ((position.y > 900f) ?
						__instance.distanceMultiplierSurface : __instance.distanceMultiplierFacility)) *
						__instance.powerOverDot.Evaluate(Vector3.Dot(pl.CameraTransform.forward, (pl.CameraTransform.position - position).normalized));
					byte b = (byte)Mathf.Clamp(Mathf.RoundToInt(num * 10f * __instance.maximumDuration), 1, 255);
					if (b >= pl.GetEffect(EffectType.Flashed).Intensity)
					{
						pl.ChangeEffectIntensity<CustomPlayerEffects.Flashed>(b);
						pl.EnableEffect(EffectType.Deafened, num * __instance.maximumDuration, true);
					}
				}
				if (__instance.serverGrenadeEffect != null)
				{
					Transform transform = __instance.transform;
					UnityEngine.Object.Instantiate(__instance.serverGrenadeEffect, transform.position, transform.rotation);
				}
				ServerLogs.AddLog(ServerLogs.Modules.Logger, $"Player {__instance._throwerName}'s {__instance.logName} grenade exploded", ServerLogs.ServerLogType.GameEvent, false);
				__result = true;
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