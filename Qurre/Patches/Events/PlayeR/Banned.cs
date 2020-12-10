using HarmonyLib;
using Qurre.API.Events;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(BanHandler), nameof(BanHandler.IssueBan))]
    internal static class Banned
    {
        private static void Postfix(BanDetails ban, BanHandler.BanType banType)
        {
            try
            {
                var ev = new BannedEvent(string.IsNullOrEmpty(ban.Id) ? null : API.Player.Get(ban.Id), ban, banType);
                Qurre.Events.Player.banned(ev);
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching PlayeR.Banned:\n{e}\n{e.StackTrace}");
            }
        }
    }
}