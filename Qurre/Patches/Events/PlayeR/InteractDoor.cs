#pragma warning disable
using System;
using System.Linq;
using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using Qurre.API.Events;
using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.PlayeR
{
	[HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerInteract), new Type[] { typeof(ReferenceHub), typeof(byte) })]
	internal static class InteractDoor
	{
		private static bool Prefix(DoorVariant __instance, ReferenceHub ply, byte colliderId)
		{
			try
			{
				var ev = new InteractDoorEvent(API.Player.Get(__instance.gameObject), __instance, false);
				bool boolean = false;
				if (__instance.ActiveLocks != 0)
				{
					DoorLockMode DLM = DoorLockUtils.GetMode((DoorLockReason)__instance.ActiveLocks);
					if ((!DLM.HasFlagFast(DoorLockMode.CanClose) || !DLM.HasFlagFast(DoorLockMode.CanOpen)) && (!DLM.HasFlagFast(DoorLockMode.ScpOverride) || ply.characterClassManager.CurRole.team != Team.SCP) && (DLM == DoorLockMode.FullLock || (__instance.TargetState && !DLM.HasFlagFast(DoorLockMode.CanClose)) || (!__instance.TargetState && !DLM.HasFlagFast(DoorLockMode.CanOpen))))
					{
						ev.IsAllowed = false;
						boolean = true;
					}
				}
				if (__instance.AllowInteracting(ply, colliderId))
				{
					if (ply.characterClassManager.CurClass == RoleType.Scp079 || __instance.RequiredPermissions.CheckPermissions(ply.inventory.curItem, ply)) ev.IsAllowed = true;
					else ev.IsAllowed = false;
				}
				Qurre.Events.Player.interactdoor(ev);
				if (ev.IsAllowed)
				{
					__instance.NetworkTargetState = !__instance.TargetState;
					__instance._triggerPlayer(ply);
				}
				else if (boolean)
					__instance.LockBypassDenied(ply, colliderId);
				else
				{
					__instance.PermissionsDenied(ply, colliderId);
					DoorEvents.TriggerAction(__instance, DoorAction.AccessDenied, ply);
				}
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