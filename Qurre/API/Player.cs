using Qurre.API.Objects;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static QurreModLoader.umm;
using Hints;
namespace Qurre.API
{
	public static class Player
	{
		private static MethodInfo sendSpawnMessage;
		public static MethodInfo SendSpawnMessage
		{
			get
			{
				if (sendSpawnMessage == null)
					sendSpawnMessage = typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.Instance | BindingFlags.InvokeMethod
						| BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
				return sendSpawnMessage;
			}
		}
		public static Dictionary<int, ReferenceHub> IdHubs = new Dictionary<int, ReferenceHub>();
		public static Dictionary<string, ReferenceHub> StrHubs = new Dictionary<string, ReferenceHub>();
		public static IEnumerable<ReferenceHub> GetHubs() => ReferenceHub.GetAllHubs().Values.Where(h => !h.IsHost());
		public static string GetUserId(this ReferenceHub player) => player.characterClassManager.UserId;
		public static void SetUserId(this ReferenceHub player, string newId) => player.characterClassManager.NetworkSyncedUserId = newId;
		public static int GetPlayerId(this ReferenceHub player) => player.queryProcessor.PlayerId;
		public static void SetPlayerId(this ReferenceHub player, int newId) => player.queryProcessor.NetworkPlayerId = newId;
		public static bool Overwatch(this ReferenceHub player) => player.serverRoles.OverwatchEnabled();
		public static void Overwatch(this ReferenceHub player, bool over) => player.serverRoles.CallTargetSetOverwatch(player.networkIdentity.connectionToClient, over);
		public static bool ItsScp(this ReferenceHub hub) => hub.characterClassManager.IsAnyScp();
		public static bool ItsNTF(this ReferenceHub hub)
		{
			switch (hub.GetRole())
			{
				case RoleType.NtfCadet:
				case RoleType.NtfScientist:
				case RoleType.NtfLieutenant:
				case RoleType.NtfCommander:
					return true;
				default:
					return false;
			}
		}
		public static RoleType GetRole(this ReferenceHub player) => player.characterClassManager.CurClass;
		public static void SetRole(this ReferenceHub player, RoleType newRole) => player.characterClassManager.SetPlayersClass(newRole, player.gameObject);
		public static void SetRole(this ReferenceHub player, RoleType newRole, bool keepPosition)
		{
			if (keepPosition)
			{
				player.characterClassManager.NetworkCurClass = newRole;
				player.playerStats.SetHPAmount(player.characterClassManager.Classes.SafeGet(player.GetRole()).maxHP);
			}
			else
				SetRole(player, newRole);
		}
		public static Vector3 GetPosition(this ReferenceHub player) => player.playerMovementSync.transform.position;
		public static Vector2 GetRotations(this ReferenceHub player) => player.playerMovementSync.NetworkRotationSync;
		public static Vector3 GetRotationVector(this ReferenceHub player) => player.characterClassManager.transform.forward;
		public static void SetPosition(this ReferenceHub player, float x, float y, float z) => player.playerMovementSync.OverridePosition(new Vector3(x, y, z), player.transform.rotation.eulerAngles.y);
		public static void SetPosition(this ReferenceHub player, Vector3 position) => player.SetPosition(position.x, position.y, position.z);
		public static void SetRotation(this ReferenceHub player, Vector2 rotations) => player.SetRotation(rotations.x, rotations.y);
		public static void SetRotation(this ReferenceHub player, float x, float y) => player.playerMovementSync.NetworkRotationSync = new Vector2(x, y);
		public static UserGroup GetGroup(this ReferenceHub player) => player.serverRoles.UserGroup();
		public static void SetGroupColor(this ReferenceHub player, string color) => player.serverRoles.SetColor(color);
		public static void SetGroupName(this ReferenceHub player, string name) => player.serverRoles.SetText(name);
		public static void SetGroup(this ReferenceHub player, string name, string color, bool show)
		{
			UserGroup ug = new UserGroup()
			{
				BadgeColor = color,
				BadgeText = name,
				HiddenByDefault = !show,
				Cover = show
			};

			player.serverRoles.SetGroup(ug, false, false, show);
		}
		public static void SetGroup(this ReferenceHub player, string name, string color, bool show, string rankName)
		{
			if (ServerStatic.GetPermissionsHandler().GetAllGroups().ContainsKey(rankName))
			{
				ServerStatic.GetPermissionsHandler().GetGroup(rankName).BadgeColor = color;
				ServerStatic.GetPermissionsHandler().GetGroup(rankName).BadgeText = name;
				ServerStatic.GetPermissionsHandler().GetGroup(rankName).HiddenByDefault = !show;
				ServerStatic.GetPermissionsHandler().GetGroup(rankName).Cover = show;


				player.serverRoles.SetGroup(ServerStatic.GetPermissionsHandler().GetGroup(rankName), false, false, show);
			}
			else
			{
				UserGroup ug = new UserGroup()
				{
					BadgeColor = color,
					BadgeText = name,
					HiddenByDefault = !show,
					Cover = show
				};

				ServerStatic.GetPermissionsHandler().GetAllGroups().Add(rankName, ug);
				player.serverRoles.SetGroup(ug, false, false, show);
			}

			if (ServerStatic.GetPermissionsHandler()._members().ContainsKey(player.GetUserId()))
			{
				ServerStatic.GetPermissionsHandler()._members()[player.GetUserId()] = rankName;
			}
			else
			{
				ServerStatic.GetPermissionsHandler()._members().Add(player.GetUserId(), rankName);
			}
		}
		public static void SetGroup(this ReferenceHub player, UserGroup userGroup) => player.serverRoles.SetGroup(userGroup, false, false, false);
		public static GlobalBadge GetGlobalBadge(this ReferenceHub player)
		{
			string token = player.serverRoles.NetworkGlobalBadge;
			if (string.IsNullOrEmpty(token)) { return null; }
			Dictionary<string, string> dictionary = (from rwr in token.Split(new string[]
		   {
						   "<br>"
		  }, StringSplitOptions.None)
													 select rwr.Split(new string[]
													 {
						   ": "
													 }, StringSplitOptions.None)).ToDictionary((string[] split) => split[0], (string[] split) => split[1]);

			int BadgeType = 0;
			if (int.TryParse(dictionary["Badge type"], out int type)) { BadgeType = type; }

			return new GlobalBadge
			{
				BadgeText = dictionary["Badge text"],
				BadgeColor = dictionary["Badge color"],
				Type = BadgeType
			};
		}
		public static string GetNickname(this ReferenceHub player) => player.nicknameSync.Network_myNickSync;
		public static void SetNickname(this ReferenceHub player, string nickname)
		{
			player.nicknameSync.Network_myNickSync = nickname;
			MEC.Timing.RunCoroutine(BlinkTag(player));
		}

