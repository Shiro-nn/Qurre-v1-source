using Qurre.API.Events;
using Qurre.Events.Modules;
using static Qurre.Events.Modules.Main;
namespace Qurre.Events
{
    public static class Map
    {
        public static event AllEvents<LczDeconEvent> LczDecon;
        public static event AllEvents<LczAnnounceEvent> LczAnnounce;
        public static event AllEvents<MTFAnnouncementEvent> MTFAnnouncement;
        public static event AllEvents<NewBloodEvent> NewBlood;
        public static event AllEvents<PlaceBulletHoleEvent> PlaceBulletHole;
        public static event AllEvents Generated;
        public static event AllEvents<SetSeedEvent> SetSeed;
        public static event AllEvents<DoorDamageEvent> DoorDamage;
        public static event AllEvents<DoorLockEvent> DoorLock;
        public static event AllEvents<DoorOpenEvent> DoorOpen;
        public static event AllEvents<UseLiftEvent> UseLift;
        public static event AllEvents<ScpDeadAnnouncementEvent> ScpDeadAnnouncement;
        public static event AllEvents<CreatePickupEvent> CreatePickup;
        public static event AllEvents<ConvertUnitNameEvent> ConvertUnitName;
        internal static void Invokes(LczDeconEvent ev) => LczDecon?.invoke(ev);
        internal static void Invokes(LczAnnounceEvent ev) => LczAnnounce?.invoke(ev);
        internal static void Invokes(MTFAnnouncementEvent ev) => MTFAnnouncement?.invoke(ev);
        internal static void Invokes(NewBloodEvent ev) => NewBlood?.invoke(ev);
        internal static void Invokes(PlaceBulletHoleEvent ev) => PlaceBulletHole?.invoke(ev);
        internal static void Invokes() => Generated.invoke();
        internal static void Invokes(SetSeedEvent ev) => SetSeed?.invoke(ev);
        internal static void Invokes(DoorDamageEvent ev) => DoorDamage?.invoke(ev);
        internal static void Invokes(DoorLockEvent ev) => DoorLock?.invoke(ev);
        internal static void Invokes(DoorOpenEvent ev) => DoorOpen?.invoke(ev);
        internal static void Invokes(UseLiftEvent ev) => UseLift?.invoke(ev);
        internal static void Invokes(ScpDeadAnnouncementEvent ev) => ScpDeadAnnouncement?.invoke(ev);
        internal static void Invokes(CreatePickupEvent ev) => CreatePickup?.invoke(ev);
        internal static void Invokes(ConvertUnitNameEvent ev) => ConvertUnitName?.invoke(ev);
    }
}