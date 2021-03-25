using Qurre.API.Events;
using Qurre.Events.modules;
using static Qurre.Events.modules.main;
namespace Qurre.Events
{
    public static class Round
    {
        public static event AllEvents WaitingForPlayers;
        public static event AllEvents Start;
        public static event AllEvents Restart;
        public static event AllEvents<RoundEndEvent> End;
        public static event AllEvents<CheckEvent> Check;
        public static event AllEvents<TeamRespawnEvent> TeamRespawn;
        public static void waitingforplayers() => WaitingForPlayers.invoke();
        public static void start() => Start.invoke();
        public static void restart() => Restart.invoke();
        public static void end(RoundEndEvent ev) => End.invoke(ev);
        public static void check(CheckEvent ev) => Check?.invoke(ev);
        public static void teamrespawn(TeamRespawnEvent ev) => TeamRespawn.invoke(ev);
    }
}