using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(SpectatorManager), nameof(SpectatorManager.CurrentSpectatedPlayer), MethodType.Setter)]
    internal static class ChangeSpectate
    {
        internal static void Prefix(SpectatorManager __instance, ref ReferenceHub value)
        {
            try
            {
                var player = Player.Get(__instance._hub);
                if (player != null)
                {
                    var ev = new ChangeSpectateEvent(player, Player.Get(__instance.CurrentSpectatedPlayer), Player.Get(value));
                    Qurre.Events.Invoke.Player.ChangeSpectate(ev);
                    if (!ev.Allowed)
                    {
                        value = __instance.CurrentSpectatedPlayer;
                        return;
                    }
                    value = ev.NewTarget?.ReferenceHub ?? ev.Player.ReferenceHub;
                }
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Player [ChangeSpectate]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}