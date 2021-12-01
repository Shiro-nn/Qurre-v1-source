using HarmonyLib;
using Qurre.API;
namespace Qurre.Patches.Modules
{
    [HarmonyPatch(typeof(HitboxIdentity), nameof(HitboxIdentity.CheckFriendlyFire), new[] { typeof(ReferenceHub), typeof(ReferenceHub), typeof(bool) })]
    internal static class FriendlyFire
    {
        internal static bool Prefix(ref bool __result, ReferenceHub attacker)
        {
            if (Player.Get(attacker) == null) return true;
            __result = Player.Get(attacker).FriendlyFire || Server.FriendlyFire;
            return !__result;
        }
    }
}