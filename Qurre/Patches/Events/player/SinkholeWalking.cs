using CustomPlayerEffects;
using HarmonyLib;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
using System;
using Extensions = Qurre.API.Extensions;

namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(SinkholeEnvironmentalHazard), nameof(SinkholeEnvironmentalHazard.DistanceChanged))]
    internal static class SinkholeWalking
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
                    var ev = new SinkholeWalkingEvent(pl, Extensions.GetSinkhole(__instance), Extensions.GetSinkhole(__instance).Effects, Extensions.GetSinkhole(__instance).EffectsDuration);
                    Qurre.Events.Invoke.Player.SinkholeWalking(ev);
                    if (ev.GiveEffects)
                    {
                        foreach (var Effect in ev.Effects)
                        {
                            ev.Player.EnableEffect(Effect, ev.Durations?[Effect] ?? 1);
                        }
                    }
                    return false;
                }
                pl.DisableEffect<SinkHole>();
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [SinkholeWalking]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}