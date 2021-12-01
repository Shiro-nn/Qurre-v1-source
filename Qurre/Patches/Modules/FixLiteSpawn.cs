using HarmonyLib;
using InventorySystem;
namespace Qurre.Patches.Modules
{
    [HarmonyPatch(typeof(InventoryItemProvider), nameof(InventoryItemProvider.RoleChanged))]
    internal static class FixLiteSpawn
    {
        private static bool Prefix(bool lite) => !lite;
    }
}