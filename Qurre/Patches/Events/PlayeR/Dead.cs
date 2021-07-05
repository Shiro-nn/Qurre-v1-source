using HarmonyLib;
using Qurre.API.Events;
using UnityEngine;
using Qurre.API;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.HurtPlayer))]
    internal static class Dead
    {
        private static void Postfix(PlayerStats __instance, ref PlayerStats.HitInfo info, GameObject go)
        {
            try
            {
                Player attacker = Player.Get(__instance.gameObject);
                Player target = Player.Get(go);
                if ((target != null && (target.Role != RoleType.Spectator || target.GodMode || target.IsHost)) || attacker == null) return;
                var ev = new DeadEvent(Player.Get(__instance.gameObject), target, info);
                Qurre.Events.Invoke.Player.Dead(ev);
                info = ev.HitInfo;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Player [Dead]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}