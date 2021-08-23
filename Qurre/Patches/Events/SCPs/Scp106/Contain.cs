using HarmonyLib;
using Qurre.API.Events;
namespace Qurre.Patches.Events.SCPs.SCP106
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdContain106))]
    internal static class Contain
    {
        private static bool Prefix(PlayerInteract __instance)
        {
            var ev = new ContainEvent(API.Player.Get(__instance._hub));
            Qurre.Events.Invoke.Scp106.Contain(ev);
            return ev.Allowed;
        }
    }
}