using HarmonyLib;
using Mirror;
using UnityEngine;
using Qurre.API.Events;
using GameCore;
using Qurre.API;
using InventorySystem.Items.MicroHID;
using Qurre.API.Objects;
using PlayerStatsSystem;
namespace Qurre.Patches.Events.SCPs.Scp049
{
    [HarmonyPatch(typeof(PlayableScps.Scp049), nameof(PlayableScps.Scp049.BodyCmd_ByteAndGameObject))]
    internal static class Doctor
    {
        private static bool Prefix(PlayableScps.Scp049 __instance, byte num, GameObject go)
        {
            try
            {
                if (num == 2)
                {
                    if (!__instance._interactRateLimit.CanExecute() || go == null)
                        return false;
                    Ragdoll component = go.GetComponent<Ragdoll>();
                    if (component == null)
                        return false;
                    ReferenceHub ownerHub = component.Info.OwnerHub;
                    if (ownerHub == null)
                    {
                        Console.AddDebugLog("SCPCTRL", "SCP-049 | Request 'finish recalling' rejected; no target found", MessageImportance.LessImportant);
                        return false;
                    }
                    if (!__instance._recallInProgressServer || ownerHub != __instance._recallHubServer || __instance._recallProgressServer < 0.85f)
                    {
                        Console.AddDebugLog("SCPCTRL", "SCP-049 | Request 'finish recalling' rejected; Debug code: ", MessageImportance.LessImportant);
                        Console.AddDebugLog("SCPCTRL", "SCP-049 | CONDITION#1 " + (__instance._recallInProgressServer ? "<color=green>PASSED</color>" :
                            ("<color=red>ERROR</color> - " + __instance._recallInProgressServer)), MessageImportance.LessImportant, nospace: true);
                        Console.AddDebugLog("SCPCTRL", "SCP-049 | CONDITION#2 " + ((ownerHub == __instance._recallHubServer) ? "<color=green>PASSED</color>" :
                            ("<color=red>ERROR</color> - " + ownerHub.playerId + "-" + ((__instance._recallHubServer == null) ? "null" :
                            __instance._recallHubServer.playerId.ToString()))), MessageImportance.LessImportant);
                        Console.AddDebugLog("SCPCTRL", "SCP-049 | CONDITION#3 " + ((__instance._recallProgressServer >= 0.85f) ? "<color=green>PASSED</color>" :
                            ("<color=red>ERROR</color> - " + __instance._recallProgressServer)), MessageImportance.LessImportant, nospace: true);
                        return false;
                    }
                    if (ownerHub.characterClassManager.CurClass != RoleType.Spectator) return false;
                    var ev = new FinishRecallEvent(Player.Get(__instance.Hub.gameObject), Player.Get(ownerHub));
                    Qurre.Events.Invoke.Scp049.FinishRecall(ev);
                    if (!ev.Allowed) return false;
                    Console.AddDebugLog("SCPCTRL", "SCP-049 | Request 'finish recalling' accepted", MessageImportance.LessImportant);
                    ownerHub.characterClassManager.SetClassID(RoleType.Scp0492, CharacterClassManager.SpawnReason.Revived);
                    if (component.CompareTag("Ragdoll")) NetworkServer.Destroy(component.gameObject);
                    __instance._recallInProgressServer = false;
                    __instance._recallHubServer = null;
                    __instance._recallProgressServer = 0f;
                    return false;
                }
                else if (num == 1)
                {
                    if (!__instance._interactRateLimit.CanExecute() || go == null)
                        return false;
                    Ragdoll component2 = go.GetComponent<Ragdoll>();
                    if (component2 == null)
                    {
                        Console.AddDebugLog("SCPCTRL", "SCP-049 | Request 'start recalling' rejected; provided object is not a dead body", MessageImportance.LessImportant);
                        return false;
                    }
                    if (component2.Info.ExistenceTime > PlayableScps.Scp049.ReviveEligibilityDuration)
                    {
                        Console.AddDebugLog("SCPCTRL", "SCP-049 | Request 'start recalling' rejected; provided object has decayed too far", MessageImportance.LessImportant);
                        return false;
                    }
                    if (component2.Info.OwnerHub == null)
                    {
                        Console.AddDebugLog("SCPCTRL", "SCP-049 | Request 'start recalling' rejected; target not found", MessageImportance.LessImportant);
                        return false;
                    }
                    bool flag = false;
                    Rigidbody[] componentsInChildren = component2.GetComponentsInChildren<Rigidbody>();
                    for (int i = 0; i < componentsInChildren.Length; i++)
                    {
                        if (Vector3.Distance(componentsInChildren[i].transform.position,
                            __instance.Hub.PlayerCameraReference.transform.position) <= PlayableScps.Scp049.ReviveDistance * 1.3f)
                        {
                            flag = true;
                            component2.Info.OwnerHub.characterClassManager.DeathPosition = __instance.Hub.playerMovementSync.RealModelPosition;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        Console.AddDebugLog("SCPCTRL", "SCP - 049 | Request 'start recalling' rejected; Distance was too great.", MessageImportance.LessImportant);
                        return false;
                    }
                    var ev = new StartRecallEvent(Player.Get(__instance.Hub.gameObject), Player.Get(component2.Info.OwnerHub));
                    Qurre.Events.Invoke.Scp049.StartRecall(ev);
                    if (!ev.Allowed) return false;
                    Console.AddDebugLog("SCPCTRL", "SCP-049 | Request 'start recalling' accepted", MessageImportance.LessImportant);
                    __instance._recallHubServer = component2.Info.OwnerHub;
                    __instance._recallProgressServer = 0f;
                    __instance._recallInProgressServer = true;
                    return false;
                }
                else if (num == 0)
                {
                    try
                    {
                        bool boolean = __instance._interactRateLimit.CanExecute() && !(go == null) &&
                            !(__instance.RemainingServerKillCooldown > 0f) &&
                            !(Vector3.Distance(go.transform.position, __instance.Hub.playerMovementSync.RealModelPosition) >=
                            PlayableScps.Scp049.AttackDistance * 1.25f) && !Physics.Linecast(__instance.Hub.playerMovementSync.RealModelPosition,
                            go.transform.position, MicroHIDItem.WallMask);
                        if (!boolean) return false;
                        Player scp = Player.Get(__instance.Hub);
                        Player target = Player.Get(go);
                        if (target is null) return false;
                        var ev = new ScpAttackEvent(scp, target, ScpAttackType.Scp049);
                        Qurre.Events.Invoke.Player.ScpAttack(ev);
                        if (!ev.Allowed) return false;
                        target.DealDamage(new ScpDamageHandler(scp.ReferenceHub, DeathTranslations.Scp049));
                        Console.AddDebugLog("SCPCTRL", "SCP-049 | Sent 'death time' RPC", MessageImportance.LessImportant);
                        __instance.Hub.scpsController.RpcTransmit_Byte(0);
                        __instance.RemainingServerKillCooldown = PlayableScps.Scp049.KillCooldown;
                        return false;
                    }
                    catch (System.Exception e)
                    {
                        Log.Error($"umm, error in patching SCPs -> SCP049 [Attack]:\n{e}\n{e.StackTrace}");
                        return true;
                    }
                }
                else return true;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP049 [StartAndFinishRecall]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}