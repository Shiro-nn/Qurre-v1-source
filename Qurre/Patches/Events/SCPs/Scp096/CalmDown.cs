using HarmonyLib;
using Qurre.API.Events;
using scp096 = PlayableScps.Scp096;
namespace Qurre.Patches.Events.SCPs.Scp096
{
    [HarmonyPatch(typeof(scp096), nameof(scp096.EndEnrage))]
    internal static class CalmDown
    {
        private static bool Prefix(scp096 __instance)
        {
            try
            {
                var ev = new CalmDownEvent(__instance, API.Player.Get(__instance.Hub.gameObject));
                Qurre.Events.Invoke.Scp096.CalmDown(ev);
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP096 [CalmDown]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}