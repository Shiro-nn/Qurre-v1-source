using Footprinting;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using Qurre.API.Objects;
using System;
using UnityEngine;
namespace Qurre.Patches.Events.SCPs.Scp939
{
	[HarmonyPatch(typeof(PlayableScps.Scp939), nameof(PlayableScps.Scp939.ServerAttack))]
	internal static class Attack
	{
		internal static bool Prefix(PlayableScps.Scp939 __instance, ref bool __result, GameObject target)
		{
			try
			{
				Player scp = Player.Get(__instance.Hub);
				__result = false;
				if (ReferenceHub.TryGetHub(target, out ReferenceHub hub))
				{
					if (hub.characterClassManager.IsAnyScp()) return false;
					var pl = Player.Get(hub);
					var ev = new ScpAttackEvent(scp, pl, ScpAttackType.Scp939);
					Qurre.Events.Invoke.Player.ScpAttack(ev);
					if (!ev.Allowed) return false;
					pl.Damage(50, DamageTypes.Scp939, scp);
					scp.ClassManager.RpcPlaceBlood(target.transform.position, 0, 2f);
					pl.EnableEffect(EffectType.Amnesia, 3f, true);
					__result = true;
					return false;
				}
				if (!target.TryGetComponent(out BreakableWindow component)) return false;
				component.Damage(50f, null, new Footprint(__instance.Hub), Vector3.zero);
				__result = true;
				return false;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching SCPs -> Scp939 [Attack]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}