using System;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.HurtPlayer))]
    internal static class Damage
	{
		public static bool Prefix(PlayerStats __instance, ref PlayerStats.HitInfo info, GameObject go)
		{
			try
			{
				if (go == null) return true;
				Player attacker = Player.Get(info.IsPlayer ? info.RHub.gameObject : __instance.gameObject);
				Player target = Player.Get(go);
				if (target == null || target.IsHost) return true;
				if (info.GetDamageType() == DamageTypes.Recontainment && target.Role == RoleType.Scp079)
				{
					var eventArgs = new DeadEvent(null, target, info);
					Qurre.Events.Player.dead(eventArgs);
				}
				if (attacker == null || attacker.IsHost) return true;
				var ev = new DamageEvent(attacker, target, info);
				if (ev.Target.IsHost) return true;
				Qurre.Events.Player.damage(ev);
				info = ev.HitInformations;
				if (!ev.Allowed) return false;
				if (!ev.Target.GodMode && (ev.Amount == -1 || ev.Amount >= (ev.Target.HP + ev.Target.AHP)))
				{
					var dE = new DiesEvent(ev.Attacker, ev.Target, ev.HitInformations);
					Qurre.Events.Player.dies(dE);
					if (!dE.Allowed) return false;
				}
				return true;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching Player [Damage]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
    }
}