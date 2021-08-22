using HarmonyLib;
using Qurre.API;
namespace Qurre.Patches.etc
{
    [HarmonyPatch(typeof(HitboxIdentity), nameof(HitboxIdentity.CheckFriendlyFire), new[] { typeof(ReferenceHub), typeof(ReferenceHub), typeof(bool) })]
    internal static class FriendlyFire
    {
        private static bool Prefix(ReferenceHub attacker, ref bool __result) => !(__result = Player.Get(attacker).FriendlyFire || Server.FriendlyFire);
    }
}