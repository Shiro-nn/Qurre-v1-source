using HarmonyLib;
using LightContainmentZoneDecontamination;
using Qurre.API.Events;
namespace Qurre.Patches.Events.Map
{
    [HarmonyPatch(typeof(DecontaminationController), nameof(DecontaminationController.FinishDecontamination))]
    internal static class LczDecontamination
    {
        private static bool Prefix()
        {
            var ev = new LczDeconEvent();
            Qurre.Events.Invoke.Map.LczDecon(ev);
            return ev.Allowed;
        }
    }
}