using System;
using HarmonyLib;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(AnimationController), nameof(AnimationController.CallCmdSyncData))]
    internal static class SyncData
    {
        private static bool Prefix(AnimationController __instance, byte state, Vector2 v2)
        {
            try
            {
                var pl = API.Player.Get(__instance.gameObject);
                if (pl == null) return true;
                var ev = new SyncDataEvent(pl, v2, state);
                Qurre.Events.Invoke.Player.SyncData(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [SyncData]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}