using CustomPlayerEffects;
using HarmonyLib;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
using System;
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
                if (player.playerEffectsController is null) return false;
                var pl = Player.Get(player);
                if (pl is null || pl.ClassManager is null) return false;
                var controller = __instance.GetSinkhole();
                if ((pl.Position - __instance.transform.position).sqrMagnitude <= __instance.DistanceToBeAffected * __instance.DistanceToBeAffected)
                {
                    var ev = new SinkholeWalkingEvent(pl, controller, controller.Effects, controller.EffectsDuration,
                        !((controller.ImmunityScps && pl.ClassManager.IsAnyScp()) || controller.ImmunityRoles.Contains(pl.Role)));
                    Qurre.Events.Invoke.Player.SinkholeWalking(ev);
                    if (!ev.Allowed) return false;
                    foreach (var ef in ev.Effects) pl.EnableEffect(ef, ev.Durations?[ef] ?? 1);
                }
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