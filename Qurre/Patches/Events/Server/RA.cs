#pragma warning disable CS8138
using HarmonyLib;
using Qurre.API.Events;
using RemoteAdmin;
using System;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.Server
{
	[HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery), new Type[] { typeof(string), typeof(CommandSender) })]
	internal static class RA
	{
		private static bool Prefix(ref string q, ref CommandSender sender)
		{
			try
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
			catch (Exception e)
			{
				Log.Error($"umm, error in patching Server.RA:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}