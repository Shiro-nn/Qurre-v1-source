using Dissonance;
using HarmonyLib;
using Mirror;
using RemoteAdmin;
namespace Qurre.Patches.etc
{
	[HarmonyPatch(typeof(CustomBroadcastTrigger), "IsUserActivated")]
	public class TriggerAudio1
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
	public class TriggerAudio2
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
	public class TriggerAudio3
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
	[HarmonyPatch(typeof(VoiceBroadcastTrigger), "ShouldActivate")]
	public class TriggerAudio4
	{
		public static bool Prefix(VoiceBroadcastTrigger __instance, ref bool __result, bool intent)
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