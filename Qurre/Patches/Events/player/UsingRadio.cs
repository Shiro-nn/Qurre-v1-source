using HarmonyLib;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(Radio), nameof(Radio.UseBattery))]
    internal static class UsingRadio
    {
        private static bool Prefix(Radio __instance)
        {
            try
            {
                if (__instance.CheckRadio() && __instance.inv.items[__instance.myRadio].id == ItemType.Radio)
                {
                    float num = __instance.inv.items[__instance.myRadio].durability - 1.67f * (1f / __instance.presets[__instance.curPreset].powerTime) * (__instance.isTransmitting ? 3 : 1);
                    var ev = new RadioUsingEvent(API.Player.Get(__instance.gameObject), __instance, num);
                    Qurre.Events.Invoke.Player.RadioUsing(ev);
                    if (!ev.Allowed) return false;
                    num = ev.Battery;
                    if (num > -1f && num < 101f) __instance.inv.items.ModifyDuration(__instance.myRadio, num);
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