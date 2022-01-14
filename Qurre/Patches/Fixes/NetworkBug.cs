/*using HarmonyLib;
using Mirror;
using UnityEngine;
namespace Qurre.Patches.Fixes
{
    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.SendRealIds))]
    internal static class NetworkBug
    {
        internal static bool Prefix(ServerRoles __instance)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void ServerRoles::SendRealIds()' called when server was not active");
            }
            else
            {
                if (__instance._hub.isDedicatedServer) return false;
                bool flag = __instance.Staff || __instance.RaEverywhere || PermissionsHandler.IsPermitted(__instance.Permissions, 18007046uL);
                if (!flag && !__instance._lastRealIdPerm) return false;
                __instance._lastRealIdPerm = flag;
                foreach (ReferenceHub value in ReferenceHub.GetAllHubs().Values)
                {
                    if (!value.isDedicatedServer)
                    {
                        try
                        {
                            value.characterClassManager.TargetSetRealId(__instance._hub.networkIdentity.connectionToClient, flag ? value.characterClassManager.UserId : null);
                        }
                        catch { }
                    }
                }
            }
            return false;
        }
    }
}*/