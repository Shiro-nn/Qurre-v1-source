﻿using System;
using HarmonyLib;
using Qurre.API.Events;
namespace Qurre.Patches.Events.Server.Report
{
    using Qurre.API;
    [HarmonyPatch(typeof(CheaterReport), nameof(CheaterReport.IssueReport))]
    internal static class Cheater
    {
        private static bool Prefix(CheaterReport __instance, GameConsoleTransmission reporter, string reporterUserId, string reportedUserId, string reason)
        {
            try
            {
                try { if (reportedUserId == reporterUserId) reporter.SendToClient(__instance.connectionToClient, "smart, smart", "yellow"); } catch { }
                var ev = new ReportCheaterEvent(Player.Get(reporterUserId), Player.Get(reportedUserId), ServerConsole.Port, reason);
                Qurre.Events.Invoke.Report.Cheater(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Server -> Report [Cheater]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}