using Dissonance;
using HarmonyLib;
using UnityEngine;
using Dissonance.Integrations.MirrorIgnorance;
using Dissonance.Networking;
using Dissonance.Audio.Capture;
namespace Qurre.Patches.Modules
{
	[HarmonyPatch(typeof(AudioSettings), nameof(AudioSettings.outputSampleRate), MethodType.Getter)]
	internal static class FixAudio1
	{
		private static bool Prefix(ref int __result)
		{
			__result = 48000;
			return false;
		}
	}
	[HarmonyPatch(typeof(CapturePipelineManager), nameof(CapturePipelineManager.RestartTransmissionPipeline))]
	internal static class FixAudio2
	{
		private static bool Prefix(string reason) => reason != "Detected a frame skip, forcing capture pipeline reset";
	}
	[HarmonyPatch(typeof(BaseCommsNetwork<MirrorIgnoranceServer, MirrorIgnoranceClient, MirrorConn, Unit, Unit>), nameof(BaseCommsNetwork<MirrorIgnoranceServer, MirrorIgnoranceClient, MirrorConn, Unit, Unit>.RunAsDedicatedServer))]
	internal static class FixAudio3
	{
		private static bool Prefix(BaseCommsNetwork<MirrorIgnoranceServer, MirrorIgnoranceClient, MirrorConn, Unit, Unit> __instance)
		{
			__instance.RunAsHost(Unit.None, Unit.None);
			return false;
		}
	}
}