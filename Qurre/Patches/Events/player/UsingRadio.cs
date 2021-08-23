using HarmonyLib;
using InventorySystem.Items.Radio;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(RadioItem), nameof(RadioItem.Update))]
    internal static class UsingRadio
    {
        private static bool Prefix(RadioItem __instance)
        {
            try
            {
                if (__instance._enabled && __instance.OwnerInventory.CurItem.TypeId == ItemType.Radio)
                {
                    var ev = new RadioUsingEvent(API.Player.Get(__instance._radio._hub), __instance, __instance.BatteryPercent);
                    Qurre.Events.Invoke.Player.RadioUsing(ev);
                    if (!ev.Allowed) return false;
                    __instance.BatteryPercent = ev.Battery;
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