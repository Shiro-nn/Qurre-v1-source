using HarmonyLib;
using Respawning;
using Respawning.NamingRules;
using UnityEngine;
using Qurre.API;
namespace Qurre.Patches.Modules
{
    [HarmonyPatch(typeof(NineTailedFoxNamingRule), nameof(NineTailedFoxNamingRule.GenerateNew))]
    internal static class UnitName
    {
        private static bool Prefix(NineTailedFoxNamingRule __instance, SpawnableTeamType type, out string regular)
        {
            if (!Round.UnitsToGenerate.TryGetValue(type, out var list) || list.Count == 0)
            {
                regular = "";
                return false;
            }
            do
            {
                regular = list[Random.Range(0, list.Count)] + "-" + Random.Range(0, Round.UnitMaxCode).ToString("00");
            }
            while (UnitNamingRule.UsedCombinations.Contains(regular));
            __instance.AddCombination(regular, type);
            return false;
        }
    }
}