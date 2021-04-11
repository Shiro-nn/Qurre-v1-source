#pragma warning disable SA1313
using System;
using HarmonyLib;
using UnityEngine;
using Qurre.API.Events;
namespace Qurre.Patches.Events.SCPs.SCP079
{
    [HarmonyPatch(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.CallRpcGainExp))]
    internal static class GetEXP
    {
        private static bool Prefix(Scp079PlayerScript __instance, ExpGainType type, RoleType details)
        {
            try
            {
                var ev = new GetEXPEvent(API.Player.Get(__instance.gameObject), type, (float)details);
                switch (type)
                {
                    case ExpGainType.KillAssist:
                    case ExpGainType.PocketAssist:
                        {
                            Team team = __instance.GetComponent<CharacterClassManager>().Classes.SafeGet(details).team;
                            int num = 6;
                            switch (team)
                            {
                                case Team.SCP:
                                    ev.Amount = __instance.GetManaFromLabel("SCP Kill Assist", __instance.expEarnWays);
                                    num = 11;
                                    break;
                                case Team.MTF:
                                    ev.Amount = __instance.GetManaFromLabel("MTF Kill Assist", __instance.expEarnWays);
                                    num = 9;
                                    break;
                                case Team.CHI:
                                    ev.Amount = __instance.GetManaFromLabel("Chaos Kill Assist", __instance.expEarnWays);
                                    num = 8;
                                    break;
                                case Team.RSC:
                                    ev.Amount = __instance.GetManaFromLabel("Scientist Kill Assist", __instance.expEarnWays);
                                    num = 10;
                                    break;
                                case Team.CDP:
                                    ev.Amount = __instance.GetManaFromLabel("Class-D Kill Assist", __instance.expEarnWays);
                                    num = 7;
                                    break;
                                default:
                                    ev.Amount = 0f;
                                    break;
                            }
                            num--;
                            if (type == ExpGainType.PocketAssist)
                                ev.Amount /= 2f;
                            break;
                        }
                    case ExpGainType.DirectKill:
                    case ExpGainType.HardwareHack:
                        break;
                    case ExpGainType.AdminCheat:
                        ev.Amount = (float)details;
                        break;
                    case ExpGainType.GeneralInteractions:
                        {
                            switch (details)
                            {
                                case RoleType.ClassD:
                                    ev.Amount = __instance.GetManaFromLabel("Door Interaction", __instance.expEarnWays);
                                    break;
                                case RoleType.Spectator:
                                    ev.Amount = __instance.GetManaFromLabel("Tesla Gate Activation", __instance.expEarnWays);
                                    break;
                                case RoleType.Scientist:
                                    ev.Amount = __instance.GetManaFromLabel("Lockdown Activation", __instance.expEarnWays);
                                    break;
                                case RoleType.Scp079:
                                    ev.Amount = __instance.GetManaFromLabel("Elevator Use", __instance.expEarnWays);
                                    break;
                            }
                            if (ev.Amount != 0f)
                            {
                                float num4 = 1f / Mathf.Clamp(__instance.levels[__instance.NetworkcurLvl].manaPerSecond / 1.5f, 1f, 7f);
                                ev.Amount = Mathf.Round(ev.Amount * num4 * 10f) / 10f;
                            }
                            break;
                        }
                    default:
                        return false;
                }
                Qurre.Events.SCPs.SCP079.getEXP(ev);
                if (ev.Allowed && ev.Amount > 0)
                {
                    __instance.AddExperience(ev.Amount);
                    return false;
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP079 [GetEXP]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}