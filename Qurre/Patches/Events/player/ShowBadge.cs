<<<<<<< HEAD
﻿using System;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using GameCore;
namespace Qurre.Patches.Events.player
{
	[HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.UserCode_CmdRequestShowTag))]
	internal static class ShowBadge
	{
		private static bool Prefix(CharacterClassManager __instance, bool global)
		{
			try
			{
				if (__instance is null || !__instance._commandRateLimit.CanExecute(true))
					return false;

				var ev = new ShowBadgeEvent(Player.Get(__instance._hub), string.IsNullOrEmpty(__instance.SrvRoles.HiddenBadge) ? __instance.SrvRoles.MyText : __instance.SrvRoles.HiddenBadge, global);
				Qurre.Events.Invoke.Player.ShowBadge(ev);

				if (!ev.Allowed)
				{
					__instance.TargetConsolePrint(__instance.connectionToClient, "The tag showing was denied.", "red");
					return false;
				}

				if (!global)
				{
					__instance.SrvRoles.NetworkGlobalBadge = null;
					__instance.SrvRoles.HiddenBadge = null;
					__instance.SrvRoles.RpcResetFixed();
					__instance.SrvRoles.RefreshPermissions(true);
					__instance.SrvRoles.MyText = ev.Badge;
					__instance.TargetConsolePrint(__instance.connectionToClient, "Local tag refreshed.", "green");
					return false;
				}

				if (string.IsNullOrEmpty(__instance.SrvRoles.PrevBadge))
				{
					__instance.TargetConsolePrint(__instance.connectionToClient, "You don't have a global tag.", "magenta");
					return false;
				}

				if ((string.IsNullOrEmpty(__instance.SrvRoles.MyText)
					|| !__instance.SrvRoles.RemoteAdmin) && (((__instance.SrvRoles.GlobalBadgeType == 3
					|| __instance.SrvRoles.GlobalBadgeType == 4) && ConfigFile.ServerConfig.GetBool("block_gtag_banteam_badges", false) && !ServerStatic.PermissionsHandler.IsVerified)
					|| (__instance.SrvRoles.GlobalBadgeType == 1 && ConfigFile.ServerConfig.GetBool("block_gtag_staff_badges", false))
					|| (__instance.SrvRoles.GlobalBadgeType == 2 && ConfigFile.ServerConfig.GetBool("block_gtag_management_badges", false) && !ServerStatic.PermissionsHandler.IsVerified)
					|| (__instance.SrvRoles.GlobalBadgeType == 0 && ConfigFile.ServerConfig.GetBool("block_gtag_patreon_badges", false) && !ServerStatic.PermissionsHandler.IsVerified)))
				{
					__instance.TargetConsolePrint(__instance.connectionToClient, "You can't show this type of global badge on this server. Try joining server with global badges allowed.", "red");
					return false;
				}

				__instance.SrvRoles.NetworkGlobalBadge = __instance.SrvRoles.PrevBadge;
				__instance.SrvRoles.GlobalHidden = false;
				__instance.SrvRoles.HiddenBadge = null;
				__instance.SrvRoles.RpcResetFixed();
				__instance.TargetConsolePrint(__instance.connectionToClient, "Global tag refreshed.", "green");

				return false;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching Player [HideTag]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
=======
﻿using System;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using GameCore;
namespace Qurre.Patches.Events.player
{
	[HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.UserCode_CmdRequestShowTag))]
	internal static class ShowBadge
	{
		private static bool Prefix(CharacterClassManager __instance, bool global)
		{
			try
			{
				if (__instance is null || !__instance._commandRateLimit.CanExecute(true))
					return false;

				var ev = new ShowBadgeEvent(Player.Get(__instance._hub), string.IsNullOrEmpty(__instance.SrvRoles.HiddenBadge) ? __instance.SrvRoles.MyText : __instance.SrvRoles.HiddenBadge, global);
				Qurre.Events.Invoke.Player.ShowBadge(ev);

				if (!ev.Allowed)
				{
					__instance.TargetConsolePrint(__instance.connectionToClient, "The tag showing was denied.", "red");
					return false;
				}

				if (!global)
				{
					__instance.SrvRoles.NetworkGlobalBadge = null;
					__instance.SrvRoles.HiddenBadge = null;
					__instance.SrvRoles.RpcResetFixed();
					__instance.SrvRoles.RefreshPermissions(true);
					__instance.SrvRoles.MyText = ev.Badge;
					__instance.TargetConsolePrint(__instance.connectionToClient, "Local tag refreshed.", "green");
					return false;
				}

				if (string.IsNullOrEmpty(__instance.SrvRoles.PrevBadge))
				{
					__instance.TargetConsolePrint(__instance.connectionToClient, "You don't have a global tag.", "magenta");
					return false;
				}

				if ((string.IsNullOrEmpty(__instance.SrvRoles.MyText)
					|| !__instance.SrvRoles.RemoteAdmin) && (((__instance.SrvRoles.GlobalBadgeType == 3
					|| __instance.SrvRoles.GlobalBadgeType == 4) && ConfigFile.ServerConfig.GetBool("block_gtag_banteam_badges", false) && !ServerStatic.PermissionsHandler.IsVerified)
					|| (__instance.SrvRoles.GlobalBadgeType == 1 && ConfigFile.ServerConfig.GetBool("block_gtag_staff_badges", false))
					|| (__instance.SrvRoles.GlobalBadgeType == 2 && ConfigFile.ServerConfig.GetBool("block_gtag_management_badges", false) && !ServerStatic.PermissionsHandler.IsVerified)
					|| (__instance.SrvRoles.GlobalBadgeType == 0 && ConfigFile.ServerConfig.GetBool("block_gtag_patreon_badges", false) && !ServerStatic.PermissionsHandler.IsVerified)))
				{
					__instance.TargetConsolePrint(__instance.connectionToClient, "You can't show this type of global badge on this server. Try joining server with global badges allowed.", "red");
					return false;
				}

				__instance.SrvRoles.NetworkGlobalBadge = __instance.SrvRoles.PrevBadge;
				__instance.SrvRoles.GlobalHidden = false;
				__instance.SrvRoles.HiddenBadge = null;
				__instance.SrvRoles.RpcResetFixed();
				__instance.TargetConsolePrint(__instance.connectionToClient, "Global tag refreshed.", "green");

				return false;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching Player [HideTag]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
>>>>>>> 5bcab29aaf87b64d74a77fd4999a3f7a38363a22
}