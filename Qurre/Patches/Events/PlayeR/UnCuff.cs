using System;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(Handcuffs), nameof(Handcuffs.ClearTarget))]
    internal static class UnCuff
    {
        private static bool Prefix(Handcuffs __instance)
        {
            try
            {
                foreach (Player target in Player.List)
                {
                    if (target == null) continue;
                    if (target.CufferId == __instance.MyReferenceHub.queryProcessor.PlayerId)
                    {
                        var ev = new UnCuffEvent(Player.Get(__instance.gameObject), target);
                        Qurre.Events.Invoke.Player.UnCuff(ev);
                        if (ev.Allowed) target.CufferId = -1;
                        break;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [UnCuff]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
    [HarmonyPatch(typeof(Handcuffs), nameof(Handcuffs.CallCmdFreeTeammate))]
    internal static class UnCuffTeam
    {
        private static bool Prefix(Handcuffs __instance, GameObject target)
        {
            try
            {
                if (!__instance.RateLimit().CanExecute(true) || target == null ||
                    (Vector3.Distance(target.transform.position, __instance.transform.position) >
                        __instance.Handcuffs_raycastDistance() * 1.10000002384186 || __instance.MyReferenceHub.characterClassManager
                            .Classes.SafeGet(__instance.MyReferenceHub.characterClassManager.CurClass).team ==
                        Team.SCP))
                    return false;
                var p = Player.Get(target);
                var ev = new UnCuffEvent(Player.Get(__instance.gameObject), p); ;
                Qurre.Events.Invoke.Player.UnCuff(ev);
                if (!ev.Allowed) return false;
                p.CufferId = -1;
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [UnCuffTeam]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}