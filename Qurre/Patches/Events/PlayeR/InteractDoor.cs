#pragma warning disable
using System;
using System.Linq;
using HarmonyLib;
using Qurre.API.Events;
using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.PlayeR
{
	[HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdOpenDoor), typeof(GameObject))]
	internal static class InteractDoor
	{
		public static bool Prefix(PlayerInteract __instance, GameObject doorId)
		{
			Door door = doorId.GetComponent<Door>();
			var ev = new InteractDoorEvent(ReferenceHub.GetHub(__instance.gameObject), door);

			try
			{
				if (!__instance.RateLimit().CanExecute() ||
					(__instance.InteractCuff().CufferId > 0 && !DisarmedInteract()) ||
					doorId == null ||
					(__instance.CCMPI().CurClass == RoleType.None || __instance.CCMPI().CurClass == RoleType.Spectator))
					return false;
				__instance.OnInteract();
				if (__instance.ServerRolesInteract().BypassMode)
				{
					ev.IsAllowed = true;
				}
				else if (string.Equals(door.permissionLevel, "CHCKPOINT_ACC", StringComparison.OrdinalIgnoreCase) &&
					__instance.GetComponent<CharacterClassManager>().Classes.SafeGet(__instance.GetComponent<CharacterClassManager>().CurClass).team == Team.SCP)
				{
					ev.IsAllowed = true;
				}
				else
				{
					Item item = __instance.InteractInv().GetItemByID(__instance.InteractInv().curItem);
					if (string.IsNullOrEmpty(door.permissionLevel))
					{
						ev.IsAllowed = !door.locked;
					}
					else if (item != null && item.permissions.Contains(door.permissionLevel))
					{
						ev.IsAllowed = !door.locked;
					}
					else
						ev.IsAllowed = false;
				}
				Qurre.Events.Player.interactdoor(ev);


				if (!ev.IsAllowed)
				{
					__instance.RpcDenied(doorId);
					return false;
				}

				door.ChangeState();

				return false;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching PlayeR.InteractDoor:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}