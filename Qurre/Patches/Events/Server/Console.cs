#pragma warning disable SA1313
using System;
using System.Linq;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using RemoteAdmin;
namespace Qurre.Patches.Events.Server
{
    [HarmonyPatch(typeof(QueryProcessor), "ProcessGameConsoleQuery", new Type[] { typeof(string), typeof(bool) })]
    internal static class Console_
    {
        private static bool Prefix(RemoteAdmin.QueryProcessor __instance, string query, bool encrypted)
        {
            try
            {
                string[] allarguments = query.Split(' ');
                string name = allarguments[0].ToLower();
                string[] args = allarguments.Skip(1).ToArray();
                Player send;
                try { send = Player.Get(__instance.gameObject) ?? API.Map.Host; } catch { send = API.Map.Host; }
                if (send == null) send = Map.Host;
                var ev = new SendingConsoleEvent(send, query, name, args, encrypted);
                Qurre.Events.Server.sendingconsole(ev);
                if (ev.ReturnMessage != "") __instance.GCT.SendToClient(__instance.connectionToClient, ev.ReturnMessage, ev.Color);
                return ev.IsAllowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Server.Console_:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}