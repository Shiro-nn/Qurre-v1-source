using HarmonyLib;
using LightContainmentZoneDecontamination;
using Qurre.API.Events;
namespace Qurre.Patches.Events.Map
{
    [HarmonyPatch(typeof(DecontaminationController), nameof(DecontaminationController.FinishDecontamination))]
    internal static class LCZDecontamination
    {
        private static bool Prefix()
        {
            var ev = new LCZDeconEvent();
            Qurre.Events.Invoke.Map.LCZDecon(ev);
            return ev.Allowed;
        }
    }
}