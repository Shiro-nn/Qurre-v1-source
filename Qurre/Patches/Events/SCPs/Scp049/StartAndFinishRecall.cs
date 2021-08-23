using HarmonyLib;
using Mirror;
using UnityEngine;
using Qurre.API.Events;
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
                    if (!__instance._interactRateLimit.CanExecute() || go == null)
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
                    if (!__instance._recallInProgressServer ||
                        target.gameObject != __instance._recallObjectServer ||
                        __instance._recallProgressServer < 0.85f)
                        return false;
                    if (target.characterClassManager.CurClass != RoleType.Spectator)
                        return false;
                    var ev = new FinishRecallEvent(API.Player.Get(__instance.Hub.gameObject), API.Player.Get(target.gameObject));
                    Qurre.Events.Invoke.Scp049.FinishRecall(ev);
                    if (!ev.Allowed) return false;
                    RoundSummary.changed_into_zombies++;
                    target.characterClassManager.SetClassID(RoleType.Scp0492, CharacterClassManager.SpawnReason.Revived);
                    target.GetComponent<PlayerStats>().Health =
                        target.characterClassManager.Classes.Get(RoleType.Scp0492).maxHP;
                    if (rgd.CompareTag("Ragdoll"))
                        NetworkServer.Destroy(rgd.gameObject);
                    __instance._recallInProgressServer = false;
                    __instance._recallObjectServer = null;
                    __instance._recallProgressServer = 0f;
                    return false;
                }
                if (num != 1)
                    return true;
                {
                    if (!__instance._interactRateLimit.CanExecute())
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
                    Qurre.Events.Invoke.Scp049.StartRecall(ev);
                    if (!ev.Allowed) return false;
                    __instance._recallObjectServer = target.gameObject;
                    __instance._recallProgressServer = 0f;
                    __instance._recallInProgressServer = true;
                    return false;
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