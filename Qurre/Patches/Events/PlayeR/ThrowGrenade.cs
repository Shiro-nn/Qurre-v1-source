#pragma warning disable SA1313
using System;
using Grenades;
using HarmonyLib;
using Qurre.API.Events;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(GrenadeManager), nameof(GrenadeManager.CallCmdThrowGrenade))]
    internal static class ThrowGrenade
    {
        private static bool Prefix(ref GrenadeManager __instance, ref int id, ref bool slowThrow, ref double time)
        {
            try
            {
                var ev = new ThrowGrenadeEvent(API.Player.Get(__instance.gameObject), __instance, id, slowThrow, time);
                Qurre.Events.Player.throwGrenade(ev);
                id = ev.Id;
                slowThrow = ev.IsSlow;
                time = ev.FuseTime;
                return ev.IsAllowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching PlayeR.ThrowGrenade:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}