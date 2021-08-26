using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
using Qurre.API.Objects;
using System;
using UnityEngine;
namespace Qurre.Patches.Events.SCPs.Scp096
{
	[HarmonyPatch(typeof(PlayableScps.Scp096), nameof(PlayableScps.Scp096.ServerHitObject))]
	internal static class Attack
	{
		internal static bool Prefix(PlayableScps.Scp096 __instance, GameObject target, ref bool __result)
		{
			try
			{
				__result = false;
				if (target.TryGetComponent<BreakableWindow>(out var breakableWindow))
				{
					__result = breakableWindow.Damage(500f, null, new Footprinting.Footprint(__instance.Hub), target.transform.position);
					return false;
				}
				if (target.TryGetComponent<DoorVariant>(out var doorVariant) && doorVariant is IDamageableDoor damageableDoor && !doorVariant.IsConsideredOpen())
				{
					__result = damageableDoor.ServerDamage(250f, DoorDamageType.Scp096);
					return false;
				}
				if (!ReferenceHub.TryGetHub(target, out var hub) || hub == null || hub == __instance.Hub || hub.characterClassManager.IsAnyScp()) return false;
				Player scp = Player.Get(__instance.Hub);
				Player _target = Player.Get(hub);
				if (Physics.Linecast(_target.Position, scp.Position, PlayableScps.Scp096._solidObjectMask)) return false;
				if (Vector3.Distance(_target.Position, scp.Position) > 5f) return false;
				var ev = new ScpAttackEvent(scp, _target, ScpAttackType.Scp096);
				Qurre.Events.Invoke.Player.ScpAttack(ev);
				if (!ev.Allowed) return false;
				if (_target.Damage(9696, DamageTypes.Scp096, scp))
				{
					__instance._targets.Remove(hub);
					NetworkServer.SendToAll(default(PlayableScps.Messages.Scp096OnKillMessage), 0, false);
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