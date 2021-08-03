using Grenades;
using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.Events;
using Qurre.API.Objects;
using System.Collections.Generic;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
	[HarmonyPatch(typeof(FragGrenade), nameof(FragGrenade.ServersideExplosion))]
	internal static class FragExplosionPatch
	{
		private static bool Prefix(FragGrenade __instance, ref bool __result)
		{
			Player thrower = Player.Get(__instance.thrower.gameObject);
			Vector3 position = __instance.transform.position;
			int num = 0;
			foreach (Collider collider in Physics.OverlapSphere(position, __instance.chainTriggerRadius, __instance.damageLayerMask))
			{
				BreakableWindow component = collider.GetComponent<BreakableWindow>();
				if (component != null)
				{
					if ((component.transform.position - position).sqrMagnitude <= __instance.sqrChainTriggerRadius) component.ServerDamageWindow(500f);
				}
				else
				{
					Door componentInParent = collider.GetComponentInParent<DoorVariant>().GetDoor();
					if (componentInParent.DoorVariant is IDamageableDoor damageableDoor)
					{
						damageableDoor.ServerDamage(__instance.damageOverDistance.Evaluate(Vector3.Distance(position, componentInParent.Position)), DoorDamageType.Grenade);
					}
					else if ((__instance.chainLengthLimit == -1 || __instance.chainLengthLimit > __instance.currentChainLength) &&
						(__instance.chainConcurrencyLimit == -1 || __instance.chainConcurrencyLimit > num))
					{
						Pickup componentInChildren = collider.GetComponentInChildren<Pickup>();
						if (componentInChildren != null && __instance.ChangeIntoGrenade(componentInChildren)) num++;
					}
				}
			}
			List<Player> targets = new List<Player>();
			foreach (Player pl in Player.List)
			{
				if (ServerConsole.FriendlyFire || pl == thrower || (pl.WeaponManager.GetShootPermission(__instance.throwerTeam, false) &&
					pl.WeaponManager.GetShootPermission(__instance.TeamWhenThrown, false)))
				{
					PlayerStats playerStats = pl.PlayerStats;
					if (playerStats != null && playerStats.ccm.InWorld)
					{
						float num2 = __instance.damageOverDistance.Evaluate(Vector3.Distance(position, playerStats.transform.position)) * (playerStats.ccm.IsHuman() ?
							GameCore.ConfigFile.ServerConfig.GetFloat("human_grenade_multiplier", 0.7f) : GameCore.ConfigFile.ServerConfig.GetFloat("scp_grenade_multiplier", 1f));
						if (num2 > __instance.absoluteDamageFalloff)
						{
							foreach (Transform transform in playerStats.grenadePoints)
							{
								if (!Physics.Linecast(position, transform.position, __instance.hurtLayerMask))
								{
									targets.Add(pl);
									break;
								}
							}
						}
					}
				}
			}
			var ev = new FragExplosionEvent(thrower, targets, position);
			Qurre.Events.Invoke.Player.FragExplosion(ev);
			if (!ev.Allowed)
			{
				__result = false;
				return false;
			}
			foreach (Player pl in targets)
			{
				float num2 = __instance.damageOverDistance.Evaluate(Vector3.Distance(position, pl.Position)) * (pl.ClassManager.IsHuman() ?
					GameCore.ConfigFile.ServerConfig.GetFloat("human_grenade_multiplier", 0.7f) : GameCore.ConfigFile.ServerConfig.GetFloat("scp_grenade_multiplier", 1f));
				pl.Damage(new PlayerStats.HitInfo(num2,
					(__instance.thrower != null) ? __instance.thrower.hub.LoggedNameFromRefHub() : "(UNKNOWN)", DamageTypes.Grenade, __instance.thrower.hub.queryProcessor.PlayerId));
				if (!pl.ClassManager.IsAnyScp())
				{
					float duration = __instance.statusDurationOverDistance.Evaluate(Vector3.Distance(position, pl.Position));
					pl.EnableEffect(EffectType.Burned, duration, false);
					pl.EnableEffect(EffectType.Concussed, duration, false);
				}

			}
			return false;
		}
	}
}