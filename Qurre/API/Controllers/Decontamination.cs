using LightContainmentZoneDecontamination;
using static QurreModLoader.umm;
namespace Qurre.API.Controllers
{
    public static class Decontamination
    {
        public static DecontaminationController Controller => DecontaminationController.Singleton;
        public static bool DisableDecontamination
        {
            get => DecontaminationController.Singleton.disableDecontamination;
            set => DecontaminationController.Singleton.disableDecontamination = value;
        }
        public static bool Locked { get => Controller.DCLocked(); set => Controller.DCLocked(value); }
        public static bool InProgress => Controller.DCBegun();
        public static void InstantStart() => Controller.FinishDecontamination();
    }
}