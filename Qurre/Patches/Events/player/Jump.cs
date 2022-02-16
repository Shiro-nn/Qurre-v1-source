using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Qurre.API.Events;
using Qurre.API;

namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(PlayerMovementSync), nameof(PlayerMovementSync.ReceivePosition2DJump), typeof(ReferenceHub))]
    internal static class Jump  
    {
        internal static bool Prefix(PlayerMovementSync __instance, ReferenceHub ply, PositionMessage2DJump position)
        {
            try
            {
                var pl = Player.Get(ply);
                var ev = new JumpEvent(pl, position.Position);
                Qurre.Events.Invoke.Player.Jump(ev);
                if(!ev.Allowed) return false;
                __instance._hub.playerMovementSync.ReceivePosition2D(ev.Position, true);
                return false;

            }
            catch (Exception e)
            {
                Log.Error($"ummmm, error in patching Player[Jump]:{e}\n{e.StackTrace}");
                return true; 
            }
          
        }
    }
}
