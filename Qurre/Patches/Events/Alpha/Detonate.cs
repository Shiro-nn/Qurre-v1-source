using HarmonyLib;
namespace Qurre.Patches.Events.Alpha
{
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.Detonate))]
    internal static class Detonate
    {
        private static void Prefix() => Qurre.Events.Invoke.Alpha.Detonated();
    }
}