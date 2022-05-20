using HarmonyLib;
using PlayerStatsSystem;
using Qurre.API;
using Qurre.API.Events;
using System;
namespace Qurre.Patches.Events.player
{
	[HarmonyPatch(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.ProcessDamage))]
	internal static class DamageProcess
	{
		public static bool Prefix(AttackerDamageHandler __instance, ReferenceHub ply)
		{
			try
			{
				RoleType curClass = ply.characterClassManager.CurClass;
				bool allowed = true;
				if (__instance.CheckSpawnProtection(__instance.Attacker.Hub, ply))
				{
					allowed = false;
				}
				var attacker = Player.Get(__instance.Attacker.Hub);
				if (attacker is null) attacker = API.Server.Host;
				if (ply.networkIdentity.netId == __instance.Attacker.NetId)
				{
					if (!__instance.AllowSelfDamage && !__instance.ForceFullFriendlyFire && !attacker.FriendlyFire)
					{
						allowed = false;
					}
					__instance.IsSuicide = true;
				}
				else if (__instance.ForceFullFriendlyFire && !__instance.AllowSelfDamage && !__instance.ForceFullFriendlyFire && !attacker.FriendlyFire)
				{
					allowed = false;
				}
				else if (!HitboxIdentity.CheckFriendlyFire(__instance.Attacker.Role, curClass, true) && !attacker.FriendlyFire)
				{
					if (AttackerDamageHandler._ffMultiplier > 0.1f) __instance.Damage *= AttackerDamageHandler._ffMultiplier;
					__instance.IsFriendlyFire = true;
				}
				var ev = new DamageProcessEvent(attacker, Player.Get(ply), __instance, __instance.Damage, __instance.IsFriendlyFire, allowed);
				Qurre.Events.Invoke.Player.DamageProcess(ev);
				if (ev.Amount == -1) ev.Amount = ev.Target.Hp + 1;
				__instance.Damage = ev.Amount;
				__instance.IsFriendlyFire = ev.FriendlyFire;
				if (!ev.Allowed) __instance.Damage = 0;
				if (!API.Server.FriendlyFire && ev.FriendlyFire) __instance.Damage = 0;
				return false;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching Player [DamageProcess]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}