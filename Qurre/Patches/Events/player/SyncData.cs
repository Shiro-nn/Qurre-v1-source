using System;
using HarmonyLib;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(AnimationController), nameof(AnimationController.RecieveData))]
    internal static class SyncData
    {
        private static bool Prefix(AnimationController __instance)
        {
            try
            {
                Vector3 vector = __instance._hub.transform.InverseTransformDirection(__instance._hub.playerMovementSync.PlayerVelocity);
                Vector2 movementSpeed = new Vector2(Mathf.Round(vector.z), Mathf.Round(vector.x));
                var pl = API.Player.Get(__instance._hub);
                if (pl == null) return true;
                var ev = new SyncDataEvent(pl, movementSpeed);
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