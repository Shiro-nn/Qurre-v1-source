#pragma warning disable SA1313
using System;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using UnityEngine;
using static Qurre.API.Events.SCP079;

namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.HurtPlayer))]
    internal static class Damage
    {
        private static bool Prefix(PlayerStats __instance, ref PlayerStats.HitInfo info, GameObject go)
        {
            try
            {
                if (go == null)
                    return true;
                ReferenceHub attacker = ReferenceHub.GetHub(info.IsPlayer ? info.RHub.gameObject : __instance.gameObject);
                ReferenceHub target = ReferenceHub.GetHub(go);
                if (target == null || target.IsHost())
                    return true;
                if (info.GetDamageType() == DamageTypes.Recontainment && target.GetRole() == RoleType.Scp079)
                {
                    Qurre.Events.SCPs.SCP079.recontain(new RecontainEvent(target));
                    var eventArgs = new DiedEvent(null, target, info);
                    Qurre.Events.Player.died(eventArgs);
                }
                if (attacker == null || attacker.IsHost())
                    return true;
                var ev = new DamageEvent(attacker, target, info);
                if (ev.Target.IsHost())
                    return true;
                Qurre.Events.Player.damage(ev);
                info = ev.HitInformations;
                if (!ev.IsAllowed)
                    return false;
                if (!ev.Target.GetGodMode() && (ev.Amount == -1 || ev.Amount >= ev.Target.GetHP() + ev.Target.GetMaxAHP()))
                {
                    var dE = new DyingEvent(ev.Attacker, ev.Target, ev.HitInformations);
                    Qurre.Events.Player.dying(dE);
                    if (!dE.IsAllowed)
                        return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching PlayeR.Damage:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}