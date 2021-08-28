using HarmonyLib;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
using Qurre.API.Objects;
using System;
using UnityEngine;
namespace Qurre.Patches.Events.SCPs.Scp939
{
	/* Scp:sl v11 beta 2.0
	[HarmonyPatch(typeof(PlayableScps.Scp939), nameof(PlayableScps.Scp939.ServerAttack))]
	internal static class Attack
	{
		internal static bool Prefix(PlayableScps.Scp939 __instance, PlayableScps.Messages.Scp939AttackMessage msg)
		{
			try
			{
				Player scp = Player.Get(__instance.Hub);
				if (msg.Victim != null && msg.Victim.TryGetComponent<BreakableWindow>(out var window))
				{
					__instance._currentBiteCooldown = 1f;
					scp.Connection.Send(new PlayableScps.Messages.ScpHitmarkerMessage(1.5f));
					NetworkServer.SendToAll(new PlayableScps.Messages.Scp939OnHitMessage(scp.Id));
					window.Damage(50f, null, new Footprinting.Footprint(scp.ReferenceHub), Vector3.zero);
					return false;
				}
				Player target = Player.Get(msg.Victim);
				if (target == null) return false;
				var ev = new ScpAttackEvent(scp, target, ScpAttackType.Scp939);
				Qurre.Events.Invoke.Player.ScpAttack(ev);
				if (!ev.Allowed) return false;
				__instance._currentBiteCooldown = 1f;
				scp.Connection.Send(new PlayableScps.Messages.ScpHitmarkerMessage(1.5f));
				NetworkServer.SendToAll(new PlayableScps.Messages.Scp939OnHitMessage(scp.Id));
				target.Damage(50, DamageTypes.Scp939, scp);
				scp.ClassManager.RpcPlaceBlood(target.Position, 0, 2f);
				target.EnableEffect(EffectType.Amnesia, 3f, true);
				return false;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching SCPs -> Scp939 [Attack]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
	*/
	//Scp:sl v11 beta 2.1
	[HarmonyPatch(typeof(Scp939PlayerScript), nameof(Scp939PlayerScript.UserCode_CmdShoot))]
	internal static class Attack
	{
		internal static bool Prefix(Scp939PlayerScript __instance, GameObject target)
		{
			try
			{
				Player scp = Player.Get(__instance._hub);
				if (target == null || !__instance.iAm939 || __instance.cooldown > 0f || Vector3.Distance(target.transform.position, __instance.transform.position) >= __instance.attackDistance * 1.2f)
					return false;
				Player _target = Player.Get(target);
				if (_target == null) return false;
				var ev = new ScpAttackEvent(scp, _target, ScpAttackType.Scp939);
				Qurre.Events.Invoke.Player.ScpAttack(ev);
				if (!ev.Allowed) return false;
				__instance.cooldown = 1f;
				_target.Damage(50, DamageTypes.Scp939, scp);
				scp.ClassManager.RpcPlaceBlood(_target.Position, 0, 2f);
				_target.EnableEffect(EffectType.Amnesia, 3f, true);
				__instance.RpcShoot();
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