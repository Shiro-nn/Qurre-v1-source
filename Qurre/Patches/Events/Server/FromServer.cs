#pragma warning disable SA1313
using System;
using System.Linq;
using HarmonyLib;
using Qurre.API.Events;
using RemoteAdmin;
namespace Qurre.Patches.Events.Server
{
    [HarmonyPatch(typeof(QueryProcessor), "ProcessGameConsoleQuery", new Type[] { typeof(string), typeof(bool) })]
    internal static class FromServer
    {
        private static bool Prefix(QueryProcessor __instance, string query, bool encrypted)
        {
            try
            {
                string ncmd = query;
                var ev = new SendingConsoleEvent(API.Round.Host, ncmd, encrypted, "", "white", true);
                Qurre.Events.Server.sendingconsole(ev);
                if (!string.IsNullOrEmpty(ev.ReturnMessage))
                {
                    __instance.GCT.SendToClient(__instance.connectionToClient, ev.ReturnMessage, ev.Color);
                }
                return ev.IsAllowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Server.FromServer:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}