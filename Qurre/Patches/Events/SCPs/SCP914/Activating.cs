#pragma warning disable SA1118
using HarmonyLib;
using Mirror;
using Scp914;
using Qurre.API.Events;
namespace Qurre.Patches.Events.SCPs.SCP914
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdUse914))]
    internal static class Activating
    {
        private static bool Prefix(PlayerInteract __instance)
        {
            try
            {
                if (!QurreModLoader.umm.RateLimit(__instance).CanExecute(true) ||
                        (QurreModLoader.umm.InteractCuff(__instance).CufferId > 0 && !QurreModLoader.umm.DisarmedInteract()) ||
                        (Scp914Machine.singleton.working || !__instance.ChckDis(Scp914Machine.singleton.button.position)))
                    return false;
                var ev = new ActivatingEvent(API.Player.Get(__instance.gameObject), 0);
                Qurre.Events.SCPs.SCP914.activating(ev);
                if (ev.Allowed)
                {
                    Scp914Machine.singleton.RpcActivate(NetworkTime.time + ev.Duration);
                    __instance.OnInteract();
                }
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP914 [Activating]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}