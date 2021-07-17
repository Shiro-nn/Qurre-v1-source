using Qurre.API.Events;
using static Qurre.Events.Map;
namespace Qurre.Events.Invoke
{
    public static class Map
    {
        public static void LCZDecon(LCZDeconEvent ev) => Invokes(ev);
        public static void AnnouncementDecontaminationZDecon(AnnouncementDecontaminationEvent ev) => Invokes(ev);
        public static void MTFAnnouncement(MTFAnnouncementEvent ev) => Invokes(ev);
        public static void NewBlood(NewBloodEvent ev) => Invokes(ev);
        public static void NewDecal(NewDecalEvent ev) => Invokes(ev);
        public static void Generated() => Invokes();
        public static void SetSeed(SetSeedEvent ev) => Invokes(ev);
        public static void DoorDamage(DoorDamageEvent ev) => Invokes(ev);
        public static void DoorLock(DoorLockEvent ev) => Invokes(ev);
        public static void DoorOpen(DoorOpenEvent ev) => Invokes(ev);
        public static void UseLift(UseLiftEvent ev) => Invokes(ev);
    }
}