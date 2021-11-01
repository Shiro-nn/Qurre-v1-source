using HarmonyLib;
using LightContainmentZoneDecontamination;
using Qurre.API.Events;
namespace Qurre.Patches.Events.Map
{
    [HarmonyPatch(typeof(DecontaminationController), nameof(DecontaminationController.UpdateSpeaker))]
    internal static class LczAnnounce
    {
        private static void Prefix(DecontaminationController __instance, ref bool hard)
        {
            try
            {
                var ev = new LczAnnounceEvent(__instance._nextPhase, hard);
                Qurre.Events.Invoke.Map.LczAnnounce(ev);
                hard = ev.IsGlobal;
                __instance._nextPhase = ev.Id;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Map [LczAnnounce]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}