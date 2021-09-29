using HarmonyLib;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.Map
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.RpcPlaceBlood))]
    internal static class NewBlood
    {
        private static bool Prefix(CharacterClassManager __instance, ref Vector3 pos, ref int type, ref float f)
        {
            try
            {
                var ev = new NewBloodEvent(API.Player.Get(__instance.gameObject), pos, type, f);
                Qurre.Events.Invoke.Map.NewBlood(ev);
                pos = ev.Position;
                type = ev.Type;
                f = ev.Multiplier;
                return ev.Allowed && Loader.SpawnBlood;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Map [NewBlood]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}