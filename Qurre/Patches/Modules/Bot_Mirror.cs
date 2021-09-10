using HarmonyLib;
using Mirror;
using Qurre.API;
namespace Qurre.Patches.Modules
{
    [HarmonyPatch(typeof(NetworkBehaviour), "SendTargetRPCInternal")]
    internal static class Bot_Mirror
    {
        internal static bool Prefix(NetworkBehaviour __instance)
        {
            var player = Player.Get(__instance.gameObject);
            if (player != null && player.Bot) return false;
            return true;
        }
    }
}