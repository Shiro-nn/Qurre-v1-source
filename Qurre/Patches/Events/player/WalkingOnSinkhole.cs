using CustomPlayerEffects;
using HarmonyLib;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
using System;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(SinkholeEnvironmentalHazard), nameof(SinkholeEnvironmentalHazard.DistanceChanged))]
    internal static class WalkingOnSinkhole
    {
        private static bool Prefix(SinkholeEnvironmentalHazard __instance, ReferenceHub player)
        {
            try
            {
                if (!NetworkServer.active) return false;
                if (player.playerEffectsController == null) return false;
                var pl = Player.Get(player);
                if ((pl.Position - __instance.transform.position).sqrMagnitude <= __instance.DistanceToBeAffected * __instance.DistanceToBeAffected)
                {
                    var ev = new WalkingOnSinkholeEvent(pl, __instance);
                    Qurre.Events.Invoke.Player.WalkingOnSinkhole(ev);
                    if (!ev.Allowed) return false;
                    if (__instance.SCPImmune && (pl.ClassManager == null || pl.ClassManager.IsAnyScp())) return false;
                    pl.EnableEffect<SinkHole>();
                    return false;
                }
                pl.DisableEffect<SinkHole>();
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [WalkingOnSinkhole]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}