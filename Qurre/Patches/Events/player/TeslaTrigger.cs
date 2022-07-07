using System;
using System.Collections.Generic;
using HarmonyLib;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(TeslaGateController), nameof(TeslaGateController.FixedUpdate))]
    internal static class TeslaTrigger
    {
		private static bool Prefix(TeslaGateController __instance)
		{
			try
			{
				if (__instance == null) return false;
				if (!NetworkServer.active) return false;
				Dictionary<GameObject, ReferenceHub> allHubs = ReferenceHub.GetAllHubs();
				using (List<TeslaGate>.Enumerator enumerator = __instance.TeslaGates.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TeslaGate teslaGate = enumerator.Current;
						if (teslaGate.isActiveAndEnabled)
						{
							if (teslaGate.InactiveTime > 0f)
							{
								teslaGate.NetworkInactiveTime = Mathf.Max(0f, teslaGate.InactiveTime - Time.fixedDeltaTime);
							}
							else
							{
								bool flag = false;
								bool flag2 = false;
								foreach (KeyValuePair<GameObject, ReferenceHub> keyValuePair in allHubs)
								{
									if (!keyValuePair.Value.isDedicatedServer)
									{
										if (!flag)
										{
											flag = teslaGate.PlayerInIdleRange(keyValuePair.Value);
										}
										if (keyValuePair.Value.characterClassManager.CurClass != RoleType.Spectator && !flag2 && teslaGate.PlayerInRange(keyValuePair.Value) && !teslaGate.InProgress)
										{
                                            if(!Player.Get(keyValuePair.Key).Invisible && !(!teslaGate.PlayerInRange(keyValuePair.Value) || teslaGate.InProgress))
                                            {
												var ev = new TeslaTriggerEvent(Player.Get(keyValuePair.Key), teslaGate.GetTesla(), teslaGate.PlayerInHurtRange(keyValuePair.Key));
												Qurre.Events.Invoke.Player.TeslaTrigger(ev);
												if(ev.Tesla.ImmunityPlayers.Contains(ev.Player) || ev.Tesla.ImmunityRoles.Contains(ev.Player.Role))
                                                {
													continue;
                                                }
                                                else
                                                {
													flag2 = true;
                                                }
											}
                                            else
                                            {
												continue;
                                            }
										}
									}
								}
								if (flag2)
								{
									teslaGate.ServerSideCode();
								}
								if (flag != teslaGate.isIdling)
								{
									teslaGate.ServerSideIdle(flag);
								}
							}
						}
					}
					return false;
				}
				foreach (TeslaGate teslaGate2 in __instance.TeslaGates)
				{
					teslaGate2.ClientSideCode();
				}
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching Player [TeslaTrigger]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}