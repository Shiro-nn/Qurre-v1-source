using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
namespace Qurre.Patches.Events.Server.Report
{
    [HarmonyPatch(typeof(CheaterReport), nameof(CheaterReport.UserCode_CmdReport))]
    internal static class Local
    {
        private static bool Prefix(CheaterReport __instance, int playerId, string reason, ref bool notifyGm)
        {
            try
            {
                Player issuer = Player.Get(__instance.gameObject);
                Player target = Player.Get(playerId);
                if (target is null) return false;
                var ev = new ReportLocalEvent(issuer, target, reason, notifyGm);
                Qurre.Events.Invoke.Report.Local(ev);
                notifyGm = ev.GlobalReport;
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Server -> Report [Local]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}