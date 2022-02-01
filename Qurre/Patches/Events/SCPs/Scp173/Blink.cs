using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Qurre.Patches.Events.SCPs.Scp173
{
    [HarmonyPatch(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173.ServerHandleBlinkMessage))]
    internal static class Blink
    {
        private static bool Prefix(PlayableScps.Scp173 __instance, ref Vector3 blinkPos, List<Player> targets)
        {
            try
            {
                var ev = new BlinkEvent(Player.Get(__instance.Hub), blinkPos, targets);
                Qurre.Events.Invoke.Scp173.Blink(ev);
                targets = ev.Targets;
                blinkPos = ev.Position;
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> Scp173 [Blink]:\n{e}\n{e.StackTrace}");
            }
            return false;
        }
    }
}