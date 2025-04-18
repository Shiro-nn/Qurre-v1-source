﻿using System;
using Mirror;
using HarmonyLib;
using Qurre.API.Events;
namespace Qurre.Patches.Events.Player
{
    using Qurre.API;
    [HarmonyPatch(typeof(PlayerMovementSync), nameof(PlayerMovementSync.ReceivePosition2DJump))]
    internal static class Jump
    {
        internal static bool Prefix(NetworkConnection connection, PositionMessage2DJump position)
        {
            try
            {
                var pl = Player.Get(connection.identity.gameObject);
                var ev = new JumpEvent(pl, position.Position);
                Qurre.Events.Invoke.Player.Jump(ev);
                if (!ev.Allowed) return false;
                pl.Movement.ReceivePosition2D(ev.Position, jump: true);
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