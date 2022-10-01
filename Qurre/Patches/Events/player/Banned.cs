using HarmonyLib;
using Qurre.API.Events;
using System.Collections.Generic;
namespace Qurre.Patches.Events.Player
{
    [HarmonyPatch(typeof(BanHandler), nameof(BanHandler.IssueBan))]
    internal static class Banned
    {
        internal static readonly List<string> Cached = new();
        private static void Postfix(BanDetails ban, BanHandler.BanType banType)
        {
            try
            {
                string _cache = ban.ToString();
                if (Cached.Contains(_cache))
                {
                    _cache = "";
                    return;
                }
                var ev = new BannedEvent(string.IsNullOrEmpty(ban.Id) ? null : API.Player.Get(ban.Id), ban, banType);
                Qurre.Events.Invoke.Player.Banned(ev);
                Cached.Add(_cache);
                _cache = "";
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Player [Banned]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}