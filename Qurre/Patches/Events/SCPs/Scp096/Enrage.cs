using HarmonyLib;
using Qurre.API.Events;
using scp096 = PlayableScps.Scp096;
namespace Qurre.Patches.Events.SCPs.Scp096
{
    [HarmonyPatch(typeof(scp096), nameof(scp096.Enrage))]
    internal static class Enrage
    {
        private static bool Prefix(scp096 __instance)
        {
            try
            {
                var ev = new EnrageEvent(__instance, API.Player.Get(__instance.Hub.gameObject));
                Qurre.Events.Invoke.Scp096.Enrage(ev);
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP096 [Enrage]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}