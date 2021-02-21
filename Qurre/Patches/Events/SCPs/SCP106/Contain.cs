#pragma warning disable SA1118
using HarmonyLib;
using static Qurre.API.Events.SCP106;
namespace Qurre.Patches.Events.SCPs.SCP106
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdContain106))]
    internal static class Contain
    {
        private static bool Prefix(CharacterClassManager __instance)
        {
            var ev = new ContainEvent(API.Player.Get(__instance.gameObject));
            Qurre.Events.SCPs.SCP106.contain(ev);
            return ev.IsAllowed;
        }
    }
}