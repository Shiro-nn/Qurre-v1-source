using System;
using System.Linq;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;

namespace Qurre.Patches.Events.SCPs.Scp079
{
    [HarmonyPatch(typeof(Scp079PlayerScript), "UserCode_CmdSwitchCamera")]
    public class ChangeCamera
    {
        private static bool Prefix(Scp079PlayerScript __instance, ushort cameraId, bool lookatRotation)
        {
            try
            {
				if (!__instance._interactRateLimit.CanExecute(true) || !__instance.iAm079)
					return false;

				Camera079 camera = null;

				foreach (Camera079 cam in Scp079PlayerScript.allCameras)
					if (cam.cameraId == cameraId)
						camera = cam;

				if (camera is null)
					return false;

				float num = __instance.CalculateCameraSwitchCost(camera.transform.position);

				Player player = Player.Get(__instance.gameObject);
				var ev = new ChangeCameraEvent(player, API.Controllers.Camera.Cameras.FirstOrDefault(x => x.Key == player.Scp079PlayerScript.currentCamera).Value, num, num > __instance._curMana);
				Qurre.Events.Invoke.Scp079.ChangeCamera(ev);

				if (!ev.Allowed)
				{
					__instance.RpcNotEnoughMana(num, __instance._curMana);
					return false;
                }
				
				__instance.RpcSwitchCamera(cameraId, lookatRotation);
				__instance.Mana -= num;
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
