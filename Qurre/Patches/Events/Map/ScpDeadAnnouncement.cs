using HarmonyLib;
using PlayerStatsSystem;
using Qurre.API.Events;
namespace Qurre.Patches.Events.Map
{
    using Qurre.API;
    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.AnnounceScpTermination))]
    internal static class ScpDeadAnnouncement
    {
        internal static bool Prefix(ReferenceHub scp, DamageHandlerBase hit)
        {
            try
            {
                var ev = new ScpDeadAnnouncementEvent(Player.Get(scp), hit.CassieDeathAnnouncement);
                Qurre.Events.Invoke.Map.ScpDeadAnnouncement(ev);
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