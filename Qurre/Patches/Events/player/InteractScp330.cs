using System;
using Interactables.Interobjects;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(Scp330Interobject), nameof(Scp330Interobject.ServerInteract))]
    internal static class InteractScp330
    {
        internal static bool Prefix(ReferenceHub ply)
        {
            try
            {
                if (!ply.characterClassManager.IsHuman()) return false;
                var ev = new InteractScp330Event(Player.Get(ply));
                Qurre.Events.Invoke.Player.InteractScp330(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [InteractScp330]:\n{e}\n{e.StackTrace}");
                return true;
            }

        }
    }
}