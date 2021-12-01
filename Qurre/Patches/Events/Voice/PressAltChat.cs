using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
namespace Qurre.Patches.Events.Voice
{
	[HarmonyPatch(typeof(Radio), nameof(Radio.Network_syncAltVoicechatButton), MethodType.Setter)]
	internal static class PressAltChat
	{
		public static void Prefix(Radio __instance, ref bool value)
		{
			try
			{
				var pl = Player.Get(__instance._hub);
				if (pl == null) return;
				var ev = new PressAltChatEvent(pl, value);
				Qurre.Events.Invoke.Voice.PressAltChat(ev);
				value = ev.Value;
			}
			catch (System.Exception e)
			{
				Log.Error($"umm, error in patching Voice [PressAltChat]:\n{e}\n{e.StackTrace}");
			}
		}
	}
}