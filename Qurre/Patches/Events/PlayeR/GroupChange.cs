using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.SetGroup))]
    internal static class GroupChange
    {
        private static bool Prefix(ServerRoles __instance, UserGroup group)
        {
            try
            {
                var ev = new GroupChangeEvent(Player.Get(__instance.gameObject), group);
                Qurre.Events.Invoke.Player.GroupChange(ev);
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Player [GroupChange]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}