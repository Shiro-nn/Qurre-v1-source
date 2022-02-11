using System;
using System.Linq;
using System.Collections.Generic;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using Interactables.Interobjects.DoorUtils;
using NorthwoodLib.Pools;
using GameCore;
using UnityEngine;
using Console = GameCore.Console;

namespace Qurre.Patches.Events.SCPs.Scp079
{
    [HarmonyPatch(typeof(Scp079PlayerScript), "UserCode_CmdInteract")]
    public class Interact
    {
        private static bool Prefix(Scp079PlayerScript __instance, Command079 command, string args, GameObject target)
        {
			try
			{
				if (__instance is null || !__instance._interactRateLimit.CanExecute(true) || !__instance.iAm079)
					return false;

				Console.AddDebugLog("SCP079", "Command received from a client: " + command, MessageImportance.LessImportant, false);
				__instance.RefreshCurrentRoom();

				if (!__instance.CheckInteractableLegitness(__instance.CurrentRoom, target, true))
					return false;

				DoorVariant doorVariant = null;
				bool flag = target is not null && target.TryGetComponent(out doorVariant);
				List<string> list = ConfigFile.ServerConfig.GetStringList("scp079_door_blacklist") ?? new List<string>();
				Player player = Player.Get(__instance.gameObject);

				switch (command)
				{
					case Command079.Door:
						{
							if (AlphaWarheadController.Host.inProgress || !flag)
								return false;

							if (target is null)
							{
								Console.AddDebugLog("SCP079", "The door command requires a target.", global::MessageImportance.LessImportant, false);
								return false;
							}

							if (doorVariant.TryGetComponent(out DoorNametagExtension doorNametagExtension) && list.Count > 0 && list.Contains(doorNametagExtension.GetName))
							{
								Console.AddDebugLog("SCP079", "Door access denied by the server.", MessageImportance.LeastImportant, false);
								return false;
							}

							string text = doorVariant.RequiredPermissions.RequiredPermissions.ToString();
							float manaFromLabel = __instance.GetManaFromLabel("Door Interaction " + (text.Contains(",") ? text.Split(new char[]
							{
								','
							})[0] : text), __instance.abilities);

							var ev = new Scp079InteractDoorEvent(player, doorVariant.GetDoor(), manaFromLabel);
							Qurre.Events.Invoke.Scp079.InteractDoor(ev);

							if (!ev.Allowed)
								return false;

							if (manaFromLabel > __instance._curMana)
							{
								Console.AddDebugLog("SCP079", "Not enough mana.", MessageImportance.LeastImportant, false);
								__instance.RpcNotEnoughMana(manaFromLabel, __instance._curMana);
								return false;
							}

							bool targetState = doorVariant.TargetState;
							doorVariant.ServerInteract(ReferenceHub.GetHub(__instance.gameObject), 0);

							if (targetState != doorVariant.TargetState)
							{
								__instance.Mana -= manaFromLabel;
								__instance.AddInteractionToHistory(target, true);
								Console.AddDebugLog("SCP079", "Door state changed.", global::MessageImportance.LeastImportant, false);
								return false;
							}

							Console.AddDebugLog("SCP079", "Door state failed to change.", MessageImportance.LeastImportant, false);
							return false;
						}

					case Command079.Doorlock:
						{
							if (AlphaWarheadController.Host.inProgress)
								return false;

							if (target is null)
							{
								Console.AddDebugLog("SCP079", "The door lock command requires a target.", global::MessageImportance.LessImportant, false);
								return false;
							}

							if (doorVariant is null)
								return false;

							if (doorVariant.TryGetComponent(out DoorNametagExtension doorNametagExtension) && list.Count > 0 && list.Contains(doorNametagExtension.GetName))
							{
								Console.AddDebugLog("SCP079", "Door access denied by the server.", global::MessageImportance.LeastImportant, false);
								return false;
							}

							DoorLockMode mode = DoorLockUtils.GetMode((DoorLockReason)doorVariant.ActiveLocks);
							if (!mode.HasFlagFast(DoorLockMode.CanOpen) && !mode.HasFlagFast(DoorLockMode.CanClose) && !mode.HasFlagFast(DoorLockMode.ScpOverride))
							{
								Console.AddDebugLog("SCP079", "Door is affected by interference and cannot be controlled by SCP-079.", MessageImportance.LeastImportant, false);
								return false;
							}

							if (((DoorLockReason)doorVariant.ActiveLocks).HasFlag(DoorLockReason.Regular079))
							{
								if (!__instance.lockedDoors.Contains(doorVariant.netId))
									return false;

								__instance.lockedDoors.Remove(doorVariant.netId);
								doorVariant.ServerChangeLock(DoorLockReason.Regular079, false);
								return false;
							}
							else
							{
								float manaFromLabel = __instance.GetManaFromLabel("Door Lock Minimum", __instance.abilities);

								var ev = new Scp079LockDoorEvent(player, doorVariant.GetDoor(), manaFromLabel);
								Qurre.Events.Invoke.Scp079.LockDoor(ev);

								if (!ev.Allowed)
									return false;

								if (manaFromLabel > __instance._curMana)
								{
									__instance.RpcNotEnoughMana(manaFromLabel, __instance._curMana);
									return false;
								}

								if (!__instance.lockedDoors.Contains(doorVariant.netId))
								{
									__instance.lockedDoors.Add(doorVariant.netId);
								}

								doorVariant.ServerChangeLock(DoorLockReason.Regular079, true);
								__instance.AddInteractionToHistory(doorVariant.gameObject, true);
								__instance.Mana -= __instance.GetManaFromLabel("Door Lock Start", __instance.abilities);

								return false;
							}
						}
					case Command079.Speaker:
						{
							string text2 = __instance.CurrentRoom.transform.parent.name + "/" + __instance.CurrentRoom.name + "/Scp079Speaker";
							GameObject gameObject = GameObject.Find(text2);
							float manaFromLabel = __instance.GetManaFromLabel("Speaker Start", __instance.abilities);

							var ev = new Scp079SpeakerEvent(player, gameObject, API.Objects.Scp079SpeakerType.StartSpeaker, manaFromLabel);
							Qurre.Events.Invoke.Scp079.Speaker(ev);

							if (!ev.Allowed)
								return false;

							if (manaFromLabel * 1.5f > __instance._curMana)
							{
								__instance.RpcNotEnoughMana(manaFromLabel, __instance._curMana);
								return false;
							}

							if (gameObject is not null)
							{
								__instance.Mana -= manaFromLabel;
								__instance.Speaker = text2;
								__instance.AddInteractionToHistory(gameObject, true);
								return false;
							}

							break;
						}
					case Command079.StopSpeaker:
						{
							var ev = new Scp079SpeakerEvent(player, GameObject.Find(__instance.Speaker), API.Objects.Scp079SpeakerType.StartSpeaker, 0f);
							Qurre.Events.Invoke.Scp079.Speaker(ev);

							if (ev.Allowed)
							    __instance.Speaker = string.Empty;

							return false;
						}
					case Command079.ElevatorTeleport:
						{
							float manaFromLabel = __instance.GetManaFromLabel("Elevator Teleport", __instance.abilities);

							var ev = new Scp079ElevatorTeleportEvent(player, manaFromLabel);
							Qurre.Events.Invoke.Scp079.ElevatorTeleport(ev);

							if (!ev.Allowed)
								return false;

							if (manaFromLabel > __instance._curMana)
							{
								__instance.RpcNotEnoughMana(manaFromLabel, __instance._curMana);
								return false;
							}

							Camera079 camera = null;

							foreach (Scp079Interactable scp079Interactable in __instance.nearbyInteractables)
							{
								if (scp079Interactable.type == Scp079Interactable.InteractableType.ElevatorTeleport)
								{
									camera = scp079Interactable.optionalObject.GetComponent<Camera079>();
								}
							}
							if (camera is not null)
							{
								__instance.RpcSwitchCamera(camera.cameraId, false);
								__instance.Mana -= manaFromLabel;
								__instance.AddInteractionToHistory(target, true);
								return false;
							}

							if (ConsoleDebugMode.CheckImportance("SCP079", MessageImportance.LeastImportant, out Color32 color))
							{
								Scp079Interactable scp079Interactable = null;
								Dictionary<Scp079Interactable.InteractableType, byte> dictionary = new();

								foreach (Scp079Interactable interactable in __instance.nearbyInteractables)
								{
									if (dictionary.ContainsKey(interactable.type))
									{
										Scp079Interactable.InteractableType type = interactable.type;
										dictionary[type] += 1;
									}
									else
									{
										dictionary[interactable.type] = 1;
									}
									if (interactable.type == Scp079Interactable.InteractableType.ElevatorTeleport)
									{
										scp079Interactable = interactable;
									}
								}
								string text3;
								if (scp079Interactable is null)
								{
									text3 = "None of the " + __instance.nearbyInteractables.Count + " were an ElevatorTeleport, found: ";
									using (Dictionary<Scp079Interactable.InteractableType, byte>.Enumerator enumerator2 = dictionary.GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											KeyValuePair<Scp079Interactable.InteractableType, byte> keyValuePair = enumerator2.Current;

											text3 = string.Concat(new object[]
											{
							                    text3,
												keyValuePair.Value,
												"x",
												keyValuePair.Key.ToString().Substring(keyValuePair.Key.ToString().Length - 4),
												" "
											});
										}

										Console.AddDebugLog("SCP079", "Could not find the second elevator: " + text3, MessageImportance.LeastImportant, false);
										return false;
									}
								}

								if (scp079Interactable.optionalObject is null)
									text3 = "Optional object is missing.";
								else if (scp079Interactable.optionalObject.GetComponent<global::Camera079>() == null)
								{
									string str = "";
									Transform transform = scp079Interactable.optionalObject.transform;

									for (int i = 0; i < 5; i++)
									{
										str = transform.name + str;

										if (transform.parent is  null)
										{
											break;
										}

										transform = transform.parent;
									}

									text3 = "Camera is missing at " + str;
								}
								else
									text3 = "Unknown error";
							}

							break;
						}
					case Command079.ElevatorUse:
						{
							float manaFromLabel = __instance.GetManaFromLabel("Elevator Use", __instance.abilities);
							args ??= string.Empty;

							var ev = new Scp079InteractLiftEvent(player, UnityEngine.Object.FindObjectsOfType<Lift>().FirstOrDefault(x => x.elevatorName == args).GetLift(), manaFromLabel);
							Qurre.Events.Invoke.Scp079.InteractLift(ev);

							if (!ev.Allowed)
								return false;

							if (manaFromLabel > __instance._curMana)
							{
								__instance.RpcNotEnoughMana(manaFromLabel, __instance._curMana);
								return false;
							}

							foreach (Lift lift in UnityEngine.Object.FindObjectsOfType<Lift>())
							{
								if (lift.elevatorName == args && lift.UseLift())
								{
									__instance.Mana -= manaFromLabel;
									bool flag2 = false;
									foreach (Lift.Elevator elevator in lift.elevators)
									{
										__instance.AddInteractionToHistory(elevator.door.GetComponentInParent<global::Scp079Interactable>().gameObject, !flag2);
										flag2 = true;
									}
								}
							}

							return false;
						}
					case Command079.Tesla:
						{
							float manaFromLabel = __instance.GetManaFromLabel("Tesla Gate Burst", __instance.abilities);

							var ev = new Scp079InteractTeslaEvent(player, __instance.CurrentRoom.GetComponentInChildren<TeslaGate>().GetTesla(), manaFromLabel);
							Qurre.Events.Invoke.Scp079.InteractTesla(ev);

							if (!ev.Allowed)
								return false;

							if (manaFromLabel > __instance._curMana)
							{
								__instance.RpcNotEnoughMana(manaFromLabel, __instance._curMana);
								return false;
							}

							if (__instance.CurrentRoom is not null)
							{
								TeslaGate tesla = __instance.CurrentRoom.GetComponentInChildren<TeslaGate>();
								if (tesla is not null)
								{
									if (tesla.InactiveTime > 0f)
									{
										Console.AddDebugLog("SCP079", "Tesla gate burst cannot be used, the gate is inactive.", global::MessageImportance.LessImportant, false);
										return false;
									}
									if (!tesla.GetTesla().Allow079Interact)
										return false;

									tesla.RpcInstantBurst();
								}
								__instance.AddInteractionToHistory(tesla.gameObject, true);
								__instance.Mana -= manaFromLabel;
								return false;
							}

							throw new Exception("Unable to find Tesla Gate in a null room");
						}
					case Command079.Lockdown:
						{
							if (AlphaWarheadController.Host.inProgress)
							{
								Console.AddDebugLog("SCP079", "Lockdown cannot commence, Warhead in progress.", MessageImportance.LessImportant, false);
								return false;
							}

							float manaFromLabel = __instance.GetManaFromLabel("Room Lockdown", __instance.abilities);

							if (manaFromLabel > __instance._curMana)
							{
								__instance.RpcNotEnoughMana(manaFromLabel, __instance._curMana);
								Console.AddDebugLog("SCP079", "Lockdown cannot commence, not enough mana.", global::MessageImportance.LessImportant, false);
								return false;
							}

							Console.AddDebugLog("SCP079", "Attempting lockdown...", MessageImportance.LeastImportant, false);
							if (__instance.CurrentRoom is not null)
							{
								HashSet<Scp079Interactable> hashSet = Scp079Interactable.InteractablesByRoomId[__instance.CurrentRoom.UniqueId];
								Console.AddDebugLog("SCP079", "Loaded all interactables", MessageImportance.LeastImportant, false);
								GameObject go = null;
								IDamageableDoor damageableDoor;

								foreach (Scp079Interactable scp079Interactable in hashSet)
								{
									if (scp079Interactable is not null)
									{
										Scp079Interactable.InteractableType type = scp079Interactable.type;
										if (type != Scp079Interactable.InteractableType.Door)
										{
											if (type == Scp079Interactable.InteractableType.Lockdown)
											{
												go = scp079Interactable.gameObject;
											}
										}
										else if (scp079Interactable.TryGetComponent(out DoorVariant doorVariant2) && (damageableDoor = (doorVariant2 as IDamageableDoor)) is not null && damageableDoor.IsDestroyed)
										{
											Console.AddDebugLog("SCP079", "Lockdown can't initiate, one of the doors were destroyed.", global::MessageImportance.LessImportant, false);
											return false;
										}
									}
								}

								if (__instance.CurrentLDCooldown > 0f)
								{
									Console.AddDebugLog("SCP079", "Lockdown still on cooldown.", global::MessageImportance.LessImportant, false);
									return false;
								}

								HashSet<DoorVariant> hashSet2 = new HashSet<DoorVariant>();
								Console.AddDebugLog("SCP079", "Looking for doors to lock...", MessageImportance.LeastImportant, false);

								var ev = new Scp079LockdownEvent(player,
									__instance.CurrentRoom.GetRoom(), 
									hashSet.Where(x => x is not null && x.TryGetComponent(out DoorVariant variant) 
									&& variant is not null 
									&& variant.GetDoor() is not null)
									.Select(x => x.GetComponent<DoorVariant>().GetDoor()).ToList(),
									manaFromLabel);

								Qurre.Events.Invoke.Scp079.Lockdown(ev);

								if (!ev.Allowed)
									return false;

								foreach (Scp079Interactable scp079Interactable in hashSet)
								{
                                    if (scp079Interactable is not null && scp079Interactable.TryGetComponent(out DoorVariant doorVariant3))
                                    {
                                        bool flag3 = doorVariant3.ActiveLocks == 0;
                                        if (!flag3)
                                        {
                                            DoorLockMode mode2 = DoorLockUtils.GetMode((DoorLockReason)doorVariant3.ActiveLocks);
                                            flag3 = (mode2.HasFlagFast(DoorLockMode.CanClose) || mode2.HasFlagFast(DoorLockMode.ScpOverride));
                                        }
                                        if (flag3)
                                        {
											if (doorVariant3.TargetState)
                                            {
                                                doorVariant3.NetworkTargetState = false;
                                            }
                                            doorVariant3.ServerChangeLock(DoorLockReason.Lockdown079, true);
                                            doorVariant3.UnlockLater(__instance.LockdownDuration, DoorLockReason.Lockdown079);
											hashSet2.Add(doorVariant3);
										}
									}
                                }

								foreach (FlickerableLightController flickerableLightController in __instance.CurrentRoom.GetComponentsInChildren<FlickerableLightController>())
								{
									if (flickerableLightController != null)
									{
										flickerableLightController.ServerFlickerLights(8f);
									}
								}
								__instance.CurrentLDCooldown = __instance.LockdownCooldown + __instance.LockdownDuration;
								foreach (ReferenceHub referenceHub in __instance._referenceHub.spectatorManager.ServerCurrentSpectatingPlayers)
								{
									__instance.TargetSetLockdownCooldown(referenceHub.networkIdentity.connectionToClient, __instance.CurrentLDCooldown);
								}
								Console.AddDebugLog("SCP079", "Lockdown initiated.", global::MessageImportance.LessImportant, false);
								__instance.AddInteractionToHistory(go, true);
								__instance.Mana -= __instance.GetManaFromLabel("Room Lockdown", __instance.abilities);
								return false;
							}
							else
							{
								Console.AddDebugLog("SCP079", "Room couldn't be specified.", MessageImportance.Normal, false);
							}
							break;
						}
					default:
						return false;
				}

				return false;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching SCPs -> SCP079 [Interact]:\n{e}\n{e.StackTrace}");
				return true;
			}
        }
    }
}
