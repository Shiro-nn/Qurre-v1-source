using HarmonyLib;
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
                var ev = new GroupChangeEvent(ReferenceHub.GetHub(__instance.gameObject), group);
                Qurre.Events.Player.groupchange(ev);
                return ev.IsAllowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching PlayeR.GroupChange:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}