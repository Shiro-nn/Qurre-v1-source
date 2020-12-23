#pragma warning disable CS8138
using HarmonyLib;
using Qurre.API.Events;
using RemoteAdmin;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.Server
{
	[HarmonyPatch(typeof(CommandProcessor), "ProcessQuery", new Type[]
	{
		typeof(string),
		typeof(CommandSender)
	})]
	internal static class RA
	{
		private static bool Prefix(ref string q, ref CommandSender sender)
		{
			var ev = new SendingRAEvent(sender, q);
			PreauthStopwatch().Restart();
			IdleMode.SetIdleMode(false);
			if (q == "REQUEST_DATA PLAYER_LIST SILENT")
				return true;
			Qurre.Events.Server.sendingra(ev);
			if (!string.IsNullOrEmpty(ev.ReplyMessage))
				sender.RaReply(ev.ReplyMessage, ev.Success, true, string.Empty);
			return ev.IsAllowed;
		}
    }
}