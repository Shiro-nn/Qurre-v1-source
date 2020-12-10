using MEC;
using Respawning;
namespace Qurre.API
{
	public static class Cassie
	{
		public static void Send(string msg, bool makeHold, bool makeNoise) => RespawnEffectsController.PlayCassieAnnouncement(msg, makeHold, makeNoise);
		public static void DelayedSend(string msg, bool makeHold, bool makeNoise, float delay) =>
			Timing.CallDelayed(delay, () => RespawnEffectsController.PlayCassieAnnouncement(msg, makeHold, makeNoise));
	}
}