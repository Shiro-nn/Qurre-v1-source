#pragma warning disable SA1118
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
namespace Qurre.Patches.Events.Server.Report
{
    [HarmonyPatch(typeof(CheaterReport), nameof(CheaterReport.CallCmdReport), typeof(int), typeof(string), typeof(byte[]), typeof(bool))]
    internal static class Local
    {
        private static bool Prefix(CheaterReport __instance, int playerId, string reason, ref bool notifyGm)
        {
            try
            {
                var issuer = Player.Get(__instance.gameObject);
                var target = Player.Get(playerId);
                if (target == null) return false;
                var ev = new ReportLocalEvent(issuer, target, reason);
                Qurre.Events.Server.Report.local(ev);
                reason = ev.Reason;
                if (!ev.Allowed && issuer.GameObject != target.GameObject) issuer.SendConsoleMessage("[Local Report] Successfully", "green");
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Server.Report.Local:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}