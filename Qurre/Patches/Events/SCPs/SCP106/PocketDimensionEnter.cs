using System;
using System.Linq;
using CustomPlayerEffects;
using HarmonyLib;
using RemoteAdmin;
using UnityEngine;
using Qurre.API.Events;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.SCPs.SCP106
{
    [HarmonyPatch(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.CallCmdMovePlayer))]
    internal static class PocketDimensionEnter
    {
        private static bool Prefix(Scp106PlayerScript __instance, GameObject ply, int t)
        {
            try
            {
                if (!__instance.Scp106Script_iawRateLimit().CanExecute(true))
                    return false;
                if (ply == null)
                    return false;
                ReferenceHub hub = ReferenceHub.GetHub(ply);
                CharacterClassManager CCM = hub.characterClassManager;
                if (CCM == null)
                    return false;
                if (!ServerTime.CheckSynchronization(t) || !__instance.iAm106 ||
                    Vector3.Distance(__instance.GetComponent<PlayerMovementSync>().RealModelPosition, ply.transform.position) >= 3f || !CCM.IsHuman())
                    return false;
                if (CCM.GodMode)
                    return false;
                if (CCM.Classes.SafeGet(CCM.CurClass).team == Team.SCP)
                    return false;
                __instance.GetComponent<CharacterClassManager>().RpcPlaceBlood(ply.transform.position, 1, 2f);
                if (Scp106Script_blastDoor().isClosed)
                {
                    __instance.GetComponent<CharacterClassManager>().RpcPlaceBlood(ply.transform.position, 1, 2f);
                    __instance.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(500f,
                        __instance.GetComponent<NicknameSync>().MyNick + " (" + __instance.GetComponent<CharacterClassManager>().UserId + ")",
                        DamageTypes.Scp106, __instance.GetComponent<QueryProcessor>().PlayerId), ply);
                }
                else
                {
                    CharacterClassManager ccm = ply.GetComponent<CharacterClassManager>();
                    foreach (Scp079PlayerScript script in Scp079PlayerScript.instances)
                    {
                        Scp079Interactable.ZoneAndRoom room = ply.GetComponent<Scp079PlayerScript>().GetOtherRoom();
                        Scp079Interactable.InteractableType[] filter = new Scp079Interactable.InteractableType[]
                        {
                            Scp079Interactable.InteractableType.Door, Scp079Interactable.InteractableType.Light,
                            Scp079Interactable.InteractableType.Lockdown, Scp079Interactable.InteractableType.Tesla,
                            Scp079Interactable.InteractableType.ElevatorUse,
                        };
                        bool boolean = false;
                        foreach (Scp079Interaction SI in script.ReturnRecentHistory(12f, filter))
                            foreach (Scp079Interactable.ZoneAndRoom zone in SI.interactable.currentZonesAndRooms)
                                if (zone.currentZone == room.currentZone &&
                                    zone.currentRoom == room.currentRoom)
                                    boolean = true;
                        if (boolean)
                            script.RpcGainExp(ExpGainType.PocketAssist, ccm.CurClass);
                    }
                    var ev = new PocketDimensionEnterEvent(API.Player.Get(ply), Vector3.down * 1998.5f);
                    Qurre.Events.Invoke.Scp106.PocketDimensionEnter(ev);
                    if (!ev.Allowed) return false;
                    ply.GetComponent<PlayerMovementSync>().OverridePosition(ev.Position, 0f, true);
                    __instance.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(40f,
                        __instance.GetComponent<NicknameSync>().MyNick + " (" + __instance.GetComponent<CharacterClassManager>().UserId + ")",
                        DamageTypes.Scp106, __instance.GetComponent<QueryProcessor>().PlayerId), ply);
                }
                var pl = API.Player.Get(hub);
                foreach (var larry in API.Player.List.Where(x => !x.Scp106Controller.PocketPlayers.Contains(pl)))
                    larry.Scp106Controller.PocketPlayers.Remove(pl);
                PlayerEffectsController effects = hub.playerEffectsController;
                effects.GetEffect<Corroding>().IsInPd = true;
                effects.EnableEffect<Corroding>(0.0f, false);
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP106 [PocketDimensionEnter]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}