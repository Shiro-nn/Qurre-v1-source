#pragma warning disable SA1118
using HarmonyLib;
using Qurre.API.Events;
using UnityEngine;
using Qurre.API;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.HurtPlayer))]
    internal static class Died
    {
        private static void Postfix(PlayerStats __instance, ref PlayerStats.HitInfo info, GameObject go)
        {
            try
            {
                ReferenceHub attacker = ReferenceHub.GetHub(__instance.gameObject);
                ReferenceHub target = ReferenceHub.GetHub(go);
                if ((target != null && (target.Role() != RoleType.Spectator || target.GodMode() || target.IsHost())) || attacker == null)
                    return;
                var ev = new DiedEvent(ReferenceHub.GetHub(__instance.gameObject), target, info);
                Qurre.Events.Player.died(ev);
                info = ev.HitInfo;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching PlayeR.Died:\n{e}\n{e.StackTrace}");
            }
        }
    }
}