using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using Qurre.API.Objects;
using System;
using UnityEngine;
namespace Qurre.Patches.Events.SCPs.Scp049
{
    [HarmonyPatch(typeof(Scp049_2PlayerScript), nameof(Scp049_2PlayerScript.UserCode_CmdHurtPlayer))]
    internal static class AttackZombie
    {
        internal static bool Prefix(Scp049_2PlayerScript __instance, GameObject plyObj)
        {
            try
			{
				if (!__instance._iawRateLimit.CanExecute(true) || plyObj == null) return false;
                Player scp = Player.Get(__instance._hub);
                Player target = Player.Get(plyObj);
				if (target == null) return false;
                if (Vector3.Distance(scp.Position, target.Position) > __instance.distance * 1.5f || !__instance.iAm049_2) return false;
                var ev = new ScpAttackEvent(scp, target, ScpAttackType.Scp0492);
                Qurre.Events.Invoke.Player.ScpAttack(ev);
                if (!ev.Allowed) return false;
                target.Damage((int)__instance.damage, DamageTypes.Scp0492, scp);
                __instance.TargetHitMarker(scp.Connection);
                scp.ClassManager.RpcPlaceBlood(target.Position, 0, (target.Role == RoleType.Spectator) ? 1.3f : 0.5f);
				return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP049 [Zombie Attack]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}