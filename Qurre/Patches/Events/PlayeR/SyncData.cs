#pragma warning disable SA1313
using System;
using HarmonyLib;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(AnimationController), nameof(AnimationController.CallCmdSyncData))]
    internal static class SyncData
    {
        private static bool Prefix(AnimationController __instance, byte state, Vector2 v2)
        {
            try
            {
                var ev = new SyncDataEvent(API.Player.Get(__instance.gameObject), v2, state);
                Qurre.Events.Player.syncData(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching PlayeR.SyncData:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}