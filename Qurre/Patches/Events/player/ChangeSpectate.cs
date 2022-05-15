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
                if (__instance is null || __instance._hub is null || value is null) return;
                var player = Player.Get(__instance._hub);
                if (player != null)
                {
                    Player _val = null;
                    try { _val = Player.Get(value); } catch { }
                    Player _cur = null;
                    try { _cur = Player.Get(__instance.CurrentSpectatedPlayer); } catch { }
                    var ev = new ChangeSpectateEvent(player, _cur, _val);
                    Qurre.Events.Invoke.Player.ChangeSpectate(ev);
                    if (!ev.Allowed)
                    {
                        value = __instance.CurrentSpectatedPlayer;
                        return;
                    }
                    value = ev.NewTarget?.ReferenceHub ?? ev.Player.ReferenceHub;
                    _val = null;
                    _cur = null;
                }
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Player [ChangeSpectate]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}