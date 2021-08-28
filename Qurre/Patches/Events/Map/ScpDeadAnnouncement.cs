using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
namespace Qurre.Patches.Events.Map
{
    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.AnnounceScpTermination))]
    internal static class ScpDeadAnnouncement
    {
        internal static bool Prefix(Role scp, ref PlayerStats.HitInfo hit, ref string groupId)
        {
            try
            {
                var ev = new ScpDeadAnnouncementEvent(string.IsNullOrEmpty(hit.Attacker) ? null : Player.Get(hit.Attacker), scp, hit, groupId);
                Qurre.Events.Invoke.Map.ScpDeadAnnouncement(ev);
                hit = ev.HitInfo;
                groupId = ev.GroupId;
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Map [ScpDeadAnnouncement]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}