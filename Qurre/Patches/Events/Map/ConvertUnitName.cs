using HarmonyLib;
using Qurre.API.Events;
using Respawning.NamingRules;
namespace Qurre.Patches.Events.Map
{
    [HarmonyPatch(typeof(NineTailedFoxNamingRule), nameof(NineTailedFoxNamingRule.GetCassieUnitName))]
    internal static class ConvertUnitName
    {
        private static bool Prefix(NineTailedFoxNamingRule __instance, string regular, ref string __result)
        {
            try
            {
                try
                {
                    string[] array = regular.Split('-');
                    __result = $"NATO_{array[0][0]} {array[1]}";
                }
                catch
                {
                    __result = regular;
                }
                var ev = new ConvertUnitNameEvent(__result);
                Qurre.Events.Invoke.Map.ConvertUnitName(ev);
                __result = ev.UnitName;
            }
            catch
            {
                Log.Error("Error, couldn't convert '" + regular + "' into a CASSIE-readable form.");
                __result = "ERROR";
            }
            return false;
        }
    }
}