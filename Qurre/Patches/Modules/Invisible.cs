using CustomPlayerEffects;
using HarmonyLib;
using Mirror;
using PlayableScps;
using Qurre.API;
using Qurre.API.Events;
using System;
using System.Collections.Generic;
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
                __instance._frame++;
                if (__instance._frame != __instance._syncFrequency) return false;

                __instance._frame = 0;
                IEnumerable<Player> players = Player.Dictionary.Values;
                __instance._usedData = players.Count();

                if (__instance.ReceivedData is null || __instance.ReceivedData.Length < __instance._usedData)
                    __instance.ReceivedData = new PlayerPositionData[__instance._usedData * 2];

                for (var i = 0; i < __instance._usedData; i++)
                    __instance.ReceivedData[i] = new PlayerPositionData(players.ElementAt(i).ReferenceHub);

                if (__instance._transmitBuffer is null || __instance._transmitBuffer.Length < __instance._usedData)
                    __instance._transmitBuffer = new PlayerPositionData[__instance._usedData * 2];

                foreach (Player player in players)
                {
                    try
                    {
                        if (player.Connection is null) continue;

                        Array.Copy(__instance.ReceivedData, __instance._transmitBuffer, __instance._usedData);

                        for (int k = 0; k < __instance._usedData; k++)
                        {
                            if (player.Role is RoleType.Spectator) continue;

                            bool _show = false;
                            Player playerToShow = players.ElementAt(k);
                            Vector3 vector = __instance._transmitBuffer[k].position - player.Position;

                            if (player.Role is RoleType.Scp173)
                            {
                                if ((playerToShow.Team is Team.SCP && !Loader.ScpTrigger173) ||
                                    player.Scp173Controller.IgnoredPlayers.Contains(playerToShow) || playerToShow.Invisible)
                                {
                                    PlayerPositionData posinfo = __instance._transmitBuffer[k];
                                    float rot = Quaternion.LookRotation(playerToShow.Position - player.Position).eulerAngles.y;
                                    __instance._transmitBuffer[k] = new PlayerPositionData(posinfo.position, rot, posinfo.playerID);
                                }
                            }
                            else if (player.Role is RoleType.Scp93953 or RoleType.Scp93989)
                            {
                                if (__instance._transmitBuffer[k].position.y < 800f && playerToShow.Team is not Team.RIP and not Team.SCP &&
                                    !playerToShow.ReferenceHub.GetComponent<Scp939_VisionController>().CanSee(player.PlayerEffectsController.GetEffect<Visuals939>()))
                                {
                                    _show = true;
                                    goto AA_001;
                                }
                            }
                            if (playerToShow.Invisible)
                            {
                                _show = true;
                                goto AA_001;
                            }
                            if (Math.Abs(vector.y) > 35f)
                            {
                                _show = true;
                                goto AA_001;
                            }
                            else
                            {
                                float sqrMagnitude = vector.sqrMagnitude;
                                if (player.Position.y < 800f)
                                {
                                    if (sqrMagnitude >= 1764f)
                                    {
                                        _show = true;
                                        goto AA_001;
                                    }
                                }
                                else if (sqrMagnitude >= 7225f)
                                {
                                    _show = true;
                                    goto AA_001;
                                }

                                if (playerToShow != null)
                                {
                                    Scp096 scp = player.ScpsController.CurrentScp as Scp096;
                                    if (scp is not null && scp.Enraged && !scp.HasTarget(playerToShow.ReferenceHub) && playerToShow.Team is not Team.SCP)
                                    {
                                        _show = true;
                                        goto AA_001;
                                    }
                                    if (playerToShow.GetEffect<Invisible>().IsEnabled)
                                    {
                                        if (player.Role == RoleType.Scp079 || 
                                            (scp is not null && scp.HasTarget(playerToShow.ReferenceHub)))
                                        {
                                            if (Loader.Better268) _show = true;
                                        }
                                        else _show = true;
                                    }
                                }
                            }


                        AA_001:
                            PlayerPositionData posData = __instance._transmitBuffer[k];
                            float rotation = posData.rotation;
                            Vector3 pos = posData.position;

                            TransmitPlayerDataEvent ev = new(player, playerToShow, pos, rotation, _show);
                            Qurre.Events.Invoke.Player.TransmitPlayerData(ev);
                            pos = ev.Position;
                            rotation = ev.Rotation;
                            _show = ev.Invisible;
                            if (player == playerToShow) _show = false;

                            if (_show) __instance._transmitBuffer[k] = new PlayerPositionData(Vector3.up * 6000f, 0.0f, playerToShow.Id);
                            else __instance._transmitBuffer[k] = new PlayerPositionData(pos, rotation, playerToShow.Id);
                        }
                        NetworkConnection conn = player.Connection;
                        if (__instance._usedData <= 20) conn.Send(new PositionPPMMessage(__instance._transmitBuffer, (byte)__instance._usedData, 0), 1);
                        else
                        {
                            byte b = 0;
                            while (b < __instance._usedData / 20)
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