using HarmonyLib;
using Qurre.API.Events;
using Qurre.API.Objects;
using System;
namespace Qurre.Patches.Events.Player
{
    using Qurre.API;
    [HarmonyPatch(typeof(SinkholeEnvironmentalHazard), nameof(SinkholeEnvironmentalHazard.OnEnter))]
    internal static class SinkholeEnter
    {
        private static bool Prefix(SinkholeEnvironmentalHazard __instance, ReferenceHub player)
        {
            try
            {
                var pl = Player.Get(player);
                if (pl is null) return true;
                var sink = __instance.GetSinkhole();
                var ev = new SinkholeWalkingEvent(pl, sink, HazardEventsType.Enter, sink.ImmunityScps || !player.characterClassManager.IsAnyScp());
                Qurre.Events.Invoke.Player.SinkholeWalking(ev);
                if (!ev.Allowed) return false;
                __instance.AffectedPlayers.Add(player);
                pl.EnableEffect(EffectType.SinkHole, 1f);
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [SinkholeEnter]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
    [HarmonyPatch(typeof(SinkholeEnvironmentalHazard), nameof(SinkholeEnvironmentalHazard.OnStay))]
    internal static class SinkholeStay
    {
        private static bool Prefix(SinkholeEnvironmentalHazard __instance, ReferenceHub player)
        {
            try
            {
                var pl = Player.Get(player);
                if (pl is null) return true;
                var sink = __instance.GetSinkhole();
                var ev = new SinkholeWalkingEvent(pl, sink, HazardEventsType.Stay, sink.ImmunityScps || !player.characterClassManager.IsAnyScp());
                Qurre.Events.Invoke.Player.SinkholeWalking(ev);
                if (!ev.Allowed) return false;
                pl.EnableEffect(EffectType.SinkHole, 1f);
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [SinkholeStay]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
    [HarmonyPatch(typeof(EnvironmentalHazard), nameof(EnvironmentalHazard.OnExit))]
    internal static class SinkholeExit
    {
        private static bool Prefix(EnvironmentalHazard __instance, ReferenceHub player)
        {
            try
            {
                if (__instance is not SinkholeEnvironmentalHazard ins) return true;
                var pl = Player.Get(player);
                if (pl is null) return true;
                var sink = ins.GetSinkhole();
                var ev = new SinkholeWalkingEvent(pl, sink, HazardEventsType.Exit, sink.ImmunityScps || !player.characterClassManager.IsAnyScp());
                Qurre.Events.Invoke.Player.SinkholeWalking(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [SinkholeExit]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}