#pragma warning disable SA1118
using System.Collections.Generic;
using GameCore;
using HarmonyLib;
using LightContainmentZoneDecontamination;
using MapGeneration;
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
                NetworkIdentity NI = other.GetComponent<NetworkIdentity>();
                if (NI == null) return false;
                if (__instance.Pocket_type() == PocketDimensionTeleport.PDTeleportType.Killer || BlastDoor.OneDoor.isClosed)
                {
                    if (__instance.Pocket_type() == PocketDimensionTeleport.PDTeleportType.Killer)
                    {
                        var ev = new PocketDimensionFailEscapeEvent(API.Player.Get(other.gameObject), __instance);
                        Qurre.Events.SCPs.SCP106.pocketdimensionfailescape(ev);
                        if (!ev.IsAllowed)
                            return false;
                    }
                    NI.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(999990f, "WORLD", DamageTypes.Pocket, 0), other.gameObject);
                }
                else if (__instance.Pocket_type() == PocketDimensionTeleport.PDTeleportType.Exit)
                {
                    __instance.Pocket_tp()?.Clear();
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
                                __instance.Pocket_tp()?.Add(gO.transform.position);
                        }
                        if (cfg.Contains("PORTAL"))
                        {
                            foreach (Scp106PlayerScript script in Object.FindObjectsOfType<Scp106PlayerScript>())
                            {
                                if (script.portalPosition != Vector3.zero)
                                    __instance.Pocket_tp()?.Add(script.portalPosition);
                            }
                        }
                    }
                    if (__instance.Pocket_tp() == null || __instance.Pocket_tp()?.Count == 0)
                    {
                        foreach (GameObject gO in GameObject.FindGameObjectsWithTag("PD_EXIT"))
                            __instance.Pocket_tp()?.Add(gO.transform.position);
                    }
                    Vector3 vec = __instance.Pocket_tp()[Random.Range(0, __instance.Pocket_tp().Count)];
                    vec.y += 2f;
                    PlayerMovementSync PMS = other.GetComponent<PlayerMovementSync>();
                    PMS.AddSafeTime(2f);
                    var ev = new PocketDimensionEscapeEvent(API.Player.Get(PMS.gameObject), vec);
                    Qurre.Events.SCPs.SCP106.pocketdimensionescape(ev);
                    PMS.OverridePosition(vec, 0.0f, false);
                    __instance.RemoveCorrosionEffect(other.gameObject);
                    PlayerManager.localPlayer.GetComponent<PlayerStats>().TargetAchieve(NI.connectionToClient, "larryisyourfriend");
                }
                if (!PocketDimensionTeleport.RefreshExit)
                    return false;
                ImageGenerator.pocketDimensionGenerator.GenerateRandom();
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