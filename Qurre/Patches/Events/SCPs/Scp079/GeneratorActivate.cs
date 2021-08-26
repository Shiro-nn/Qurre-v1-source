using HarmonyLib;
using MapGeneration.Distributors;
using Qurre.API;
using Qurre.API.Events;
using static MapGeneration.Distributors.Scp079Generator;
namespace Qurre.Patches.Events.SCPs.Scp079
{
    [HarmonyPatch(typeof(Scp079Generator), nameof(Scp079Generator.ServerSetFlag))]
    internal static class GeneratorActivate
    {
        private static bool Prefix(Scp079Generator __instance, GeneratorFlags flag, bool state)
        {
            try
            {
                if (flag != GeneratorFlags.Engaged) return true;
                if (!state) return true;
                var ev = new GeneratorActivateEvent(__instance.GetGenerator());
                Qurre.Events.Invoke.Scp079.GeneratorActivate(ev);
                if (ev.Allowed) API.Round.ActiveGenerators++;
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP079 [ActivateGenerator]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}