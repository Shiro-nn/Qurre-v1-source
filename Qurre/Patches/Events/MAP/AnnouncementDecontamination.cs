#pragma warning disable SA1313
using HarmonyLib;
using LightContainmentZoneDecontamination;
using Qurre.API.Events;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.MAP
{
    [HarmonyPatch(typeof(DecontaminationController), nameof(DecontaminationController.UpdateSpeaker))]
    internal static class AnnouncementDecontamination
    {
        private static bool Prefix(DecontaminationController __instance, ref bool hard)
        {
            try
            {
                var ev = new AnnouncementDecontaminationEvent(QurreModLoader.umm.DC_nextPhase(__instance), hard);
                Qurre.Events.Map.announcementdecontamination(ev);
                hard = ev.IsGlobal;
                __instance.Set_DC_nextPhase(ev.Id);
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching MAP.AnnouncementDecontamination:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}