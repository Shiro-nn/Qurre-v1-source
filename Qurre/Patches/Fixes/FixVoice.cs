using System;
using HarmonyLib;
using Mirror;
using UnityEngine;
namespace Qurre.Patches.Fixes
{
    [HarmonyPatch(typeof(NetworkConnection), "TransportReceive")]
    internal static class FixVoice
	{
		internal static bool Prefix(NetworkConnection __instance, ArraySegment<byte> buffer)
		{
			if (buffer.Count < 2)
			{
				Debug.LogError($"ConnectionRecv {__instance} Message was too short (messages should start with message id)");
				return false;
			}
			return true;
		}
	}
}