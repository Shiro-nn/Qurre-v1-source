using Qurre.API.Events;
using Qurre.Events.modules;
using static Qurre.Events.modules.main;
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
        public static event AllEvents<GrenadeExplodeEvent> GrenadeExplode;
        public static event AllEvents<SetSeedEvent> SetSeed;
        public static event AllEvents<DoorDamageEvent> DoorDamage;
        public static void lczdecon(LCZDeconEvent ev) => LCZDecon?.invoke(ev);
        public static void announcementdecontamination(AnnouncementDecontaminationEvent ev) => AnnouncementDecontaminationZDecon?.invoke(ev);
        public static void mtfAnnouncement(MTFAnnouncementEvent ev) => MTFAnnouncement?.invoke(ev);
        public static void newblood(NewBloodEvent ev) => NewBlood?.invoke(ev);
        public static void newdecal(NewDecalEvent ev) => NewDecal?.invoke(ev);
        public static void generated() => Generated.invoke();
        public static void grenadeexplode(GrenadeExplodeEvent ev) => GrenadeExplode?.invoke(ev);
        public static void setSeed(SetSeedEvent ev) => SetSeed?.invoke(ev);
        public static void doordamage(DoorDamageEvent ev) => DoorDamage?.invoke(ev);
    }
}