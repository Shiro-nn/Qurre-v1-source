#pragma warning disable SA1118
using System.Collections.Generic;
using GameCore;
using HarmonyLib;
using LightContainmentZoneDecontamination;
using Mirror;
using UnityEngine;
using static Qurre.API.Events.SCP106;
namespace Qurre.Patches.Events.SCPs.SCP106
{
    [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.OnTriggerEnter))]
    internal static class PocketDimensionEscape
    {
        private static bool Prefix(PocketDimensionTeleport __instance, Collider other)
        {
            try
            {
                NetworkIdentity NI = other.GetComponent<NetworkIdentity>();
                if (!(NI != null))
                    return false;
                if (QurreModLoader.umm.Pocket_type(__instance) == PocketDimensionTeleport.PDTeleportType.Killer || BlastDoor.OneDoor.isClosed)
                {
                    if (QurreModLoader.umm.Pocket_type(__instance) == PocketDimensionTeleport.PDTeleportType.Killer)
                    {
                        var ev = new PocketDimensionFailEscapeEvent(ReferenceHub.GetHub(other.gameObject), __instance);
                        Qurre.Events.SCPs.SCP106.pocketdimensionfailescape(ev);
                        if (!ev.IsAllowed)
                            return false;
                    }
                    NI.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(999990f, "WORLD", DamageTypes.Pocket, 0), other.gameObject);
                }
                else if (QurreModLoader.umm.Pocket_type(__instance) == PocketDimensionTeleport.PDTeleportType.Exit)
                {
                    QurreModLoader.umm.Pocket_tp(__instance).Clear();
                    bool flag = false;
                    DecontaminationController.DecontaminationPhase[] dP = DecontaminationController.Singleton.DecontaminationPhases;
                    if (DecontaminationController.GetServerTime > dP[dP.Length - 2].TimeTrigger)
                        flag = true;
                    List<string> cfg = ConfigFile.ServerConfig.GetStringList(flag ? "pd_random_exit_rids_after_decontamination" : "pd_random_exit_rids");
                    if (cfg.Count > 0)
                    {
                        foreach (GameObject gO in GameObject.FindGameObjectsWithTag("RoomID"))
                        {
                            if (gO.GetComponent<Rid>() != null && cfg.Contains(gO.GetComponent<Rid>().id))
                                QurreModLoader.umm.Pocket_tp(__instance).Add(gO.transform.position);
                        }
                        if (cfg.Contains("PORTAL"))
                        {
                            foreach (Scp106PlayerScript script in Object.FindObjectsOfType<Scp106PlayerScript>())
                            {
                                if (script.portalPosition != Vector3.zero)
                                    QurreModLoader.umm.Pocket_tp(__instance).Add(script.portalPosition);
                            }
                        }
                    }
                    if (QurreModLoader.umm.Pocket_tp(__instance) == null || QurreModLoader.umm.Pocket_tp(__instance).Count == 0)
                    {
                        foreach (GameObject gO in GameObject.FindGameObjectsWithTag("PD_EXIT"))
                            QurreModLoader.umm.Pocket_tp(__instance).Add(gO.transform.position);
                    }
                    Vector3 vec = QurreModLoader.umm.Pocket_tp(__instance)[Random.Range(0, QurreModLoader.umm.Pocket_tp(__instance).Count)];
                    vec.y += 2f;
                    PlayerMovementSync PMS = other.GetComponent<PlayerMovementSync>();
                    PMS.AddSafeTime(2f);
                    var ev = new PocketDimensionEscapeEvent(ReferenceHub.GetHub(PMS.gameObject), vec);
                    Qurre.Events.SCPs.SCP106.pocketdimensionescape(ev);
                    PMS.OverridePosition(vec, 0.0f, false);
                    __instance.RemoveCorrosionEffect(other.gameObject);
                    PlayerManager.localPlayer.GetComponent<PlayerStats>().TargetAchieve(NI.connectionToClient, "larryisyourfriend");
                }
                if (!PocketDimensionTeleport.RefreshExit)
                    return false;
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