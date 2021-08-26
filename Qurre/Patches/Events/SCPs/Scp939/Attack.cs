using HarmonyLib;
using Mirror;
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
}