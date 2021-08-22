using HarmonyLib;
using PlayableScps;
using Qurre.API;
using UnityEngine;
namespace Qurre.Patches.Controllers
{
    [HarmonyPatch(typeof(Scp173), nameof(Scp173.UpdateObservers))]
    internal static class Scp173Controller
    {
        private static void Prefix(Scp173 __instance, out int __state) => __state = __instance._observingPlayers.Count;
        private static void Postfix(Scp173 __instance, int __state)
        {
            var peanut = Player.Get(__instance.Hub);
            foreach (var ply in __instance._observingPlayers)
            {
                var player = Player.Get(ply.gameObject);
                var flag = false;
                if (player.Invisible || player.Team == Team.SCP) flag = true;
                if (peanut.Scp173Controller.IgnoredPlayers.Contains(player)) flag = true;
                if (flag)
                {
                    __instance._observingPlayers.Remove(player.ReferenceHub);
                    __instance._isObserved = __instance._observingPlayers.Count > 0;
                    if (__state != __instance._observingPlayers.Count && __instance._blinkCooldownRemaining > 0f)
                    {
                        __instance._blinkCooldownRemaining = Mathf.Max(0f, __instance._blinkCooldownRemaining + (__instance._observingPlayers.Count - __state));
                        if (__instance._blinkCooldownRemaining <= 0f)
                            __instance.BlinkReady = true;
                    }
                }
            }
        }
    }
}