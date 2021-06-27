using HarmonyLib;
using PlayableScps;
using Qurre.API.Events;
namespace Qurre.Patches.Events.SCPs.SCP096
{
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.EndEnrage))]
    internal static class CalmDown
    {
        private static bool Prefix(Scp096 __instance)
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