using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using PlayableScps.Messages;
using PlayerStatsSystem;
using Qurre.API.Events;
using Qurre.API.Objects;
using System;
using UnityEngine;
using Utils.Networking;
namespace Qurre.Patches.Events.SCPs.Scp096
{
	using Qurre.API;
	[HarmonyPatch(typeof(PlayableScps.Scp096), nameof(PlayableScps.Scp096.ServerHitObject))]
	internal static class Attack
	{
		internal static bool Prefix(PlayableScps.Scp096 __instance, GameObject target, ref bool __result)
		{
			try
			{
				__result = false;
				if (target.TryGetComponent(out BreakableWindow component))
				{
					__result = component.Damage(500f, new Scp096DamageHandler(__instance, 500f, Scp096DamageHandler.AttackType.Slap), target.transform.position);
					return false;
				}

				IDamageableDoor damageableDoor;
				if (target.TryGetComponent(out DoorVariant component2) && (damageableDoor = (component2 as IDamageableDoor)) != null && !component2.IsConsideredOpen())
				{
					__result = damageableDoor.ServerDamage(250f, DoorDamageType.Scp096);
					return false;
				}
				if (!ReferenceHub.TryGetHub(target, out ReferenceHub hub)) return false;
				if (hub is null || hub == __instance.Hub || hub.characterClassManager.IsAnyScp()) return false;
				if (Physics.Linecast(__instance.Hub.playerMovementSync.RealModelPosition, hub.playerMovementSync.RealModelPosition,
					PlayableScps.Scp096._solidObjectMask)) return false;
				if (Vector3.Distance(hub.playerMovementSync.RealModelPosition, hub.playerMovementSync.RealModelPosition) > 5f)
					return false;
				var ev = new ScpAttackEvent(Player.Get(__instance.Hub), Player.Get(hub), ScpAttackType.Scp096);
				Qurre.Events.Invoke.Player.ScpAttack(ev);
				if (!ev.Allowed) return false;
				if (hub.playerStats.DealDamage(new Scp096DamageHandler(__instance, 9696f, Scp096DamageHandler.AttackType.Slap)))
				{
					__instance._targets.Remove(hub);
					NetworkUtils.SendToAuthenticated(new Scp096OnKillMessage(__instance.Hub));
				}
				__result = true;
				return false;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching SCPs -> Scp096 [Attack]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}