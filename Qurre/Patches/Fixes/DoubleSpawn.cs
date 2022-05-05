using HarmonyLib;
using MEC;
using System;
using System.Collections.Generic;
using static CharacterClassManager;

namespace Qurre.Patches.Fixes
{
    [HarmonyPatch(typeof(CharacterClassManager), "SetClassIDAdv")]
    internal static class DoubleSpawn
    {
        internal static Dictionary<CharacterClassManager, Module> Data = new();

        internal static bool Prefix(CharacterClassManager __instance, RoleType id, SpawnReason spawnReason)
        {
            Timing.CallDelayed(5f, () =>
            {
                if (Data.ContainsKey(__instance))
                    Data.Remove(__instance);
            });

            if (!Data.ContainsKey(__instance))
            {
                Data.Add(__instance, new Module(DateTime.Now, id));
                return true;
            }

            Module data = Data[__instance];

            if ((DateTime.Now - data.Date).TotalSeconds < 1 && data.Role == id || spawnReason == SpawnReason.LateJoin && !Loader.LateJoinSpawn)
                return false;

            return true;
        }

        [Serializable]
        internal class Module
        {
            public DateTime Date { get; private set; } = DateTime.Now;

            public RoleType Role { get; private set; } = RoleType.None;

            internal Module(DateTime date, RoleType roleType)
            {
                Date = date;
                Role = roleType;
            }
        }
    }
}