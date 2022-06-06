using Dissonance;
using HarmonyLib;
using Dissonance.Integrations.MirrorIgnorance;
using Dissonance.Networking;
using Dissonance.Audio.Capture;
namespace Qurre.Patches.Modules
{
	[HarmonyPatch(typeof(CapturePipelineManager), nameof(CapturePipelineManager.RestartTransmissionPipeline))]
	internal static class FixAudio1
	{
		private static bool Prefix(string reason) => reason != "Detected a frame skip, forcing capture pipeline reset";
	}
	[HarmonyPatch(typeof(BaseCommsNetwork<MirrorIgnoranceServer, MirrorIgnoranceClient, MirrorConn, Unit, Unit>), nameof(BaseCommsNetwork<MirrorIgnoranceServer, MirrorIgnoranceClient, MirrorConn, Unit, Unit>.RunAsDedicatedServer))]
	internal static class FixAudio2
	{
		private static bool Prefix(BaseCommsNetwork<MirrorIgnoranceServer, MirrorIgnoranceClient, MirrorConn, Unit, Unit> __instance)
		{
			__instance.RunAsHost(Unit.None, Unit.None);
			return false;
		}
	}
}