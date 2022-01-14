using System;
using HarmonyLib;
using Mirror;
using UnityEngine;
namespace Qurre.Patches.Fixes
{
    [HarmonyPatch(typeof(NetworkConnection), "TransportReceive")]
    internal static class FixVC
	{
		internal static bool Prefix(NetworkConnection __instance, ArraySegment<byte> buffer)
		{
			if (buffer.Count < 2)
			{
				Debug.LogError(string.Format("[PATCHED] ConnectionRecv {0} Message was too short (messages should start with message id)", __instance));
				return false;
			}
			return true;
		}
	}
}