using HarmonyLib;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
using Qurre.API.Objects;
using System;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(TantrumEnvironmentalHazard), nameof(TantrumEnvironmentalHazard.OnEnter))]
    internal static class TantrumEnter
    {
        private static bool Prefix(TantrumEnvironmentalHazard __instance, ReferenceHub player)
        {
            try
            {
                if (player == null || __instance.DisableEffect || __instance._correctPosition == null ||
                    player.characterClassManager == null) return false;
                var pl = Player.Get(player);
                var ev = new TantrumWalkingEvent(pl, __instance, HazardEventsType.Enter, !player.characterClassManager.IsAnyScp());
                Qurre.Events.Invoke.Player.TantrumWalking(ev);
                if (!ev.Allowed) return false;
                __instance.AffectedPlayers.Add(player);
                pl.EnableEffect(EffectType.Stained);
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [TantrumWalking]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
    [HarmonyPatch(typeof(TantrumEnvironmentalHazard), nameof(TantrumEnvironmentalHazard.OnStay))]
    internal static class TantrumStay
    {
        private static bool Prefix(TantrumEnvironmentalHazard __instance, ReferenceHub player)
        {
            try
            {
                var pl = Player.Get(player);
                if (pl is null) return true;
                var ev = new TantrumWalkingEvent(pl, __instance, HazardEventsType.Stay, !player.characterClassManager.IsAnyScp());
                Qurre.Events.Invoke.Player.TantrumWalking(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [TantrumWalking]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
    [HarmonyPatch(typeof(TantrumEnvironmentalHazard), nameof(TantrumEnvironmentalHazard.OnExit))]
    internal static class TantrumExit
    {
        private static bool Prefix(TantrumEnvironmentalHazard __instance, ReferenceHub player)
        {
            try
            {
                if (player == null || __instance.DisableEffect || __instance._correctPosition == null ||
                    player.characterClassManager == null) return false;
                var pl = Player.Get(player);
                var ev = new TantrumWalkingEvent(pl, __instance, HazardEventsType.Exit, !player.characterClassManager.IsAnyScp());
                Qurre.Events.Invoke.Player.TantrumWalking(ev);
                if (!ev.Allowed) return false;
                __instance.AffectedPlayers.Remove(player);
                pl.EnableEffect(EffectType.Stained, 2f);
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