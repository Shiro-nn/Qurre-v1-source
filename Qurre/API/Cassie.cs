using MEC;
using Respawning;
namespace Qurre.API
{
	public static class Cassie
	{
		public static void Send(string message, bool makeHold, bool makeNoise) => RespawnEffectsController.PlayCassieAnnouncement(message, makeHold, makeNoise);
		public static void DelayedSend(string message, bool makeHold, bool makeNoise, float delay) =>
			Timing.CallDelayed(delay, () => RespawnEffectsController.PlayCassieAnnouncement(message, makeHold, makeNoise));
	}
}