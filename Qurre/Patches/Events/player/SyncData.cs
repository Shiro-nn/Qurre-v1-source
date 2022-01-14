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
            bool send_error = true;
            try
            {
                if (__instance._hub is null) return true;
                var pl = API.Player.Get(__instance._hub);
                try { _ = pl.GameObject.transform; _ = pl.Transform; _ = pl.Position; } catch { send_error = false; }
                Vector3 vector = __instance._hub.transform.InverseTransformDirection(__instance._hub.playerMovementSync.PlayerVelocity);
                Vector2 movementSpeed = new(Mathf.Round(vector.z), Mathf.Round(vector.x));
                if (pl is null) return true;
                var ev = new SyncDataEvent(pl, movementSpeed);
                Qurre.Events.Invoke.Player.SyncData(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                if (send_error) Log.Error($"umm, error in patching Player [SyncData]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}