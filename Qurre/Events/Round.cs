using Qurre.API.Events;
using Qurre.Events.modules;
using static Qurre.Events.modules.Main;
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
        internal static void Invokes(Type type)
        {
            if (type == Type.WaitingForPlayers) WaitingForPlayers.invoke();
            else if (type == Type.Start) Start.invoke();
            else if (type == Type.Restart) Restart.invoke();
            else Log.Error("[Events] The type called to Round.Invokes was not found.");
        }
        internal static void Invokes(RoundEndEvent ev) => End.invoke(ev);
        internal static void Invokes(CheckEvent ev) => Check?.invoke(ev);
        internal static void Invokes(TeamRespawnEvent ev) => TeamRespawn.invoke(ev);
        internal enum Type
        {
            WaitingForPlayers,
            Start,
            Restart
        }
    }
}