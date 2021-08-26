using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using Qurre.API.Objects;
using System;
using UnityEngine;
namespace Qurre.Patches.Events.SCPs.Scp049
{
    [HarmonyPatch(typeof(PlayableScps.Scp049), nameof(PlayableScps.Scp049.BodyCmd_ByteAndGameObject))]
    internal static class Attack
    {
        internal static bool Prefix(PlayableScps.Scp049 __instance, byte num, GameObject go)
        {
            if (num != 0) return true;
            try
            {
                if (!__instance._interactRateLimit.CanExecute(true)) return false;
                if (go == null || __instance.RemainingServerKillCooldown > 0f) return false;
                Player scp = Player.Get(__instance.Hub);
                Player target = Player.Get(go);
                if (Vector3.Distance(target.Position, scp.Position) >= PlayableScps.Scp049.AttackDistance * 1.25f) return false;
                if (Physics.Linecast(scp.Position, target.Position, InventorySystem.Items.MicroHID.MicroHIDItem.WallMask)) return false;
                var ev = new ScpAttackEvent(scp, target, ScpAttackType.Scp049);
                Qurre.Events.Invoke.Player.ScpAttack(ev);
                if (!ev.Allowed) return false;
                target.Damage(4949, DamageTypes.Scp049, scp);
                GameCore.Console.AddDebugLog("SCPCTRL", "SCP-049 | Sent 'death time' RPC", MessageImportance.LessImportant, false);
                __instance.Hub.scpsController.RpcTransmit_Byte(0);
                __instance.RemainingServerKillCooldown = PlayableScps.Scp049.KillCooldown;
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP049 [Attack]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}