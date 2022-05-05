using HarmonyLib;
using Qurre.API;
using Respawning;
using Respawning.NamingRules;
using UnityEngine;

namespace Qurre.Patches.Modules
{
    [HarmonyPatch(typeof(NineTailedFoxNamingRule), "GenerateNew")]
    internal static class UnitName
    {
        private static bool Prefix(NineTailedFoxNamingRule __instance, SpawnableTeamType type, out string regular)
        {
            if (!Round.UnitsToGenerate.TryFind(out API.Addons.UnitGenerator list, _type => _type.Team == type) || list.Units.Count == 0)
            {
                regular = "";
                return false;
            }
            do
            {
                regular = list.Units[Random.Range(0, list.Units.Count - 1)] + "-" + Random.Range(0, list.MaxCode).ToString("00");
            }
            while (UnitNamingRule.UsedCombinations.Contains(regular));
            __instance.AddCombination(regular, type);
            return false;
        }
    }
}