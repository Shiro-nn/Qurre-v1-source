using Qurre.API.Events;
using Qurre.Events.modules;
using static Qurre.API.Events.Grenade;
using static Qurre.Events.modules.main;
namespace Qurre.Events
{
    public static class Map
    {
        #region main
        public static event AllEvents<LczDeconEvent> LCZDecon;
        public static event AllEvents<AnnouncementDecontaminationEvent> AnnouncementDecontaminationZDecon;
        public static event AllEvents<MtfAnnouncementEvent> MTFAnnouncement;
        public static event AllEvents<NewBloodEvent> NewBlood;
        public static event AllEvents<NewDecalEvent> NewDecal;
        public static void lczdecon(LczDeconEvent ev) => LCZDecon.invoke(ev);
        public static void announcementdecontamination(AnnouncementDecontaminationEvent ev) => AnnouncementDecontaminationZDecon.invoke(ev);
        public static void mtfAnnouncement(MtfAnnouncementEvent ev) => MTFAnnouncement.invoke(ev);
        public static void newblood(NewBloodEvent ev) => NewBlood.invoke(ev);
        public static void newdecal(NewDecalEvent ev) => NewDecal.invoke(ev);
        #endregion
        #region Grenade
        public static class Grenade
        {
            public static event AllEvents<ExplodeEvent> Explode;
            public static void explode(ExplodeEvent ev) => Explode.invoke(ev);
        }
        #endregion
    }
}