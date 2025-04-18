﻿using System;
using System.Linq;
using HarmonyLib;
using Qurre.API.Events;
using RemoteAdmin;
namespace Qurre.Patches.Events.Server
{
    using Qurre.API;
    [HarmonyPatch(typeof(QueryProcessor), nameof(QueryProcessor.ProcessGameConsoleQuery))]
    internal static class Console
    {
        private static bool Prefix(QueryProcessor __instance, string query)
        {
            try
            {
                string[] allarguments = query.Split(' ');
                string name = allarguments[0].ToLower();
                string[] args = allarguments.Skip(1).ToArray();
                var ev = new SendingConsoleEvent(Player.Get(__instance.gameObject), query, name, args);
                Qurre.Events.Invoke.Server.SendingConsole(ev);
                if (!string.IsNullOrEmpty(ev.ReturnMessage)) __instance.GCT.SendToClient(__instance.connectionToClient, ev.ReturnMessage, ev.Color);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Server -> [Console]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}