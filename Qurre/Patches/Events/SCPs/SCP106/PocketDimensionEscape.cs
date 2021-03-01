#pragma warning disable SA1118
using System.Collections.Generic;
using HarmonyLib;
using LightContainmentZoneDecontamination;
using Mirror;
using UnityEngine;
using static Qurre.API.Events.SCP106;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.SCPs.SCP106
{
    [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.OnTriggerEnter))]
    internal static class PocketDimensionEscape
    {
        private static bool Prefix(PocketDimensionTeleport __instance, Collider other)
        {
            try
            {
                if (!NetworkServer.active) return false;

                var component = other.GetComponent<NetworkIdentity>();
                if (component == null) return false;
                var type = __instance.Pocket_type();
                var player = API.Player.Get(component?.gameObject);
                if (player == null) return false;
                if (type == PocketDimensionTeleport.PDTeleportType.Killer || BlastDoor.OneDoor.isClosed)
                {
                    if (type == PocketDimensionTeleport.PDTeleportType.Killer)
                    {
                        var ev = new PocketDimensionFailEscapeEvent(API.Player.Get(other.gameObject), __instance);
                        Qurre.Events.SCPs.SCP106.pocketdimensionfailescape(ev);
                        if (!ev.IsAllowed) return false;
                    }
                    player.Damage(9999, DamageTypes.Pocket);
                }
                else
                {
                    List<Vector3> tp = new List<Vector3>();
                    bool flag = false;
                    DecontaminationController.DecontaminationPhase[] dP = DecontaminationController.Singleton.DecontaminationPhases;
                    if (DecontaminationController.GetServerTime > dP[dP.Length - 2].TimeTrigger) flag = true;
                    var _cfg = GameCore.ConfigFile.ServerConfig.GetStringList(flag ? "pd_random_exit_rids_after_decontamination" : "pd_random_exit_rids");
                    foreach (GameObject gO in GameObject.FindGameObjectsWithTag("RoomID"))
                    {
                        var _rid = gO.GetComponent<Rid>();
                        if (_rid != null && _cfg.Contains(_rid.id)) tp.Add(gO.transform.position);
                    }
                    if (_cfg.Contains("PORTAL"))
                        foreach (Scp106PlayerScript _script in UnityEngine.Object.FindObjectsOfType<Scp106PlayerScript>())
                            if (_script.portalPosition != Vector3.zero)
                                tp.Add(_script.portalPosition);
                    if (tp == null || tp.Count == 0)
                        foreach (GameObject _go in GameObject.FindGameObjectsWithTag("PD_EXIT"))
                            tp.Add(_go.transform.position);
                    var pos = tp[UnityEngine.Random.Range(0, tp.Count)];
                    pos.y += 2f;
                    if (player.Team == Team.SCP) type = PocketDimensionTeleport.PDTeleportType.Exit;
                    var ev = new PocketDimensionEscapeEvent(player, pos);
                    Qurre.Events.SCPs.SCP106.pocketdimensionescape(ev);
                    if (!ev.IsAllowed) return false;
                    player.ReferenceHub.playerMovementSync.AddSafeTime(2f, false);
                    player.Position = pos;
                    __instance.RemoveCorrosionEffect(player.GameObject);
                    PlayerManager.localPlayer.GetComponent<PlayerStats>().TargetAchieve(component.connectionToClient, "larryisyourfriend");
                }
                if (PocketDimensionTeleport.RefreshExit) MapGeneration.ImageGenerator.pocketDimensionGenerator.GenerateRandom();
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs.SCP106.PocketDimensionEscape:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}