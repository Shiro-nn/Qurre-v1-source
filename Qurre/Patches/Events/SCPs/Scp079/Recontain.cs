using HarmonyLib;
using System;
using Qurre.API.Events;
namespace Qurre.Patches.Events.SCPs.Scp079
{
    [HarmonyPatch(typeof(Recontainer079), nameof(Recontainer079.RefreshActivator))]
    internal static class Recontain
    {
        internal static bool Prefix(Recontainer079 __instance)
        {
            try
            {
                if (!(__instance._delayStopwatch.Elapsed.TotalSeconds > __instance._activationDelay && __instance._delayStopwatch.IsRunning))
                {
                    if (!__instance._activatorGlass.isBroken) return false;
                    if (__instance._alreadyRecontained) return true;
                }
                var ev = new Scp079RecontainEvent(__instance);
                Qurre.Events.Invoke.Scp079.Recontain(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP079 [Recontain]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}