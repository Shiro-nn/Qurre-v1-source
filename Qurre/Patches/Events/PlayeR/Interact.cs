#pragma warning disable SA1313
using HarmonyLib;
using Qurre.API.Events;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.OnInteract))]
    internal static class Interact
    {
        private static void Prefix(PlayerInteract __instance) => Qurre.Events.Player.interact(new InteractEvent(API.Player.Get(__instance.gameObject)));
    }
}