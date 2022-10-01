using System.Linq;
using HarmonyLib;
using Qurre.API.Events;
using Respawning.NamingRules;
namespace Qurre.Patches.Events.Map
{
    using Qurre.API;
    [HarmonyPatch(typeof(NineTailedFoxNamingRule), nameof(NineTailedFoxNamingRule.PlayEntranceAnnouncement))]
    internal static class MTFAnnouncement
    {
        private static bool Prefix(ref string regular)
        {
            try
            {
                if (string.IsNullOrEmpty(regular)) return false;
                int scpsLeft = Player.List.Where(x => x.Team == Team.SCP && x.Role != RoleType.Scp0492).Count();
                string[] inf = regular.Split('-');
                string unit = "";
                int num = 0;
                if (inf.Length >= 2)
                {
                    unit = inf[0];
                    num = int.Parse(inf[1]);
                }
                var ev = new MTFAnnouncementEvent(scpsLeft, unit, num);
                Qurre.Events.Invoke.Map.MTFAnnouncement(ev);
                regular = $"{ev.UnitName}-{ev.UnitNumber}";
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Map [MTFAnnouncement]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}