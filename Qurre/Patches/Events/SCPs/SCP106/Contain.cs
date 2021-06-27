using HarmonyLib;
using Qurre.API.Events;
namespace Qurre.Patches.Events.SCPs.SCP106
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdContain106))]
    internal static class Contain
    {
        private static bool Prefix(CharacterClassManager __instance)
        {
            var ev = new ContainEvent(API.Player.Get(__instance.gameObject));
            Qurre.Events.Invoke.Scp106.Contain(ev);
            return ev.Allowed;
        }
    }
}