		private static IEnumerator<float> BlinkTag(ReferenceHub player)
		{
			yield return MEC.Timing.WaitForOneFrame;

			player.HideTag();

			yield return MEC.Timing.WaitForOneFrame;

			player.ShowTag();
		}
		private static void HideTag(this ReferenceHub player) => player.characterClassManager.CallCmdRequestHideTag();
		private static void ShowTag(this ReferenceHub player, bool isGlobal = false) => player.characterClassManager.CallCmdRequestShowTag(isGlobal);
		public static void RAMessage(this CommandSender sender, string message, bool success = true, string pluginName = null)
		{
			sender.RaReply((pluginName ?? Assembly.GetCallingAssembly().GetName().Name) + "#" + message, success, true, string.Empty);
		}
		public static void Broadcast(this ReferenceHub player, ushort time, string message, Broadcast.BroadcastFlags flag = 0) => Map.BroadcastComponent.TargetAddElement(player.scp079PlayerScript.connectionToClient, message, time, flag);
		public static void ClearBroadcasts(this ReferenceHub player) => Map.BroadcastComponent.TargetClearElements(player.scp079PlayerScript.connectionToClient);
		public static Team GetTeam(this ReferenceHub player) => player.GetRole().GetTeam();
		public static Team GetTeam(this RoleType roleType)
		{
			switch (roleType)
			{
				case RoleType.ChaosInsurgency:
					return Team.CHI;
				case RoleType.Scientist:
					return Team.RSC;
				case RoleType.ClassD:
					return Team.CDP;
				case RoleType.Scp049:
				case RoleType.Scp93953:
				case RoleType.Scp93989:
				case RoleType.Scp0492:
				case RoleType.Scp079:
				case RoleType.Scp096:
				case RoleType.Scp106:
				case RoleType.Scp173:
					return Team.SCP;
				case RoleType.Spectator:
					return Team.RIP;
				case RoleType.FacilityGuard:
				case RoleType.NtfCadet:
				case RoleType.NtfLieutenant:
				case RoleType.NtfCommander:
				case RoleType.NtfScientist:
					return Team.MTF;
				case RoleType.Tutorial:
					return Team.TUT;
				default:
					return Team.RIP;
			}
		}
		public static Side GetSide(this RoleType type) => type.GetTeam().GetSide();
		public static Side GetSide(this Team team)
		{
			switch (team)
			{
				case Team.SCP:
					return Side.SCP;
				case Team.MTF:
				case Team.RSC:
					return Side.MTF;
				case Team.CHI:
				case Team.CDP:
					return Side.CHAOS;
				case Team.TUT:
					return Side.TUTORIAL;
				case Team.RIP:
				default: return Side.NONE;
			}
		}
		public static Side GetSide(this ReferenceHub hub) => hub.GetTeam().GetSide();
		public static ReferenceHub Get(this GameObject player) => ReferenceHub.GetHub(player);
		public static ReferenceHub Get(int playerId)
		{
			if (IdHubs.ContainsKey(playerId))
				return IdHubs[playerId];

			foreach (ReferenceHub hub in GetHubs())
			{
				if (hub.GetPlayerId() == playerId)
				{
					IdHubs.Add(playerId, hub);

					return hub;
				}
			}

			return null;
		}
		public static ReferenceHub Get(string args)
		{
			try
			{
				if (StrHubs.ContainsKey(args))
					return StrHubs[args];

				ReferenceHub playerFound = null;

				if (short.TryParse(args, out short playerId))
					return Get(playerId);

				if (args.EndsWith("@steam") || args.EndsWith("@discord") || args.EndsWith("@northwood") || args.EndsWith("@patreon"))
				{
					foreach (ReferenceHub player in GetHubs())
					{
						if (player.GetUserId() == args)
						{
							playerFound = player;
						}
					}
				}
				else
				{
					if (args == "WORLD" || args == "SCP-018" || args == "SCP-575" || args == "SCP-207")
						return null;

					int maxNameLength = 31, lastnameDifference = 31;
					string str1 = args.ToLower();

					foreach (ReferenceHub player in GetHubs())
					{
						if (!player.GetNickname().ToLower().Contains(args.ToLower()))
							continue;

						if (str1.Length < maxNameLength)
						{
							int nameDifference;
							int x = maxNameLength - str1.Length;
							int y = maxNameLength - player.GetNickname().Length;
							string str2 = player.GetNickname();
							for (int i = 0; i < x; i++) str1 += "z";
							for (int i = 0; i < y; i++) str2 += "z";
							int n = str1.Length;
							int m = str2.Length;
							int[,] d = new int[n + 1, m + 1];
							if (n == 0)
								nameDifference = m;
							if (m == 0)
								nameDifference = n;
							for (int i = 1; i <= n; i++)
								for (int j = 1; j <= m; j++)
								{
									int cost = (str2[j - 1] == str1[i - 1]) ? 0 : 1;
									d[i, j] = Math.Min(
										Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
										d[i - 1, j - 1] + cost);
								}
							nameDifference = d[n, m];
							if (nameDifference < lastnameDifference)
							{
								lastnameDifference = nameDifference;
								playerFound = player;
							}
						}
					}
				}
				if (playerFound != null)
					StrHubs.Add(args, playerFound);
				return playerFound;
			}
			catch (Exception exception)
			{
				Log.Error($"GetPlayer error: {exception}");
				return null;
			}
		}
		public static Room GetCurrentRoom(this ReferenceHub player)
		{
			Vector3 playerPos = player.GetPosition();
			Vector3 end = playerPos - new Vector3(0f, 10f, 0f);
			bool flag = Physics.Linecast(playerPos, end, out RaycastHit raycastHit, -84058629);

			if (!flag || raycastHit.transform == null)
				return null;

			Transform transform = raycastHit.transform;

			while (transform.parent != null && transform.parent.parent != null)
				transform = transform.parent;

			foreach (Room room in Map.Rooms)
				if (room.Position == transform.position)
					return room;

			return new Room
			{
				Name = transform.name,
				Position = transform.position,
				Transform = transform
			};
		}
		public static void Mute(this ReferenceHub player) => player.characterClassManager.NetworkMuted = true;
		public static void Unmute(this ReferenceHub player) => player.characterClassManager.NetworkMuted = false;
		public static bool IsMuted(this ReferenceHub player) => player.characterClassManager.NetworkMuted;
		public static void IntercomMute(this ReferenceHub player) => player.characterClassManager.NetworkIntercomMuted = true;
		public static void IntercomUnmute(this ReferenceHub player) => player.characterClassManager.NetworkIntercomMuted = false;
		public static bool IsIntercomMuted(this ReferenceHub player) => player.characterClassManager.NetworkIntercomMuted;
		public static bool IsHost(this ReferenceHub player) => player.characterClassManager.IsHost;
		public static bool GetGodMode(this ReferenceHub player) => player.characterClassManager.GodMode;
		public static void SetGodMode(this ReferenceHub player, bool enable) => player.characterClassManager.GodMode = enable;
		public static float GetHP(this ReferenceHub player) => player.playerStats.Health;
		public static void SetHP(this ReferenceHub player, float amount) => player.playerStats.Health = amount;
		public static void AddHP(this ReferenceHub player, float amount) => player.playerStats.Health += amount;
		public static void Heal(this ReferenceHub player, float amount) => player.playerStats.Health = Mathf.Clamp(player.playerStats.Health + amount, 1, player.playerStats.maxHP);
		public static void Heal(this ReferenceHub player) => player.playerStats.Health = player.playerStats.maxHP;
		public static void Damage(this ReferenceHub player, int amount, DamageTypes.DamageType damageType)
		{
			player.playerStats.HurtPlayer(new PlayerStats.HitInfo(amount, "WORLD", damageType, player.queryProcessor.PlayerId), player.gameObject);
		}
		public static float GetMaxHP(this ReferenceHub player) => player.playerStats.maxHP;
		public static void SetMaxHP(this ReferenceHub player, float amount) => player.playerStats.maxHP = (int)amount;
		public static float GetAHP(this ReferenceHub player) => player.playerStats.unsyncedArtificialHealth;
		public static void SetAHP(this ReferenceHub player, float amount) => player.playerStats.unsyncedArtificialHealth = amount;
		public static void AddAHP(this ReferenceHub player, float amount) => player.playerStats.unsyncedArtificialHealth += amount;
		public static float GetMaxAHP(this ReferenceHub player) => player.playerStats.maxArtificialHealth;
		public static float SetMaxAHP(this ReferenceHub player, int amount) => player.playerStats.maxArtificialHealth = amount;
		public static List<Inventory.SyncItemInfo> GetAllItems(this ReferenceHub player) => player.inventory.items.ToList();
		public static Inventory.SyncItemInfo GetCurrentItem(this ReferenceHub player) => player.inventory.GetItemInHand();
		public static void SetCurrentItem(this ReferenceHub player, ItemType itemType) => player.inventory.SetCurItem(itemType);
		public static void AddItem(this ReferenceHub player, ItemType itemType, float duration = float.NegativeInfinity, int sight = 0, int barrel = 0, int other = 0) =>
			player.inventory.AddNewItem(itemType, duration, sight, barrel, other);
		public static void AddItem(this ReferenceHub player, ItemType itemType) => player.inventory.AddNewItem(itemType);
		public static void AddItem(this ReferenceHub player, Inventory.SyncItemInfo item) => player.inventory.AddNewItem(item.id, item.durability, item.modSight, item.modBarrel, item.modOther);
		public static void DropItem(this ReferenceHub player, Inventory.SyncItemInfo item)
		{
			player.inventory.SetPickup(item.id, item.durability, player.GetPosition(), player.inventory.camera.transform.rotation, item.modSight, item.modBarrel, item.modOther);
			player.inventory.items.Remove(item);
		}
		public static void RemoveItem(this ReferenceHub player, Inventory.SyncItemInfo item) => player.inventory.items.Remove(item);
		public static void SetInventory(this ReferenceHub player, List<Inventory.SyncItemInfo> items)
		{
			player.ClearInventory();

			foreach (Inventory.SyncItemInfo item in items)
				player.inventory.AddNewItem(item.id, item.durability, item.modSight, item.modBarrel, item.modOther);
		}
		public static void ClearInventory(this ReferenceHub player) => player.inventory.items.Clear();
		public static bool IsReloading(this ReferenceHub player) => player.weaponManager.IsReloading();
		public static bool IsZooming(this ReferenceHub player) => player.weaponManager.ZoomInProgress();
		public static void Ban(this ReferenceHub player, int duration, string reason, string issuer = "API") => player.gameObject.Ban(duration, reason, issuer);
		public static void Ban(this GameObject player, int duration, string reason, string issuer = "API") => PlayerManager.localPlayer.GetComponent<BanPlayer>().BanUser(player, duration, reason, issuer, false);
		public static void Kick(this ReferenceHub player, string reason, string issuer = "API") => player.Ban(0, reason, issuer);
		public static void Kick(this GameObject player, string reason, string issuer = "API") => player.Ban(0, reason, issuer);
		public static void Handcuff(this ReferenceHub player, ReferenceHub target)
		{
			Handcuffs handcuffs = target.handcuffs;
			if (handcuffs == null) { return; }
			if (handcuffs.CufferId < 0 &&
				player.inventory.items.Any((Inventory.SyncItemInfo item) => item.id == ItemType.Disarmer) &&
				Vector3.Distance(player.transform.position, target.transform.position) <= 130f)
				handcuffs.NetworkCufferId = player.GetPlayerId();
		}
		public static void Uncuff(this ReferenceHub player) => player.handcuffs.NetworkCufferId = -1;
		public static bool IsHandCuffed(this ReferenceHub player) => player.handcuffs.CufferId != -1;
		public static bool GetHandCuffer(this ReferenceHub player) => Get(player.handcuffs.CufferId);
		public static string GetIpAddress(this ReferenceHub player) => player.queryProcessor._conns().address;
		public static void SetScale(this ReferenceHub player, float scale) => player.SetScale(Vector3.one * scale);
		public static void SetScale(this ReferenceHub player, Vector3 scale) => player.SetScale(scale.x, scale.y, scale.z);
		public static void SetScale(this ReferenceHub player, float x, float y, float z)
		{
			try
			{
				player.transform.localScale = new Vector3(x, y, z);

				foreach (ReferenceHub target in GetHubs())
					SendSpawnMessage?.Invoke(null, new object[] { player.GetComponent<NetworkIdentity>(), target.GetConnection() });
			}
			catch (Exception exception)
			{
				Log.Error($"SetScale error: {exception}");
			}
		}
		public static Vector3 GetScale(this ReferenceHub player) => player.transform.localScale;
		public static NetworkConnection GetConnection(this ReferenceHub player) => player.scp079PlayerScript.connectionToClient;
		public static void Disconnect(this ReferenceHub player, string reason = null) => ServerConsole.Disconnect(player.gameObject, string.IsNullOrEmpty(reason) ? "" : reason);
		public static void Kill(this ReferenceHub player, DamageTypes.DamageType damageType = default) => player.playerStats.HurtPlayer(new PlayerStats.HitInfo(-1f, "WORLD", damageType, 0), player.gameObject);
		public static IEnumerable<ReferenceHub> GetHubs(this Team team) => GetHubs().Where(player => player.GetTeam() == team);
		public static IEnumerable<ReferenceHub> GetHubs(this RoleType role) => GetHubs().Where(player => player.GetRole() == role);
		public static string GetGroupName(this ReferenceHub player) => ServerStatic.GetPermissionsHandler()._members()[player.GetUserId()];
		public static bool GetBypassMode(this ReferenceHub player) => player.serverRoles.BypassMode;
		public static void SetBypassMode(this ReferenceHub player, bool isEnabled) => player.serverRoles.BypassMode = isEnabled;
		public static void SendConsoleMessage(this ReferenceHub player, string message, string color)
		{
			player.characterClassManager.TargetConsolePrint(player.GetConnection(), message, color);
		}
		public static void SetFriendlyFire(this ReferenceHub player, bool value) => player.weaponManager.GetShootPermission(player.characterClassManager, value);
		public static string GetBadgeName(this ReferenceHub rh) => rh.serverRoles.UserGroup().BadgeText;
		public static bool IsCuffed(this ReferenceHub rh) => rh.GetCufferId() != -1;
		public static int GetCufferId(this ReferenceHub rh) => rh.handcuffs.NetworkCufferId;
		public static void SetCufferId(this ReferenceHub rh, int id) => rh.handcuffs.NetworkCufferId = id;
		public static Stamina Stamina(this ReferenceHub rh) => rh.fpc.staminaController();

