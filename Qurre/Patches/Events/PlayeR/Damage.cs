#pragma warning disable SA1313
using System;
using CustomPlayerEffects;
using Dissonance.Integrations.MirrorIgnorance;
using HarmonyLib;
using Mirror;
using PlayableScps;
using PlayableScps.Interfaces;
using Qurre.API;
using Qurre.API.Events;
using RemoteAdmin;
using Respawning;
using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.HurtPlayer))]
    internal static class Damage
	{
		public static PlayerStats.HitInfo LastInfo { get; set; }
		public static GameObject LastTarget { get; set; }

		public static bool Prefix(PlayerStats __instance, PlayerStats.HitInfo info, GameObject go, bool noTeamDamage = false, bool IsValidDamage = false)
		{
			try
			{
				LastInfo = info;
				LastTarget = go;
				bool flag = false;
				bool flag2 = false;
				bool flag3 = go == null;
				ReferenceHub referenceHub = flag3 ? null : ReferenceHub.GetHub(go);

				if (info.Amount < 0f)
				{
					if (flag3)
					{
						info.Amount = Mathf.Abs(999999f);
					}
					else
					{
						info.Amount = ((referenceHub.playerStats != null) ? Mathf.Abs(referenceHub.playerStats.Health + referenceHub.playerStats.syncArtificialHealth + 10f) : Mathf.Abs(999999f));
					}
				}

				if (referenceHub != null)
				{
					Burned effect = referenceHub.playerEffectsController.GetEffect<Burned>();
					if (effect != null && effect.Enabled)
					{
						info.Amount *= effect.DamageMult;
					}
				}

				if (info.Amount > 2.14748365E+09f) info.Amount = 2.14748365E+09f;
				if (info.GetDamageType().isWeapon && referenceHub.characterClassManager.IsAnyScp() && info.GetDamageType() != DamageTypes.MicroHid) info.Amount *= __instance.PlayerStats_weaponManager().weapons[__instance.PlayerStats_weaponManager().curWeapon].scpDamageMultiplier;
				if (flag3) return false;
				PlayerStats playerStats = referenceHub.playerStats;
				CharacterClassManager characterClassManager = referenceHub.characterClassManager;
				if (playerStats == null || characterClassManager == null) return false;
				if (characterClassManager.GodMode) return false;
				if (__instance.ccm.CurRole.team == Team.SCP && __instance.ccm.Classes.SafeGet(characterClassManager.CurClass).team == Team.SCP && __instance.ccm != characterClassManager) return false;
				if (characterClassManager.SpawnProtected && !__instance.PlayerStats_allowSPDmg()) return false;
				bool flag4 = !noTeamDamage && info.IsPlayer && referenceHub != info.RHub && referenceHub.characterClassManager.Fraction == info.RHub.characterClassManager.Fraction;
				if (flag4) info.Amount *= FriendlyFireFactor;
				Player myTarget = Player.Get(referenceHub);
				Player myPlayer = info.GetDamageType() == DamageTypes.Grenade ? Player.Get(info.PlayerId) : Player.Get(__instance.ccm.CCM_hub());
				if (myTarget == null || myPlayer == null) return true;
				var ev = new DamageEvent(myPlayer, myTarget, info);
				Qurre.Events.Player.damage(ev);
				if (!ev.Allowed) return false;
				float health = playerStats.Health;
				if (__instance.lastHitInfo.Attacker == "ARTIFICIALDEGEN")
				{
					playerStats.unsyncedArtificialHealth -= info.Amount;
					if (playerStats.unsyncedArtificialHealth < 0f)
					{
						playerStats.unsyncedArtificialHealth = 0f;
					}
				}
				else
				{
					if (playerStats.unsyncedArtificialHealth > 0f)
					{
						float num = info.Amount * playerStats.artificialNormalRatio;
						float num2 = info.Amount - num;
						playerStats.unsyncedArtificialHealth -= num;
						if (playerStats.unsyncedArtificialHealth < 0f)
						{
							num2 += Mathf.Abs(playerStats.unsyncedArtificialHealth);
							playerStats.unsyncedArtificialHealth = 0f;
						}

						playerStats.Health -= num2;
						if (playerStats.Health > 0f && playerStats.Health - num <= 0f && characterClassManager.CurRole.team != Team.SCP)
							__instance.TargetAchieve(characterClassManager.connectionToClient, "didntevenfeelthat");
					}
					else
					{
						playerStats.Health -= info.Amount;
					}

					if (playerStats.Health < 0f)
						playerStats.Health = 0f;
					playerStats.lastHitInfo = info;
				}
				PlayableScpsController component = go.GetComponent<PlayableScpsController>();
				IDamagable damagable;
				if (component != null && (damagable = (component.CurrentScp as IDamagable)) != null)
					damagable.OnDamage(info);
				if (playerStats.Health < 1f && characterClassManager.CurClass != RoleType.Spectator)
				{
					IImmortalScp immortalScp;
					if (component != null && (immortalScp = (component.CurrentScp as IImmortalScp)) != null && !immortalScp.OnDeath(info, __instance.gameObject))
						return false;
					foreach (Scp079PlayerScript scp079PlayerScript in Scp079PlayerScript.instances)
					{
						Scp079Interactable.ZoneAndRoom otherRoom = myTarget.ReferenceHub.scp079PlayerScript.GetOtherRoom();
						bool flag5 = false;
						foreach (Scp079Interaction scp079Interaction in scp079PlayerScript.ReturnRecentHistory(12f, __instance.PlayerStats_filters()))
						{
							foreach (Scp079Interactable.ZoneAndRoom zoneAndRoom in scp079Interaction.interactable.currentZonesAndRooms)
							{
								if (zoneAndRoom.currentZone == otherRoom.currentZone && zoneAndRoom.currentRoom == otherRoom.currentRoom)
								{
									flag5 = true;
								}
							}
						}

						if (flag5)
							scp079PlayerScript.RpcGainExp(ExpGainType.KillAssist, characterClassManager.CurClass);
					}

					if (RoundSummary.RoundInProgress() && RoundSummary.roundTime < 60 && IsValidDamage)
						__instance.TargetAchieve(characterClassManager.connectionToClient, "wowreally");
					if (__instance.isLocalPlayer && info.PlayerId != referenceHub.queryProcessor.PlayerId)
						RoundSummary.Kills++;
					flag = true;
					if (characterClassManager.CurClass == RoleType.Scp096)
					{
						ReferenceHub hub = ReferenceHub.GetHub(go);
						if (hub != null && hub.scpsController.CurrentScp is Scp096 && (hub.scpsController.CurrentScp as Scp096).PlayerState == Scp096PlayerState.Enraging)
							__instance.TargetAchieve(characterClassManager.connectionToClient, "unvoluntaryragequit");
					}
					else if (info.GetDamageType() == DamageTypes.Pocket)
						__instance.TargetAchieve(characterClassManager.connectionToClient, "newb");
					else if (info.GetDamageType() == DamageTypes.Scp173)
						__instance.TargetAchieve(characterClassManager.connectionToClient, "firsttime");
					else if (info.GetDamageType() == DamageTypes.Grenade && info.PlayerId == referenceHub.queryProcessor.PlayerId)
						__instance.TargetAchieve(characterClassManager.connectionToClient, "iwanttobearocket");
					else if (info.GetDamageType().isWeapon)
					{
						Inventory inventory = referenceHub.inventory;
						if (characterClassManager.CurClass == RoleType.Scientist)
						{
							Item itemByID = inventory.GetItemByID(inventory.curItem);
							if (itemByID != null && itemByID.itemCategory == ItemCategory.Keycard && __instance.GetComponent<CharacterClassManager>().CurClass == RoleType.ClassD)
								__instance.TargetAchieve(__instance.connectionToClient, "betrayal");
						}

						if (Time.realtimeSinceStartup - __instance.PlayerStats_killStreakTime() > 30f || __instance.PlayerStats_killStreak() == 0)
						{
							__instance.PlayerStats_killStreak(0);
							__instance.PlayerStats_killStreakTime((int)Time.realtimeSinceStartup);
						}

						if (myPlayer.ReferenceHub.weaponManager.GetShootPermission(characterClassManager, true))
							__instance.PlayerStats_killStreak(__instance.PlayerStats_killStreak()+1);
						if (__instance.PlayerStats_killStreak() >= 5)
							__instance.TargetAchieve(__instance.connectionToClient, "pewpew");
						if ((__instance.ccm.CurRole.team == Team.MTF || __instance.ccm.Classes.SafeGet(__instance.ccm.CurClass).team == Team.RSC) && characterClassManager.CurClass == RoleType.ClassD)
							__instance.TargetStats(__instance.connectionToClient, "dboys_killed", "justresources", 50);
					}
					else if (__instance.ccm.CurRole.team == Team.SCP && go.GetComponent<MicroHID>().CurrentHidState != MicroHID.MicroHidState.Idle)
						__instance.TargetAchieve(__instance.connectionToClient, "illpassthanks");
					if (__instance.ccm.CurRole.team == Team.RSC && __instance.ccm.Classes.SafeGet(characterClassManager.CurClass).team == Team.SCP)
						__instance.TargetAchieve(__instance.connectionToClient, "timetodoitmyself");
					var dE = new DiesEvent(ev.Attacker, ev.Target, ev.HitInformations);
					Qurre.Events.Player.dies(dE);
					if (!dE.Allowed) return false;
					bool flag6 = info.IsPlayer && referenceHub == info.RHub;
					flag2 = flag4;
					if (flag6)
					{
						ServerLogs.AddLog(ServerLogs.Modules.ClassChange, string.Concat(new string[]
						{
					referenceHub.LoggedNameFromRefHub(),
					" playing as ",
					referenceHub.characterClassManager.CurRole.fullName,
					" committed a suicide using ",
					info.GetDamageName(),
					"."
						}), ServerLogs.ServerLogType.Suicide, false);
					}
					else
					{
						ServerLogs.AddLog(ServerLogs.Modules.ClassChange, string.Concat(new string[]
						{
					referenceHub.LoggedNameFromRefHub(),
					" playing as ",
					referenceHub.characterClassManager.CurRole.fullName,
					" has been killed by ",
					info.Attacker,
					" using ",
					info.GetDamageName(),
					info.IsPlayer ? (" playing as " + info.RHub.characterClassManager.CurRole.fullName + ".") : "."
						}), flag2 ? ServerLogs.ServerLogType.Teamkill : ServerLogs.ServerLogType.KillLog, false);
					}
					if (info.GetDamageType().isScp || info.GetDamageType() == DamageTypes.Pocket)
					{
						RoundSummary.kills_by_scp++;
					}
					else if (info.GetDamageType() == DamageTypes.Grenade)
					{
						RoundSummary.kills_by_frag++;
					}
					if (!__instance.PlayerStats_pocketCleanup() || info.GetDamageType() != DamageTypes.Pocket)
					{
						referenceHub.inventory.ServerDropAll();
						PlayerMovementSync playerMovementSync = referenceHub.playerMovementSync;
						if (characterClassManager.Classes.CheckBounds(characterClassManager.CurClass) && info.GetDamageType() != DamageTypes.RagdollLess)
						{
							__instance.GetComponent<RagdollManager>().SpawnRagdoll(go.transform.position, go.transform.rotation, (playerMovementSync == null) ? Vector3.zero : playerMovementSync.PlayerVelocity, (int)characterClassManager.CurClass, info, characterClassManager.CurRole.team > Team.SCP, go.GetComponent<MirrorIgnorancePlayer>().PlayerId, referenceHub.nicknameSync.DisplayName, referenceHub.queryProcessor.PlayerId);
						}
					}
					else
					{
						referenceHub.inventory.Clear();
					}
					characterClassManager.NetworkDeathPosition = go.transform.position;
					if (characterClassManager.CurRole.team == Team.SCP)
					{
						if (characterClassManager.CurClass == RoleType.Scp0492)
						{
							NineTailedFoxAnnouncer.CheckForZombies(go);
						}
						else
						{
							GameObject x = null;
							foreach (GameObject gameObject in PlayerManager.players)
							{
								if (gameObject.GetComponent<QueryProcessor>().PlayerId == info.PlayerId)
								{
									x = gameObject;
								}
							}
							if (x != null)
							{
								NineTailedFoxAnnouncer.AnnounceScpTermination(characterClassManager.CurRole, info, string.Empty);
							}
							else
							{
								DamageTypes.DamageType damageType = info.GetDamageType();
								if (damageType == DamageTypes.Tesla)
								{
									NineTailedFoxAnnouncer.AnnounceScpTermination(characterClassManager.CurRole, info, "TESLA");
								}
								else if (damageType == DamageTypes.Nuke)
								{
									NineTailedFoxAnnouncer.AnnounceScpTermination(characterClassManager.CurRole, info, "WARHEAD");
								}
								else if (damageType == DamageTypes.Decont)
								{
									NineTailedFoxAnnouncer.AnnounceScpTermination(characterClassManager.CurRole, info, "DECONTAMINATION");
								}
								else if (characterClassManager.CurClass != RoleType.Scp079)
								{
									NineTailedFoxAnnouncer.AnnounceScpTermination(characterClassManager.CurRole, info, "UNKNOWN");
								}
							}
						}
					}
					playerStats.SetHPAmount(100);
					characterClassManager.SetClassID(RoleType.Spectator);
				}
				else
				{
					Vector3 pos = Vector3.zero;
					float num3 = 40f;
					if (info.GetDamageType().isWeapon)
					{
						GameObject playerOfID = __instance.PlayerStats_GetPlayerOfID(info.PlayerId);
						if (playerOfID != null)
						{
							pos = go.transform.InverseTransformPoint(playerOfID.transform.position).normalized;
							num3 = 100f;
						}
					}
					else if (info.GetDamageType() == DamageTypes.Pocket)
					{
						PlayerMovementSync component2 = __instance.ccm.GetComponent<PlayerMovementSync>();
						if (component2.RealModelPosition.y > -1900f)
						{
							component2.OverridePosition(Vector3.down * 1998.5f, 0f, true);
						}
					}
					__instance.CallTargetBloodEffect(go.GetComponent<NetworkIdentity>().connectionToClient, pos, Mathf.Clamp01(info.Amount / num3));
				}
				RespawnTickets singleton = RespawnTickets.Singleton;
				Team team = characterClassManager.CurRole.team;
				byte b = (byte)team;
				if (b != 0)
				{
					if (b == 3)
					{
						if (flag)
						{
							Team team2 = __instance.ccm.Classes.SafeGet(characterClassManager.CurClass).team;
							if (team2 == Team.CDP && team2 == Team.CHI)
							{
								singleton.GrantTickets(SpawnableTeamType.ChaosInsurgency, __instance.PlayerStats_respawn_tickets_ci_scientist_died_count(), false);
							}
						}
					}
				}
				else if (characterClassManager.CurClass != RoleType.Scp0492)
				{
					for (float num4 = 1f; num4 > 0f; num4 -= __instance.PlayerStats_respawn_tickets_mtf_scp_hurt_interval())
					{
						float num5 = (float)playerStats.maxHP * num4;
						if (health > num5 && playerStats.Health < num5)
						{
							singleton.GrantTickets(SpawnableTeamType.NineTailedFox, __instance.PlayerStats_respawn_tickets_mtf_scp_hurt_count(), false);
						}
					}
				}
				IDamagable damagable2;
				if (component != null && (damagable2 = (component.CurrentScp as IDamagable)) != null)
				{
					damagable2.OnDamage(info);
				}
				if (!flag4|| PermissionsHandler.IsPermitted(info.RHub.serverRoles.Permissions, PlayerPermissions.FriendlyFireDetectorImmunity))
				{
					return flag;
				}
				if (referenceHub.characterClassManager.CurRole.team == Team.CDP && info.RHub.characterClassManager.CurRole.team == Team.CDP)
				{
					return flag;
				}
				if (flag2)
				{
					if (info.RHub.FriendlyFireHandlerRespawn())
					{
						return flag;
					}
					if (info.RHub.FriendlyFireHandlerWindow())
					{
						return flag;
					}
					if (info.RHub.FriendlyFireHandlerLife())
					{
						return flag;
					}
					if (info.RHub.FriendlyFireHandlerRound())
					{
						return flag;
					}
				}
				if (info.RHub.FriendlyFireHandlerRespawn(info.Amount))
				{
					return flag;
				}
				if (info.RHub.FriendlyFireHandlerWindow(info.Amount))
				{
					return flag;
				}
				if (info.RHub.FriendlyFireHandlerLife(info.Amount))
				{
					return flag;
				}
				info.RHub.FriendlyFireHandlerRound(info.Amount);
				return flag;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching PlayeR.Damage:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
    }
}