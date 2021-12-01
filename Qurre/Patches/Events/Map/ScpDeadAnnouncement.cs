using HarmonyLib;
using PlayerStatsSystem;
using Qurre.API;
using Qurre.API.Events;
using System.Collections.Generic;
namespace Qurre.Patches.Events.Map
{
    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.AnnounceScpTermination))]
    internal static class ScpDeadAnnouncement
    {
        internal static bool Prefix(ReferenceHub scp, DamageHandlerBase hit)
        {
            try
            {
                var ev = new ScpDeadAnnouncementEvent(Player.Get(scp), hit.CassieDeathAnnouncement);
                Qurre.Events.Invoke.Map.ScpDeadAnnouncement(ev);
                DoVoid(ev.CassieDeath);
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Map [ScpDeadAnnouncement]:\n{e}\n{e.StackTrace}");
                return true;
            }
            void DoVoid(string CassieDeath)
            {
                NineTailedFoxAnnouncer.singleton.scpListTimer = 0f;
                Role curRole = scp.characterClassManager.CurRole;
                if (curRole.team != 0 || curRole.roleId == RoleType.Scp0492)
                {
                    return;
                }

                string cassieDeathAnnouncement = CassieDeath;
                if (string.IsNullOrEmpty(cassieDeathAnnouncement))
                {
                    return;
                }

                foreach (NineTailedFoxAnnouncer.ScpDeath scpDeath in NineTailedFoxAnnouncer.scpDeaths)
                {
                    if (!(scpDeath.announcement != cassieDeathAnnouncement))
                    {
                        scpDeath.scpSubjects.Add(scp.characterClassManager.CurRole);
                        return;
                    }
                }

                NineTailedFoxAnnouncer.scpDeaths.Add(new NineTailedFoxAnnouncer.ScpDeath
                {
                    scpSubjects = new List<Role>(new Role[1]
                    {
                curRole
                    }),
                    announcement = cassieDeathAnnouncement
                });
            }
        }
    }
}