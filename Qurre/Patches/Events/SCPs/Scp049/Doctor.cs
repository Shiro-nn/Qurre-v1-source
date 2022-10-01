using HarmonyLib;
using Mirror;
using UnityEngine;
using Qurre.API.Events;
using InventorySystem.Items.MicroHID;
using Qurre.API.Objects;
using PlayerStatsSystem;
namespace Qurre.Patches.Events.SCPs.Scp049
{
    using Qurre.API;
    [HarmonyPatch(typeof(PlayableScps.Scp049), nameof(PlayableScps.Scp049.BodyCmd_ByteAndGameObject))]
    internal static class Doctor
    {
        private static bool Prefix(PlayableScps.Scp049 __instance, byte num, GameObject go)
        {
            try
            {
                switch (num)
                {
                    case 0:
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
                    case 1:
                        {
                            if (!__instance._interactRateLimit.CanExecute() || go is null) return false;
                            Ragdoll component = go.GetComponent<Ragdoll>();
                            if (component is null) return false;
                            if (component.Info.ExistenceTime > PlayableScps.Scp049.ReviveEligibilityDuration) return false;
                            if (!API.Map.Ragdolls.TryFind(out var ragdl, x => x.ragdoll == component)) return false;
                            if (ragdl.Owner is null) return false;
                            bool flag = false;
                            Rigidbody[] componentsInChildren = component.GetComponentsInChildren<Rigidbody>();
                            for (int i = 0; i < componentsInChildren.Length; i++)
                            {
                                if (Vector3.Distance(componentsInChildren[i].transform.position,
                                    __instance.Hub.PlayerCameraReference.transform.position) <= PlayableScps.Scp049.ReviveDistance * 1.3f)
                                {
                                    flag = true;
                                    ragdl.Owner.ClassManager.DeathPosition = __instance.Hub.playerMovementSync.RealModelPosition;
                                    break;
                                }
                            }
                            if (!flag) return false;
                            var ev = new StartRecallEvent(Player.Get(__instance.Hub.gameObject), ragdl.Owner, ragdl);
                            Qurre.Events.Invoke.Scp049.StartRecall(ev);
                            if (!ev.Allowed) return false;
                            __instance._recallHubServer = ragdl.Owner.ReferenceHub;
                            __instance._recallProgressServer = 0f;
                            __instance._recallInProgressServer = true;
                            return false;
                        }
                    case 2:
                        {
                            if (!__instance._interactRateLimit.CanExecute() || go is null)
                                return false;
                            Ragdoll component = go.GetComponent<Ragdoll>();
                            if (component is null) return false;
                            if (!API.Map.Ragdolls.TryFind(out var ragdl, x => x.ragdoll == component)) return false;
                            Player owner = ragdl.Owner;
                            if (owner is null) return false;
                            if (!__instance._recallInProgressServer || owner.ReferenceHub != __instance._recallHubServer || __instance._recallProgressServer < 0.85f)
                                return false;
                            if (owner.Role != RoleType.Spectator) return false;
                            var ev = new FinishRecallEvent(Player.Get(__instance.Hub.gameObject), owner);
                            Qurre.Events.Invoke.Scp049.FinishRecall(ev);
                            if (!ev.Allowed) return false;
                            owner.SetRole(RoleType.Scp0492, reason: CharacterClassManager.SpawnReason.Revived);
                            if (component.CompareTag("Ragdoll")) NetworkServer.Destroy(component.gameObject);
                            __instance._recallInProgressServer = false;
                            __instance._recallHubServer = null;
                            __instance._recallProgressServer = 0f;
                            return false;
                        }
                    default: return true;
                }
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP049 [StartAndFinishRecall]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}