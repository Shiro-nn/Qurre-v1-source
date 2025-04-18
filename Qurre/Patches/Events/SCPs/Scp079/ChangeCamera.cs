﻿using System;
using HarmonyLib;
using Qurre.API.Events;
namespace Qurre.Patches.Events.SCPs.Scp079
{
	using Qurre.API;
	[HarmonyPatch(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.UserCode_CmdSwitchCamera))]
	internal static class ChangeCamera
	{
		private static bool Prefix(Scp079PlayerScript __instance, ushort cameraId, bool lookatRotation)
		{
			try
			{
				if (!__instance._interactRateLimit.CanExecute(true) || !__instance.iAm079)
					return true;

				if (!Scp079PlayerScript.allCameras.TryFind(out Camera079 camera, x => x.cameraId == cameraId)) return false;

				float num = __instance.CalculateCameraSwitchCost(camera.transform.position);

				var ev = new ChangeCameraEvent(Player.Get(__instance.gameObject), camera.GetCamera(), num);
				Qurre.Events.Invoke.Scp079.ChangeCamera(ev);

				if (!ev.Allowed) return false;

				if (ev.PowerCost > __instance._curMana)
				{
					__instance.RpcNotEnoughMana(ev.PowerCost, __instance._curMana);
					return false;
				}

				__instance.RpcSwitchCamera(cameraId, lookatRotation);
				__instance.Mana -= ev.PowerCost;
				__instance.currentCamera = camera;

				return false;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching SCPs -> SCP079 [ChangeCamera]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}