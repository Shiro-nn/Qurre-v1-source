using HarmonyLib;
using InventorySystem.Items.Radio;
using Mirror;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(RadioItem), nameof(RadioItem.Update))]
    internal static class UsingRadio
    {
        private static bool Prefix(RadioItem __instance)
        {
            try
            {
                if (!NetworkServer.active) return false;
                if (__instance.IsUsable)
                {
                    float num = (__instance._radio.UsingRadio ? (__instance.Ranges[__instance.CurRange].MinuteCostWhenTalking) : __instance.Ranges[__instance.CurRange].MinuteCostWhenIdle) / 60f;
                    var ev = new RadioUsingEvent(API.Player.Get(__instance._radio._hub), __instance, __instance._battery, Time.deltaTime * (num / 100f));
                    Qurre.Events.Invoke.Player.RadioUsing(ev);
                    if (!ev.Allowed) return false;
                    __instance._battery = Mathf.Clamp01(ev.Battery - ev.Consumption);
                    if (__instance._battery == 0f) __instance._radio.ForceDisableRadio();
                    if (Mathf.Abs(__instance._lastSentBatteryLevel - __instance.BatteryPercent) >= 1 && __instance.OwnerInventory.CurItem.TypeId == ItemType.Radio)
                        __instance.SendStatusMessage();
                }
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Player [UseRadio]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}