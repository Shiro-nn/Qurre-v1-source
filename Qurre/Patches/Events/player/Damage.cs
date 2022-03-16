using System;
using HarmonyLib;
using PlayerStatsSystem;
using Qurre.API;
using Qurre.API.Events;
using Qurre.API.Objects;
namespace Qurre.Patches.Events.player
{
	[HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.DealDamage))]
	internal static class Damage
	{
		public static bool Prefix(PlayerStats __instance, ref bool __result, ref DamageHandlerBase handler)
		{
			var _handler = handler;
			try
			{
				var attacker = handler.GetAttacker();
				Player target = Player.Get(__instance.gameObject);
				if (attacker == null) attacker = target;
				if (target == null || target.IsHost) return true;
				var type = handler.GetDamageType();
				if (type == DamageTypes.Recontainment && target.Role == RoleType.Scp079)
				{
					var eventArgs = new DeadEvent(null, target, handler, type);
					Qurre.Events.Invoke.Player.Dead(eventArgs);
					return true;
				}
				if (attacker == null || attacker.IsHost) return true;
				var doAmout = GetAmout();
				var ev = new DamageEvent(attacker, target, handler, type, doAmout);
				if (ev.Target.IsHost) return true;
				Qurre.Events.Invoke.Player.Damage(ev);
				handler = ev.DamageInfo;
				if (!ev.Allowed)
				{
					__result = false;
					return false;
				}
				if (!SetAmout(ev.Amount)) ev.Amount = doAmout;
				/*if (!ev.Target.GodMode && (ev.Amount == -1 || ev.Amount >= (ev.Target.Hp + ev.Target.Ahp)))
				{
					var dE = new DiesEvent(ev.Attacker, ev.Target, handler, type);
					Qurre.Events.Invoke.Player.Dies(dE);
					if (!dE.Allowed)
					{
						__result = false;
						return false;
					}
				}*/
				return true;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching Player [Damage]:\n{e}\n{e.StackTrace}");
				return true;
			}
			float GetAmout()
			{
				return _handler switch
				{
					StandardDamageHandler data => data.Damage,
					_ => -1,
				};
			}
			bool SetAmout(float amout)
			{
				if (_handler is StandardDamageHandler data) data.Damage = amout;
				else return false;
				return true;
			}
		}
	}
}