using HarmonyLib;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
using Qurre.API.Objects;
using System;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(TantrumEnvironmentalHazard), nameof(TantrumEnvironmentalHazard.DistanceChanged))]
    internal static class TantrumWalking
    {
        private static bool Prefix(TantrumEnvironmentalHazard __instance, ReferenceHub player)
        {
            try
            {
                if (!NetworkServer.active) return false;
                if (player == null || __instance.DisableEffect) return false;
                var pl = Player.Get(player);
                if (Vector3.Distance(player.transform.position, __instance.transform.position) <= __instance.DistanceToBeAffected)
                {
                    var ev = new TantrumWalkingEvent(pl, __instance);
                    Qurre.Events.Invoke.Player.TantrumWalking(ev);
                    if (!ev.Allowed) return false;
                    if (__instance.SCPImmune && (pl.ClassManager == null || pl.ClassManager.IsAnyScp())) return false;
                    pl.EnableEffect(EffectType.Stained, 1f);
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [TantrumWalking]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}