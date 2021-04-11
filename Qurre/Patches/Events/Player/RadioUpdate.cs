using HarmonyLib;
using Qurre.API.Events;
using Qurre.API.Objects;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(Radio), nameof(Radio.CallCmdUpdatePreset))]
    internal static class RadioUpdate
    {
        private static bool Prefix(Radio __instance, byte preset)
        {
            try
            {
                var ev = new RadioUpdateEvent(API.Player.Get(__instance.gameObject), (RadioStatus)preset);
                Qurre.Events.Player.radioUpdate(ev);
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