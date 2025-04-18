﻿using HarmonyLib;
using Qurre.API.Events;
using RemoteAdmin;
using System;
using System.Linq;
namespace Qurre.Patches.Events.Server
{
	[HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery), new Type[] { typeof(string), typeof(CommandSender) })]
	internal static class RemoteAdminCommand
	{
		private static bool Prefix(string q, CommandSender sender)
		{
			try
			{
				string[] allarguments = q.Split(' ');
				string name = allarguments[0].ToLower();
				string[] args = allarguments.Skip(1).ToArray();
				IdleMode.PreauthStopwatch.Restart();
				IdleMode.SetIdleMode(false);
				if (q == "$0 1")
				{
					var ev = new RaRequestPlayerListEvent(sender, string.IsNullOrEmpty(sender.SenderId) ? API.Server.Host : (API.Player.Get(sender.SenderId) ?? API.Server.Host), q, name, args);
					Qurre.Events.Invoke.Server.RaRequestPlayerList(ev);
					return ev.Allowed;
				}
				else
				{
					var ev = new SendingRAEvent(sender, string.IsNullOrEmpty(sender.SenderId) ? API.Server.Host : (API.Player.Get(sender.SenderId) ?? API.Server.Host), q, name, args);
					Qurre.Events.Invoke.Server.SendingRA(ev);
					if (!string.IsNullOrEmpty(ev.ReplyMessage))
						sender.RaReply(ev.ReplyMessage, ev.Success, true, string.Empty);
					return ev.Allowed;
				}
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching Server [RA]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}