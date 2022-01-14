using CustomPlayerEffects;
using HarmonyLib;
using Mirror;
using PlayableScps;
using Qurre.API;
using Qurre.API.Events;
using System;
using System.Linq;
using UnityEngine;
namespace Qurre.Patches.Modules
{
    [HarmonyPatch(typeof(PlayerPositionManager), nameof(PlayerPositionManager.TransmitData))]
    internal static class InvisiblePatch
    {
        internal static bool Prefix(PlayerPositionManager __instance)
        {
            try
            {
                if (!NetworkServer.active) return false;
                int ii = __instance._frame; ii++;
                __instance._frame = ii;
                if (__instance._frame != __instance._syncFrequency) return false;
                __instance._frame = 0;
                var players = Player.List.ToList();
                players.AddRange(Map.Bots.Select(x => x.Player));
                __instance._usedData = players.Count;
                if (__instance.ReceivedData == null || __instance.ReceivedData.Length < __instance._usedData)
                    __instance.ReceivedData = new PlayerPositionData[__instance._usedData * 2];
                for (var i = 0; i < __instance._usedData; i++)
                    __instance.ReceivedData[i] = new PlayerPositionData(players[i].ReferenceHub);
                if (__instance._transmitBuffer == null || __instance._transmitBuffer.Length < __instance._usedData)
                    __instance._transmitBuffer = new PlayerPositionData[__instance._usedData * 2];
                foreach (var player in players)
                {
                    try
                    {
                        if (player.Connection == null) continue;
                        Array.Copy(__instance.ReceivedData, __instance._transmitBuffer, __instance._usedData);
                        for (int k = 0; k < __instance._usedData; k++)
                        {
                            var showinvoid = false;
                            var playerToShow = players[k];
                            var vector = __instance._transmitBuffer[k].position - player.Position;
                            if (player.Role == RoleType.Scp173)
                            {
                                if ((playerToShow.Team == Team.SCP && !Loader.ScpTrigger173) || player.Scp173Controller.IgnoredPlayers.Contains(playerToShow) || playerToShow.Invisible)
                                {
                                    var posinfo = __instance._transmitBuffer[k];
                                    var rot = Quaternion.LookRotation(playerToShow.Position - player.Position).eulerAngles.y;
                                    __instance._transmitBuffer[k] = new PlayerPositionData(posinfo.position, rot, posinfo.playerID);
                                }
                            }
                            else if (player.Role == RoleType.Scp93953 || player.Role == RoleType.Scp93989)
                            {
                                if (__instance._transmitBuffer[k].position.y < 800f && playerToShow.Team != Team.RIP && playerToShow.Team != Team.SCP && !playerToShow.ReferenceHub.GetComponent<Scp939_VisionController>().CanSee(player.PlayerEffectsController.GetEffect<Visuals939>()))
                                {
                                    showinvoid = true;
                                    goto AA_001;
                                }
                            }
                            if (playerToShow.Invisible)
                            {
                                showinvoid = true;
                                goto AA_001;
                            }
                            if (player.Role == RoleType.Spectator) continue;
                            if (Math.Abs(vector.y) > 35f)
                            {
                                showinvoid = true;
                                goto AA_001;
                            }
                            else
                            {
                                var sqrMagnitude = vector.sqrMagnitude;
                                if (player.Position.y < 800f)
                                {
                                    if (sqrMagnitude >= 1764f)
                                    {
                                        showinvoid = true;
                                        goto AA_001;
                                    }
                                }
                                else if (sqrMagnitude >= 7225f)
                                {
                                    showinvoid = true;
                                    goto AA_001;
                                }

                                if (playerToShow != null)
                                {
                                    var scp = player.ReferenceHub.scpsController.CurrentScp as Scp096;
                                    if (scp != null && scp.Enraged && !scp.HasTarget(playerToShow.ReferenceHub))
                                    {
                                        showinvoid = true;
                                        goto AA_001;
                                    }
                                    if (playerToShow.ReferenceHub.playerEffectsController.GetEffect<Invisible>().IsEnabled)
                                    {
                                        var flag = false;
                                        if (scp != null) flag = scp.HasTarget(playerToShow.ReferenceHub);

                                        if (player.Role == RoleType.Scp079 || flag)
                                        {
                                            if (Loader.Better268) showinvoid = true;
                                        }
                                        else showinvoid = true;
                                    }
                                }
                            }


                        AA_001:
                            var posData = __instance._transmitBuffer[k];
                            var rotation = posData.rotation;
                            var pos = posData.position;

                            var ev = new TransmitPlayerDataEvent(player, playerToShow, pos, rotation, showinvoid);
                            Qurre.Events.Invoke.Player.TransmitPlayerData(ev);
                            pos = ev.Position;
                            rotation = ev.Rotation;
                            showinvoid = ev.Invisible;
                            if (player == playerToShow) showinvoid = false;

                            if (showinvoid) __instance._transmitBuffer[k] = new PlayerPositionData(Vector3.up * 6000f, 0.0f, playerToShow.Id);
                            else __instance._transmitBuffer[k] = new PlayerPositionData(pos, rotation, playerToShow.Id);
                        }
                        var conn = player.Connection;
                        if (__instance._usedData <= 20) conn.Send(new PositionPPMMessage(__instance._transmitBuffer, (byte)__instance._usedData, 0), 1);
                        else
                        {
                            byte b = 0;
                            while ((int)b < __instance._usedData / 20)
                            {
                                conn.Send(new PositionPPMMessage(__instance._transmitBuffer, 20, b), 1);
                                b += 1;
                            }
                            byte b2 = (byte)(__instance._usedData % (b * 20));
                            if (b2 > 0) conn.Send(new PositionPPMMessage(__instance._transmitBuffer, b2, b), 1);
                        }
                    }
                    catch { }
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Modules [Invisible]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}