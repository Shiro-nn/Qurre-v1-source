#pragma warning disable SA1313
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.SCPs.SCP079
{
    [HarmonyPatch(typeof(Generator079), nameof(Generator079.CheckFinish))]
    internal static class GeneratorActivate
    {
        private static bool Prefix(Generator079 __instance)
        {
            try
            {
                if (__instance.Generator_prevFinish() || __instance.Generator_localTime() > 0.0)
                    return false;
                var ev = new GeneratorActivateEvent(__instance.GetGenerator());
                Qurre.Events.SCPs.SCP079.generatoractivate(ev);
                __instance.Generator_prevFinish(true);
                __instance.epsenRenderer.sharedMaterial = __instance.matLetGreen;
                __instance.epsdisRenderer.sharedMaterial = __instance.matLedBlack;
                __instance.Generator_asource().PlayOneShot(__instance.unlockSound);
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs.SCP079.ActivateGenerator:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}