#pragma warning disable SA1313
using HarmonyLib;
using Qurre.API.Events;
using RemoteAdmin;
using System;
namespace Qurre.Patches.Events.Server
{
    [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery), typeof(string), typeof(CommandSender))]
    internal static class RA
    {
        private static bool Prefix(string q, CommandSender sender)
        {
            try
            {
                if (q.Contains("REQUEST_DATA PLAYER_LIST SILENT")) return true;
                if (sender == null) sender = new Error();
                var ev = new SendingRAEvent(sender, q);
                Qurre.Events.Server.sendingra(ev);
                return ev.IsAllowed;
            }
            catch/* (System.Exception e)*/
            {
                //if (API.Round.IsStarted) Log.Error($"umm, error in patching Server.RA:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
        public class Error : CommandSender
        {
            public override void RaReply(string text, bool success, bool logToConsole, string overrideDisplay) => ServerConsole.AddLog(text, ConsoleColor.Gray);
            public override void Print(string text) => ServerConsole.AddLog(text, ConsoleColor.Gray);
            public Error() { }
            public override string SenderId => "ERROR";
            public override string Nickname => "ERROR";
            public override ulong Permissions => 0;
            public override byte KickPower => 0;
            public override bool FullPermissions => false;
        }
    }
}