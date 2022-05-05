using HarmonyLib;
using InventorySystem;

namespace Qurre.Patches.Fixes
{
    [HarmonyPatch(typeof(InventoryItemProvider), "RoleChanged")]
    internal static class LiteSpawn
    {
        private static bool Prefix(bool lite) => !lite;
    }
}