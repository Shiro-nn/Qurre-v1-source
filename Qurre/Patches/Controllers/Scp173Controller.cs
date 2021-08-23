using HarmonyLib;
using PlayableScps;
using Qurre.API;
using UnityEngine;
namespace Qurre.Patches.Controllers
{
	[HarmonyPatch(typeof(Scp173), nameof(Scp173.UpdateObservers))]
	internal static class Scp173Controller
	{
		private static bool Prefix(Scp173 __instance)
		{
			try
			{
				var peanut = Player.Get(__instance.Hub);
				foreach (Player pl in Player.List)
				{
					if (pl.Role == RoleType.Spectator || pl == peanut || pl.Team == Team.SCP || peanut.Scp173Controller.IgnoredPlayers.Contains(pl))
					{
						if (__instance._observingPlayers.Contains(pl.ReferenceHub)) __instance._observingPlayers.Remove(pl.ReferenceHub);
					}
					else
					{
						Vector3 realModelPosition = __instance.Hub.playerMovementSync.RealModelPosition;
						bool flag = false;
						if (VisionInformation.GetVisionInformation(pl.ReferenceHub, realModelPosition, -2f, 40f, false, false,
							__instance.Hub.localCurrentRoomEffects, 0).IsLooking && (!Physics.Linecast(realModelPosition + new Vector3(0f, 1.5f, 0f),
							pl.ReferenceHub.PlayerCameraReference.position, VisionInformation.VisionLayerMask) || !Physics.Linecast(realModelPosition + new Vector3(0f, -1f, 0f),
							pl.ReferenceHub.PlayerCameraReference.position, VisionInformation.VisionLayerMask))) flag = true;
						if (flag)
						{
							if (!__instance._observingPlayers.Contains(pl.ReferenceHub)) __instance._observingPlayers.Add(pl.ReferenceHub);
						}
						else if (__instance._observingPlayers.Contains(pl.ReferenceHub)) __instance._observingPlayers.Remove(pl.ReferenceHub);
					}
				}
				__instance._isObserved = (__instance._observingPlayers.Count > 0);
				return false;
			}
			catch (System.Exception e)
			{
				Log.Error($"umm, error in patching Controllers [Scp173Controller]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}