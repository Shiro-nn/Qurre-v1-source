#pragma warning disable SA1313
using HarmonyLib;
namespace Qurre.Patches.Events.Alpha
{
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.Detonate))]
    internal static class Detonate
    {
        private static void Prefix() => Qurre.Events.Alpha.detonated();
    }
}