﻿#pragma warning disable SA1313
using System;
using HarmonyLib;
using Qurre.API;
using static Qurre.API.Events.Report;
namespace Qurre.Patches.Events.Server.Report
{
    [HarmonyPatch(typeof(CheaterReport), nameof(CheaterReport.IssueReport))]
    internal static class Cheater
    {
        private static bool Prefix(CheaterReport __instance, GameConsoleTransmission reporter, string reporterUserId, string reportedUserId, ref string reason)
        {
            try
            {
                if (reportedUserId == reporterUserId)
                    reporter.SendToClient(__instance.connectionToClient, "smart, smart" + Environment.NewLine, "yellow");
                var ev = new CheaterEvent(Player.Get(reporterUserId), Player.Get(reportedUserId), ServerConsole.Port, reason);
                Qurre.Events.Server.Report.cheater(ev);
                reason = ev.Reason;
                return ev.IsAllowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Server.Report.Cheater:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}