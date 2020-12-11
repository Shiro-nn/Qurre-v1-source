#pragma warning disable SA1313
using HarmonyLib;
using LightContainmentZoneDecontamination;
using Qurre.API.Events;

namespace Qurre.Patches.Events.MAP
{
    [HarmonyPatch(typeof(DecontaminationController), nameof(DecontaminationController.FinishDecontamination))]
    internal static class LCZDecontamination
    {
        private static bool Prefix()
        {
            var ev = new LczDeconEvent();
            Qurre.Events.Map.lczdecon(ev);
            return ev.IsAllowed;
        }
    }
}