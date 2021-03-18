using LightContainmentZoneDecontamination;
using static QurreModLoader.umm;
namespace Qurre.API.Controllers
{
    public class Decontamination
    {
        internal Decontamination() { }
        public DecontaminationController Controller => DecontaminationController.Singleton;
        public bool DisableDecontamination
        {
            get => DecontaminationController.Singleton.disableDecontamination;
            set => DecontaminationController.Singleton.disableDecontamination = value;
        }
        public bool Locked { get => Controller.DCLocked(); set => Controller.DCLocked(value); }
        public bool InProgress => Controller.DCBegun();
        public void InstantStart() => Controller.FinishDecontamination();
    }
}