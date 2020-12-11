#pragma warning disable SA1313
using System;
using System.Linq;
using HarmonyLib;
using Qurre.API.Events;
using Console = GameCore.Console;
namespace Qurre.Patches.Events.Server
{
    [HarmonyPatch(typeof(Console), nameof(Console.TypeCommand), new Type[] { typeof(string), typeof(CommandSender) })]
    internal static class FromServer
    {
        private static bool Prefix(string cmd)
        {
            try
            {
                var ev = new SendingRAEvent(new BotSender(), API.Map.Host, cmd);
                Qurre.Events.Server.sendingra(ev);
                return ev.IsAllowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Server.FromServer:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
        public class BotSender : CommandSender
        {
            public override void RaReply(string text, bool success, bool logToConsole, string overrideDisplay)
            {
                ServerConsole.AddLog(text, ConsoleColor.Gray);
            }

            public override void Print(string text)
            {
                ServerConsole.AddLog(text, ConsoleColor.Gray);
            }
            public BotSender() { }
            public override string SenderId => "SERVER CONSOLE";
            public override string Nickname => "SERVER CONSOLE";
            public override ulong Permissions => ServerStatic.GetPermissionsHandler().FullPerm;
            public override byte KickPower => byte.MaxValue;
            public override bool FullPermissions => true;
        }
    }
}