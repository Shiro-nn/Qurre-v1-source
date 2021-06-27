using Qurre.API.Events;
using static Qurre.Events.Round;
namespace Qurre.Events.Invoke
{
    public static class Round
    {
        public static void WaitingForPlayers() => Invokes(Type.WaitingForPlayers);
        public static void Start() => Invokes(Type.Start);
        public static void Restart() => Invokes(Type.Restart);
        public static void End(RoundEndEvent ev) => Invokes(ev);
        public static void Check(CheckEvent ev) => Invokes(ev);
        public static void TeamRespawn(TeamRespawnEvent ev) => Invokes(ev);
    }
}