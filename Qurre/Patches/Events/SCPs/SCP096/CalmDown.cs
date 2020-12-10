#pragma warning disable SA1313
using HarmonyLib;
using PlayableScps;
using static Qurre.API.Events.SCP096;
namespace Qurre.Patches.Events.SCPs.SCP096
{
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.EndEnrage))]
    internal static class CalmDown
    {
        private static bool Prefix(Scp096 __instance)
        {
            try
            {
                var ev = new CalmDownEvent(__instance, ReferenceHub.GetHub(__instance.Hub.gameObject));
                Qurre.Events.SCPs.SCP096.calmdown(ev);
                return ev.IsAllowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs.SCP096.CalmDown:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}