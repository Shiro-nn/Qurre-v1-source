#pragma warning disable SA1313
using HarmonyLib;
using Mirror;
using UnityEngine;
using Qurre.API.Events;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.SCPs.SCP049
{
    [HarmonyPatch(typeof(PlayableScps.Scp049), nameof(PlayableScps.Scp049.BodyCmd_ByteAndGameObject))]
    internal static class StartAndFinishRecall
    {
        private static bool Prefix(PlayableScps.Scp049 __instance, byte num, GameObject go)
        {
            try
            {
                if (num == 2)
                {
                    if (!__instance.RateLimit().CanExecute() || go == null)
                        return false;
                    Ragdoll rgd = go.GetComponent<Ragdoll>();
                    if (rgd == null)
                        return false;
                    ReferenceHub target = null;
                    foreach (GameObject player in PlayerManager.players)
                    {
                        ReferenceHub hub = ReferenceHub.GetHub(player);
                        if (hub.queryProcessor.PlayerId == rgd.owner.PlayerId)
                        {
                            target = hub;
                            break;
                        }
                    }
                    if (target == null)
                        return false;
                    if (!__instance.Scp049_recallInProgressServer() ||
                        target.gameObject != __instance.Scp049_recallObjectServer() ||
                        __instance.Scp049_recallProgressServer() < 0.85f)
                        return false;
                    if (target.characterClassManager.CurClass != RoleType.Spectator)
                        return false;
                    var ev = new FinishRecallEvent(API.Player.Get(__instance.Hub.gameObject), API.Player.Get(target.gameObject));
                    Qurre.Events.SCPs.SCP049.finishrecall(ev);
                    if (!ev.Allowed) return false;
                    RoundSummary.changed_into_zombies++;
                    target.characterClassManager.SetClassID(RoleType.Scp0492);
                    target.GetComponent<PlayerStats>().Health =
                        target.characterClassManager.Classes.Get(RoleType.Scp0492).maxHP;
                    if (rgd.CompareTag("Ragdoll"))
                        NetworkServer.Destroy(rgd.gameObject);
                    __instance.Scp049_recallInProgressServer(false);
                    __instance.Scp049_recallObjectServer(null);
                    __instance.Scp049_recallProgressServer(0f);
                    return false;
                }
                if (num != 1)
                    return true;
                {
                    if (!__instance.RateLimit().CanExecute())
                        return false;
                    if (go == null)
                        return false;
                    Ragdoll telo = go.GetComponent<Ragdoll>();
                    if (telo == null)
                        return false;
                    if (!telo.allowRecall)
                        return false;
                    ReferenceHub target = null;
                    foreach (GameObject player in PlayerManager.players)
                    {
                        ReferenceHub hub = ReferenceHub.GetHub(player);
                        if (hub != null && hub.queryProcessor.PlayerId == telo.owner.PlayerId)
                        {
                            target = hub;
                            break;
                        }
                    }
                    if (target == null)
                        return false;
                    if (Vector3.Distance(telo.transform.position, __instance.Hub.PlayerCameraReference.transform.position) >=
                        PlayableScps.Scp049.ReviveDistance * 1.3f)
                        return false;
                    var ev = new StartRecallEvent(API.Player.Get(__instance.Hub.gameObject), API.Player.Get(target.gameObject));
                    Qurre.Events.SCPs.SCP049.startrecall(ev);
                    if (!ev.Allowed) return false;
                    __instance.Scp049_recallObjectServer(target.gameObject);
                    __instance.Scp049_recallProgressServer(0f);
                    __instance.Scp049_recallInProgressServer(true);
                    return false;
                }
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs.SCP049.StartAndFinishRecall:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}