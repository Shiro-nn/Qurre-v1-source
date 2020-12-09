﻿#pragma warning disable SA1118
using System;
using HarmonyLib;
using Mirror;
using Qurre.API.Events.Alpha;
using UnityEngine;
namespace Qurre.Patches.Events.Alpha
{
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.CancelDetonation), new Type[] { typeof(GameObject) })]
    internal static class Stopping
    {
        private static bool Prefix(AlphaWarheadController __instance, GameObject disabler)
        {
            try
            {
                if (!__instance.inProgress || __instance.timeToDetonation <= 10.0)
                    return false;
                if (__instance.timeToDetonation <= 15.0 && disabler != null)
                    __instance.GetComponent<PlayerStats>().TargetAchieve(disabler.GetComponent<NetworkIdentity>().connectionToClient, "thatwasclose");
                var ev = new StopEvent(ReferenceHub.GetHub(disabler) ?? API.Map.Host);
                Qurre.Events.Alpha.stopping(ev);
                return ev.IsAllowed && !API.Alpha.IsLocked;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Alpha.Stopping:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}