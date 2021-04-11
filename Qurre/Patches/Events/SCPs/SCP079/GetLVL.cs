using HarmonyLib;
using Qurre.API.Events;
namespace Qurre.Patches.Events.SCPs.SCP079
{
    [HarmonyPatch(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.TargetLevelChanged))]
    internal static class GetLVL
    {
        private static bool Prefix(Scp079PlayerScript __instance, ref int newLvl)
        {
            try
            {
                var ev = new GetLVLEvent(API.Player.Get(__instance.gameObject), __instance.NetworkcurLvl - 1, newLvl);
                Qurre.Events.SCPs.SCP079.getLVL(ev);
                newLvl = ev.NewLevel;
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP079 [GetLVL]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}