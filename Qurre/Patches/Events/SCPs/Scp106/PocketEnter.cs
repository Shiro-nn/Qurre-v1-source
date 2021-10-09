using System;
using HarmonyLib;
using UnityEngine;
using Qurre.API.Events;
using Qurre.API;
using System.Collections.Generic;
using Qurre.API.Objects;
namespace Qurre.Patches.Events.SCPs.Scp106
{
    [HarmonyPatch(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.UserCode_CmdMovePlayer))]
    internal static class PocketEnter
    {
        private static bool Prefix(Scp106PlayerScript __instance, GameObject ply, int t)
        {
            try
            {
                if (!__instance._iawRateLimit.CanExecute(true) || !__instance.iAm106 || !ServerTime.CheckSynchronization(t)) return false;
                if (ply == null) return false;
                Player pl = Player.Get(ply);
                CharacterClassManager characterClassManager = pl.ClassManager;
                if (characterClassManager == null || characterClassManager.GodMode || !characterClassManager.IsHuman()) return false;
                Vector3 position = ply.transform.position;
                float num = Vector3.Distance(__instance._hub.playerMovementSync.RealModelPosition, position);
                float num2 = Math.Abs(__instance._hub.playerMovementSync.RealModelPosition.y - position.y);
                if ((num >= 3.1f && num2 < 1.02f) || (num >= 3.4f && num2 < 1.95f) || (num >= 3.7f && num2 < 2.2f) || (num >= 3.9f && num2 < 3f) || num >= 4.2f)
                {
                    __instance._hub.characterClassManager.TargetConsolePrint(__instance.connectionToClient, string.Format("106 MovePlayer command rejected - too big distance (code: T.1). Distance: {0}, Y Diff: {1}.", num, num2), "gray");
                    return false;
                }
                if (Physics.Linecast(__instance._hub.playerMovementSync.RealModelPosition, ply.transform.position, InventorySystem.Items.MicroHID.MicroHIDItem.WallMask))
                {
                    __instance._hub.characterClassManager.TargetConsolePrint(__instance.connectionToClient, string.Format("106 MovePlayer command rejected - collider found between you and the target (code: T.2). Distance: {0}, Y Diff: {1}.", num, num2), "gray");
                    return false;
                }
                var ev = new PocketEnterEvent(pl, Vector3.down * 1998.5f);
                Qurre.Events.Invoke.Scp106.PocketEnter(ev);
                if (!ev.Allowed) return false;
                __instance._hub.characterClassManager.RpcPlaceBlood(ply.transform.position, 1, 2f);
                __instance.TargetHitMarker(__instance.connectionToClient, __instance.captureCooldown);
                __instance._currentServerCooldown = __instance.captureCooldown;
                if (Scp106PlayerScript._blastDoor.isClosed)
                {
                    __instance._hub.characterClassManager.RpcPlaceBlood(ply.transform.position, 1, 2f);
                    __instance._hub.playerStats.HurtPlayer(new PlayerStats.HitInfo(500f, null, DamageTypes.Scp106, __instance._hub.playerId, false), ply, false, true);
                }
                else
                {
                    __instance._hub.playerStats.HurtPlayer(new PlayerStats.HitInfo(40f, null, DamageTypes.Scp106, __instance._hub.playerId, false), ply, false, true);
                    pl.Position = ev.Position;
                    foreach (Scp079PlayerScript scp079PlayerScript in Scp079PlayerScript.instances)
                    {
                        Scp079Interactable.InteractableType[] filter = new Scp079Interactable.InteractableType[]
                        {
                    Scp079Interactable.InteractableType.Door,
                    Scp079Interactable.InteractableType.Light,
                    Scp079Interactable.InteractableType.Lockdown,
                    Scp079Interactable.InteractableType.Tesla,
                    Scp079Interactable.InteractableType.ElevatorUse
                        };
                        bool flag = false;
                        using (IEnumerator<Scp079Interaction> enumerator2 = scp079PlayerScript.ReturnRecentHistory(12f, filter).GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                if (MapGeneration.RoomIdUtils.IsTheSameRoom(enumerator2.Current.interactable.transform.position, ply.transform.position))
                                {
                                    flag = true;
                                }
                            }
                        }
                        if (flag)
                        {
                            scp079PlayerScript.RpcGainExp(ExpGainType.PocketAssist, characterClassManager.CurClass);
                        }
                    }
                }
                pl.EnableEffect(EffectType.Corroding, 0f, false);
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP106 [PocketEnter]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}