using System;
using HarmonyLib;
using Qurre.API.Events;
namespace Qurre.Patches.Events.Player
{
	using Qurre.API;
	[HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.UserCode_CmdRequestHideTag))]
	internal static class HideBadge
	{
		private static bool Prefix(CharacterClassManager __instance)
		{
			try
			{
				if (__instance is null || !__instance._commandRateLimit.CanExecute(true))
					return false;

				if (!string.IsNullOrEmpty(__instance.SrvRoles.HiddenBadge))
				{
					__instance.TargetConsolePrint(__instance.connectionToClient, "Your badge is already hidden.", "yellow");
					return false;
				}

				if (string.IsNullOrEmpty(__instance.SrvRoles.MyText))
				{
					__instance.TargetConsolePrint(__instance.connectionToClient, "You don't have a badge.", "red");
					return false;
				}

				var ev = new HideBadgeEvent(Player.Get(__instance._hub), __instance.SrvRoles.MyText, __instance.SrvRoles.GlobalSet);
				Qurre.Events.Invoke.Player.HideBadge(ev);

				if (!ev.Allowed)
				{
					__instance.TargetConsolePrint(__instance.connectionToClient, "The tag hiding was denied.", "red");
					return false;
				}

				__instance.SrvRoles.GlobalHidden = ev.Global;
				__instance.SrvRoles.HiddenBadge = ev.Global ? __instance.SrvRoles.MyText : ev.Badge;
				__instance.SrvRoles.NetworkGlobalBadge = null;
				__instance.SrvRoles.SetText(null);
				__instance.SrvRoles.SetColor(null);
				__instance.SrvRoles.RefreshHiddenTag();
				__instance.TargetConsolePrint(__instance.connectionToClient, "Badge hidden.", "green");

				return false;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching Player [HideTag]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}