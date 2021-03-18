#pragma warning disable SA1313
using HarmonyLib;
using Qurre.API.Events;
using Scp096 = PlayableScps.Scp096;
namespace Qurre.Patches.Events.SCPs.SCP096
{
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.Enrage))]
    internal static class Enrage
    {
        private static bool Prefix(Scp096 __instance)
        {
            try
            {
                var ev = new EnrageEvent(__instance, API.Player.Get(__instance.Hub.gameObject));
                Qurre.Events.SCPs.SCP096.enrage(ev);
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs.SCP096.Enrage:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}