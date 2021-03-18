using MEC;
using Respawning;
namespace Qurre.API
{
	public class Cassie
	{
		public static void Send(string msg, bool makeHold = false, bool makeNoise = false) => RespawnEffectsController.PlayCassieAnnouncement(msg, makeHold, makeNoise);
		public static void DelayedSend(string msg, float delay, bool makeHold = false, bool makeNoise = false) =>
			Timing.CallDelayed(delay, () => RespawnEffectsController.PlayCassieAnnouncement(msg, makeHold, makeNoise));
	}
}