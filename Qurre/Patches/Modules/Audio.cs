using Dissonance;
using HarmonyLib;
using Mirror;
using RemoteAdmin;
namespace Qurre.Patches.Modules
{
	[HarmonyPatch(typeof(CustomBroadcastTrigger), nameof(CustomBroadcastTrigger.IsUserActivated))]
	internal static class FixAudio1
	{
		public static bool Prefix(CustomBroadcastTrigger __instance, ref bool __result)
		{
			bool flag = __instance.GetComponent<QueryProcessor>().PlayerId >= 9000 || __instance.GetComponent<NetworkIdentity>().connectionToClient == null;
			bool result;
			if (flag)
			{
				__result = true;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}
	}
	[HarmonyPatch(typeof(VoiceBroadcastTrigger), "get_CanTrigger")]
	internal static class FixAudio2
	{
		public static bool Prefix(VoiceBroadcastTrigger __instance, ref bool __result)
		{
			bool flag = __instance.GetComponent<QueryProcessor>().PlayerId >= 9000 || __instance.GetComponent<NetworkIdentity>().connectionToClient == null;
			bool result;
			if (flag)
			{
				__result = true;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}
	}
	[HarmonyPatch(typeof(BaseCommsTrigger), "get_TokenActivationState")]
	internal static class FixAudio3
	{
		public static bool Prefix(BaseCommsTrigger __instance, ref bool __result)
		{
			bool flag = __instance.GetComponent<QueryProcessor>().PlayerId >= 9000 || __instance.GetComponent<NetworkIdentity>().connectionToClient == null;
			bool result;
			if (flag)
			{
				__result = true;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}
	}
	[HarmonyPatch(typeof(VoiceBroadcastTrigger), nameof(VoiceBroadcastTrigger.ShouldActivate))]
	internal static class FixAudio4
	{
		public static bool Prefix(VoiceBroadcastTrigger __instance, ref bool __result)
		{
			bool flag = __instance.GetComponent<QueryProcessor>().PlayerId >= 9000 || __instance.GetComponent<NetworkIdentity>().connectionToClient == null;
			bool result;
			if (flag)
			{
				__result = true;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}
	}
}