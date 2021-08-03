using HarmonyLib;
using Qurre.API.Events;
using Qurre.API.Objects;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(Radio), nameof(Radio.CallCmdUpdatePreset))]
    internal static class RadioUpdate
    {
        private static bool Prefix(Radio __instance, ref byte preset)
        {
            try
            {
                var ev = new RadioUpdateEvent(API.Player.Get(__instance.gameObject), __instance, (RadioStatus)preset);
                Qurre.Events.Invoke.Player.RadioUpdate(ev);
                preset = (byte)ev.ChangeTo;
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