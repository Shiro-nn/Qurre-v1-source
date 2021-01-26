#pragma warning disable SA1313
using System.Linq;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using Respawning.NamingRules;
namespace Qurre.Patches.Events.MAP
{
    [HarmonyPatch(typeof(NineTailedFoxNamingRule), nameof(NineTailedFoxNamingRule.PlayEntranceAnnouncement))]
    internal static class MTFAnnouncement
    {
        private static bool Prefix(ref string regular)
        {
            try
            {
                int scpsLeft = Player.GetHubs().Where(player => player.GetTeam() == Team.SCP && player.GetRole() != RoleType.Scp0492).Count();
                string[] unitInformations = regular.Split('-');
                var ev = new MTFAnnouncementEvent(scpsLeft, unitInformations[0], int.Parse(unitInformations[1]));
                Qurre.Events.Map.mtfAnnouncement(ev);
                regular = $"{ev.UnitName}-{ev.UnitNumber}";
                return ev.IsAllowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching MAP.MTFAnnouncement:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}