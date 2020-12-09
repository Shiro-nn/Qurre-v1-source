#pragma warning disable SA1313
using System;
using HarmonyLib;
using Qurre.API.Events;
namespace Qurre.Patches.Events.Server
{
    [HarmonyPatch(typeof(RemoteAdmin.QueryProcessor), nameof(RemoteAdmin.QueryProcessor.ProcessGameConsoleQuery), new Type[] { typeof(string), typeof(bool) })]
    internal static class Console_
    {
        private static bool Prefix(RemoteAdmin.QueryProcessor __instance, ref string query, bool encrypted)
        {
            try
            {
                if (query == null) query = "";
                string str = query;
                ReferenceHub send;
                try { send = ReferenceHub.GetHub(__instance.gameObject) ?? API.Map.Host; } catch { send = API.Map.Host; }
                if (send == null) send = API.Map.Host;
                var ev = new SendingConsoleEvent(send, str, encrypted);
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