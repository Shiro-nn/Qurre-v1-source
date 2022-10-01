using HarmonyLib;
using Qurre.API.Events;
using Qurre.API.Objects;
using System;
namespace Qurre.Patches.Events.SCPs.Scp173
{
	using Qurre.API;
	[HarmonyPatch(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173.ServerKillPlayer))]
	internal static class Attack
	{
		internal static bool Prefix(PlayableScps.Scp173 __instance, ReferenceHub target)
		{
			try
			{
				Player scp = Player.Get(__instance.Hub);
				Player _target = Player.Get(target);
				if (target == __instance.Hub || _target.ClassManager.IsAnyScp() || _target.Role == RoleType.Spectator) return false;
				var ev = new ScpAttackEvent(scp, _target, ScpAttackType.Scp173);
				Qurre.Events.Invoke.Player.ScpAttack(ev);
				return ev.Allowed;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching SCPs -> Scp173 [Attack]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}