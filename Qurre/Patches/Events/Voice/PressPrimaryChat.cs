﻿using HarmonyLib;
using Qurre.API.Events;
namespace Qurre.Patches.Events.Voice
{
	using Qurre.API;
	[HarmonyPatch(typeof(Radio), nameof(Radio.Network_syncPrimaryVoicechatButton), MethodType.Setter)]
	internal static class PressPrimaryChat
	{
		public static void Prefix(Radio __instance, ref bool value)
		{
			try
			{
				var pl = Player.Get(__instance._hub);
				if (pl is null) return;
				var ev = new PressPrimaryChatEvent(pl, value);
				Qurre.Events.Invoke.Voice.PressPrimaryChat(ev);
				value = ev.Value;
			}
			catch (System.Exception e)
			{
				Log.Error($"umm, error in patching Voice [PressPrimaryChat]:\n{e}\n{e.StackTrace}");
			}
		}
	}
}