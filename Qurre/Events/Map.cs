using Qurre.API.Events;
using Qurre.Events.modules;
using static Qurre.Events.modules.Main;
namespace Qurre.Events
{
    public static class Map
    {
        public static event AllEvents<LCZDeconEvent> LCZDecon;
        public static event AllEvents<AnnouncementDecontaminationEvent> AnnouncementDecontaminationZDecon;
        public static event AllEvents<MTFAnnouncementEvent> MTFAnnouncement;
        public static event AllEvents<NewBloodEvent> NewBlood;
        public static event AllEvents<NewDecalEvent> NewDecal;
        public static event AllEvents Generated;
        public static event AllEvents<SetSeedEvent> SetSeed;
        public static event AllEvents<DoorDamageEvent> DoorDamage;
        public static event AllEvents<DoorLockEvent> DoorLock;
        public static event AllEvents<DoorOpenEvent> DoorOpen;
        public static event AllEvents<UseLiftEvent> UseLift;
        internal static void Invokes(LCZDeconEvent ev) => LCZDecon?.invoke(ev);
        internal static void Invokes(AnnouncementDecontaminationEvent ev) => AnnouncementDecontaminationZDecon?.invoke(ev);
        internal static void Invokes(MTFAnnouncementEvent ev) => MTFAnnouncement?.invoke(ev);
        internal static void Invokes(NewBloodEvent ev) => NewBlood?.invoke(ev);
        internal static void Invokes(NewDecalEvent ev) => NewDecal?.invoke(ev);
        internal static void Invokes() => Generated.invoke();
        internal static void Invokes(SetSeedEvent ev) => SetSeed?.invoke(ev);
        internal static void Invokes(DoorDamageEvent ev) => DoorDamage?.invoke(ev);
        internal static void Invokes(DoorLockEvent ev) => DoorLock?.invoke(ev);
        internal static void Invokes(DoorOpenEvent ev) => DoorOpen?.invoke(ev);
        internal static void Invokes(UseLiftEvent ev) => UseLift?.invoke(ev);
    }
}