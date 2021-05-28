using System;
using GameCore;
using HarmonyLib;
using Qurre.API.Events;
using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(Handcuffs), nameof(Handcuffs.CallCmdCuffTarget))]
    internal static class Cuff
    {
        private static bool Prefix(Handcuffs __instance, GameObject target)
        {
            try
            {
                if (!__instance.RateLimit().CanExecute() || target == null ||
                    Vector3.Distance(target.transform.position, __instance.transform.position) >
                    __instance.Handcuffs_raycastDistance() * 1.10000002384186)
                    return false;
                Handcuffs handcuffs = ReferenceHub.GetHub(target).handcuffs;
                if (handcuffs == null || __instance.MyReferenceHub.inventory.curItem != ItemType.Disarmer ||
                    (__instance.MyReferenceHub.characterClassManager.CurClass < RoleType.Scp173 ||
                     handcuffs.CufferId >= 0) || handcuffs.MyReferenceHub.inventory.curItem != ItemType.None)
                    return false;
                Team team1 = __instance.MyReferenceHub.characterClassManager.Classes
                    .SafeGet(__instance.MyReferenceHub.characterClassManager.CurClass).team;
                Team team2 = __instance.MyReferenceHub.characterClassManager.Classes
                    .SafeGet(handcuffs.MyReferenceHub.characterClassManager.CurClass).team;
                bool flag = false;
                switch (team1)
                {
                    case Team.MTF:
                        if (team2 == Team.CHI || team2 == Team.CDP)
                            flag = true;
                        if (team2 == Team.RSC && ConfigFile.ServerConfig.GetBool("mtf_can_cuff_researchers"))
                            flag = true;
                        break;
                    case Team.CHI:
                        if (team2 == Team.MTF || team2 == Team.RSC)
                            flag = true;
                        if (team2 == Team.CDP && ConfigFile.ServerConfig.GetBool("ci_can_cuff_class_d"))
                            flag = true;
                        break;
                    case Team.RSC:
                        if (team2 == Team.CHI || team2 == Team.CDP)
                            flag = true;
                        break;
                    case Team.CDP:
                        if (team2 == Team.MTF || team2 == Team.RSC)
                            flag = true;
                        break;
                }
                if (!flag) return false;
                __instance.ClearTarget();
                var ev = new CuffEvent(API.Player.Get(__instance.gameObject), API.Player.Get(target));
                Qurre.Events.Player.cuff(ev);
                if (!ev.Allowed) return false;
                handcuffs.NetworkCufferId = __instance.MyReferenceHub.queryProcessor.PlayerId;
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [Cuff]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}