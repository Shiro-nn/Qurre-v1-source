using HarmonyLib;
using MEC;
using Qurre.API;
using System;
using System.Collections.Generic;
using static CharacterClassManager;
namespace Qurre.Patches.Modules
{
	[HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetClassIDAdv))]
	internal static class FixDoubleSpawn
	{
		internal static Dictionary<CharacterClassManager, Module> Data = new Dictionary<CharacterClassManager, Module>();
		internal static bool Prefix(CharacterClassManager __instance, RoleType id, bool lite, SpawnReason spawnReason, bool isHook = false)
		{
			Timing.CallDelayed(5f, () =>
			{
				if (Data.ContainsKey(__instance)) Data.Remove(__instance);
			});
			if (!Data.ContainsKey(__instance))
			{
				Data.Add(__instance, new Module(DateTime.Now, id));
				return true;
			}
			var data = Data[__instance];
			if ((DateTime.Now - data.Date).TotalSeconds < 1 && data.Role == id)
				return false;
			if (spawnReason == SpawnReason.LateJoin && !Loader.LateJoinSpawn)
				return false;
			else return true;
		}
		[Serializable]
		internal class Module
		{
			public DateTime Date = DateTime.Now;
			public RoleType Role = RoleType.None;
			internal Module(DateTime date, RoleType roleType)
			{
				Date = date;
				Role = roleType;
			}
		}
	}
}