		public static bool OfflineBan(
		  string ip,
		  string id,
		  string Nick,
		  int duration,
		  string reason,
		  string AdminNick)
		{
			Log.Info("[BAN] BAN OFFLINE USER: " + Nick + " ADMIN: " + AdminNick);
			string str1 = "";
			try
			{
				str1 = id;
			}
			catch
			{
				Log.Info("Failed during issue of User ID ban (1)!");
			}
			if (duration <= 0)
				return true;
			string str2 = string.IsNullOrEmpty(Nick) ? "(no nick)" : Nick;
			long num = TimeBehaviour.CurrentTimestamp();
			long banExpieryTime = TimeBehaviour.GetBanExpirationTime((uint)duration);
			try
			{
				if (str1 != null)
				{
					Log.Info("[BAN] BAN OFFLINE NICK AND STEAMID");
					BanHandler.IssueBan(new BanDetails()
					{
						OriginalName = str2,
						Id = str1,
						IssuanceTime = num,
						Expires = banExpieryTime,
						Reason = reason,
						Issuer = AdminNick
					}, BanHandler.BanType.UserId);
				}
			}
			catch
			{
				Log.Info("Failed during issue of User ID ban (2)!");
				return false;
			}
			try
			{
				if (!string.IsNullOrEmpty(ip))
				{
					Log.Info("[BAN] BAN OFFLINE IP");
					BanHandler.IssueBan(new BanDetails()
					{
						OriginalName = str2,
						Id = ip,
						IssuanceTime = num,
						Expires = banExpieryTime,
						Reason = reason,
						Issuer = AdminNick
					}, BanHandler.BanType.IP);
				}
			}
			catch
			{
				Log.Info("Failed during issue of IP ban!");
				return false;
			}
			return true;
		}
		public static void InvokeStaticMethod(this Type type, string methodName, object[] param)
		{
			BindingFlags flags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic |
								 BindingFlags.Static | BindingFlags.Public;
			MethodInfo info = type.GetMethod(methodName, flags);
			info?.Invoke(null, param);
		}
		public static void ChangeModel(this ReferenceHub player, RoleType newModel)
		{
			GameObject gameObject = player.gameObject;
			CharacterClassManager ccm = gameObject.GetComponent<CharacterClassManager>();
			NetworkIdentity identity = gameObject.GetComponent<NetworkIdentity>();
			RoleType FirstRole = player.GetRole();
			ccm.CurClass = newModel;
			ObjectDestroyMessage destroyMessage = new ObjectDestroyMessage
			{
				netId = identity.netId
			};
			foreach (ReferenceHub ply in GetHubs())
			{
				if (ply.GetPlayerId() == player.GetPlayerId())
					continue;

				GameObject gameObject2 = ply.gameObject;
				NetworkConnection playerCon = gameObject2.GetComponent<NetworkIdentity>().connectionToClient;
				playerCon.Send(destroyMessage, 0);
				object[] parameters = new object[] { identity, playerCon };
				typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", parameters);
			}
			ccm.CurClass = FirstRole;
		}
		public static void SizeCamera(this ReferenceHub player, Vector3 size)
		{
			GameObject target = player.gameObject;
			NetworkIdentity component = target.GetComponent<NetworkIdentity>();
			target.transform.localScale = size;

			ObjectDestroyMessage objectDestroyMessage = default;
			objectDestroyMessage.netId = component.netId;
			foreach (GameObject ply in PlayerManager.players)
			{
				NetworkConnection connectionToClient = ply.GetComponent<NetworkIdentity>().connectionToClient;
				if (ply != target)
					connectionToClient.Send(objectDestroyMessage, 0);
				object[] param = new object[] { component, connectionToClient };
				typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", param);
			}
		}
		public static void ShowHint(string text, float duration = 1f)
		{
			ReferenceHub.LocalHub.hints.Show(new TextHint(text, new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), duration));
		}
		public static void BodyDelete(this ReferenceHub player)
		{
			foreach (Ragdoll doll in UnityEngine.Object.FindObjectsOfType<Ragdoll>())
				if (doll.owner.PlayerId == player.queryProcessor.PlayerId)
					NetworkServer.Destroy(doll.gameObject);
		}
		public static List<string> GetGameObjectsInRange(this ReferenceHub player, float range)
		{
			List<string> gameObjects = new List<string>();
			foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType<GameObject>()) { if (Vector3.Distance(obj.transform.position, player.GetPosition()) <= range && !obj.name.Contains("mixamorig") && !obj.name.Contains("Pos")) { gameObjects.Add(obj.name.Trim() + "\n"); } }
			return gameObjects;
		}
		public static void Reconnect(this ReferenceHub player)
		{
			GameObject localPlayer = PlayerManager.localPlayer;
			NetworkIdentity component = localPlayer.GetComponent<NetworkIdentity>();
			ObjectDestroyMessage msg = default(ObjectDestroyMessage);
			msg.netId = component.netId;
			NetworkConnection connectionToClient = player.gameObject.GetComponent<NetworkIdentity>().connectionToClient;
			if (!(player.gameObject == localPlayer))
			{
				connectionToClient.Send(msg, 0);
				object[] param = new object[]
				{
					component,
					connectionToClient
				};
				typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", param);
			}
		}
		public static void ShowHitmark(this ReferenceHub player)
		{
			foreach (ReferenceHub hub in ReferenceHub.GetAllHubs().Values)
			{
				foreach (ReferenceHub player2 in GetHubs())
				{
					if (player.playerId == player2.playerId)
						continue;
				}
				hub.weaponManager.CallRpcConfirmShot(true, 13);
			}
		}
		public static void Blink(this ReferenceHub player) => player.GetComponent<Scp173PlayerScript>().CallRpcBlinkTime();
		public static void PlayNeckSnapSound(this ReferenceHub player) => player.GetComponent<Scp173PlayerScript>().CallRpcSyncAudio();
		public static void PlayFallSound(this ReferenceHub player) => player.falldamage.CallRpcDoSound();
		public static void Redirect(this ReferenceHub player, float timeOffset, ushort port) => player.playerStats.CallRpcRoundrestartRedirect(timeOffset, port);
		public static void Teleport(this ReferenceHub player, Vector3 position, float rotation = 0f, bool unstuck = false) => player.playerMovementSync.OverridePosition(position, rotation, unstuck);
		public static Vector3 Get106Portal(this ReferenceHub player)
		{
			if (!player.GetComponent<Scp106PlayerScript>().iAm106)
			{
				return Vector3.zero;
			}
			return player.GetComponent<Scp106PlayerScript>().NetworkportalPosition;
		}
		public static void PlayReloadAnimation(this ReferenceHub player, sbyte weapon = 0) => player.weaponManager.CallRpcReload(weapon);
		public static void Play106TeleportAnimation(this ReferenceHub player) => player.scp106PlayerScript.CallRpcTeleportAnimation();
		public static void Play106ContainAnimation(this ReferenceHub player) => player.scp106PlayerScript.CallRpcContainAnimation();
		public static void Create106Portal(this ReferenceHub player) => player.scp106PlayerScript.CallCmdMakePortal();
		public static void Use106Portal(this ReferenceHub player) => player.scp106PlayerScript.CallCmdUsePortal();
		public static void PersonalClearBroadcasts(this ReferenceHub player)
        	{
			if(player.GetConnection() != null)
            		{
				GameObject.Find("Host").GetComponent<Broadcast>().TargetClearElements(player.GetConnection());
            		}
        	}
	}
}
