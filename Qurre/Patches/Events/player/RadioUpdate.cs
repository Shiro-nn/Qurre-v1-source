using HarmonyLib;
using InventorySystem.Items.Radio;
using Qurre.API.Events;
using Qurre.API.Objects;
namespace Qurre.Patches.Events.Player
{
    [HarmonyPatch(typeof(RadioItem), nameof(RadioItem.ServerProcessCmd))]
    internal static class RadioUpdate
    {
        private static bool Prefix(RadioItem __instance, RadioMessages.RadioCommand command)
        {
            try
            {
                if (__instance == null || __instance.gameObject == null) return true;
                bool enabled = __instance._enabled;
                byte rangeId = __instance._rangeId;
                switch (command)
                {
                    case RadioMessages.RadioCommand.Enable:
                        enabled = true;
                        break;
                    case RadioMessages.RadioCommand.Disable:
                        enabled = false;
                        break;
                    case RadioMessages.RadioCommand.ChangeRange:
                        rangeId += 1;
                        if (rangeId >= __instance.Ranges.Length) rangeId = 0;
                        break;
                }
                rangeId--;
                var ev = new RadioUpdateEvent(API.Player.Get(__instance.gameObject), __instance, (RadioStatus)rangeId, enabled);
                Qurre.Events.Invoke.Player.RadioUpdate(ev);
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Player [RadioUpdate]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}