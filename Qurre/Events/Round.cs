using Qurre.API.Events;
using Qurre.Events.Modules;
using static Qurre.Events.Modules.Main;
namespace Qurre.Events
{
    public static class Round
    {
        public static event AllEvents Waiting;
        public static event AllEvents Start;
        public static event AllEvents Restart;
        public static event AllEvents<RoundEndEvent> End;
        public static event AllEvents<CheckEvent> Check;
        public static event AllEvents<TeamRespawnEvent> TeamRespawn;
        internal static void InvokesW() => Waiting.invoke();
        internal static void InvokesS() => Start.invoke();
        internal static void InvokesR() => Restart.invoke();
        internal static void Invokes(RoundEndEvent ev) => End.invoke(ev);
        internal static void Invokes(CheckEvent ev) => Check?.invoke(ev);
        internal static void Invokes(TeamRespawnEvent ev) => TeamRespawn.invoke(ev);
    }
}