using HarmonyLib;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.OnInteract))]
    internal static class Interact
    {
        private static void Prefix(PlayerInteract __instance) => Qurre.Events.Invoke.Player.Interact(new(API.Player.Get(__instance.gameObject)));
    }
}