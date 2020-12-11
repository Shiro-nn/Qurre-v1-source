#pragma warning disable SA1313
using HarmonyLib;
using Qurre.API.Events;
using RemoteAdmin;
namespace Qurre.Patches.Events.Server
{
    [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery), typeof(string), typeof(CommandSender))]
    internal static class RA
    {
        private static bool Prefix(ref string q, ref CommandSender sender)
        {
            try
            {
                if (q == null) q = "";
                string str = q;
                var sende = sender;
                try
                {
                    if (str.Contains("REQUEST_DATA PLAYER_LIST SILENT")) return true;
                    ReferenceHub send;
                    try { send = API.Player.Get(sende.SenderId) ?? API.Map.Host; }
                    catch { send = API.Map.Host; }
                    var ev = new SendingRaEvent(sende, send, str);
                    Qurre.Events.Server.sendingra(ev);
                    return ev.IsAllowed;
                }
                catch
                {
                    var ev = new SendingRaEvent(sende, API.Map.Host, str);
                    Qurre.Events.Server.sendingra(ev);
                    return ev.IsAllowed;
                }
            }
            catch (System.Exception e)
            {
                if (API.Round.IsStarted)
                    Log.Error($"umm, error in patching Server.RA:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}