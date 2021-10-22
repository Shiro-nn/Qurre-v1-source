using HarmonyLib;
using System;
using UnityEngine;
namespace Qurre.Patches.Modules
{
    [HarmonyPatch(typeof(PlayerMovementSync), nameof(PlayerMovementSync.OverridePosition))]
    internal static class Bot_Position
    {
        private static bool Prefix(PlayerMovementSync __instance, Vector3 pos, float rot, bool forceGround = false)
        {
            bool error_send = true;
            try
            {
                try { _ = __instance.transform.localScale; } catch { error_send = false; }
                if (forceGround && Physics.Raycast(pos, Vector3.down, out var raycastHit, 100f, __instance.CollidableSurfaces))
                    pos = raycastHit.point + Vector3.up * 1.23f * __instance.transform.localScale.y;
                __instance.ForcePosition(pos);
                __instance.TargetSetRotation(__instance.connectionToClient, rot);
                return false;
            }
            catch (Exception e)
            {
                if (error_send) Log.Error($"umm, error in patching Modules [Bot Position]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}