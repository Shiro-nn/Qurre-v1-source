using Qurre.API.Objects;
using Mirror;
using System;
using System.Collections.Generic;
using MEC;
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
		public static string UserId(this ReferenceHub player) => player.characterClassManager.UserId;
		public static void SetUserId(this ReferenceHub player, string newId) => player.characterClassManager.NetworkSyncedUserId = newId;
		public static int PlayerId(this ReferenceHub player) => player.queryProcessor.PlayerId;
		public static void SetPlayerId(this ReferenceHub player, int newId) => player.queryProcessor.NetworkPlayerId = newId;
		public static bool Overwatch(this ReferenceHub player) => player.serverRoles.OverwatchEnabled();
		public static void SetOverwatch(this ReferenceHub player, bool enable) => player.serverRoles.CallTargetSetOverwatch(player.networkIdentity.connectionToClient, enable);
		public static bool IsScp(this ReferenceHub hub) => hub.characterClassManager.IsAnyScp();
        public static bool IsNTF(this ReferenceHub hub)
        {
            switch (hub.Role())
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
		public static bool IsHuman(this ReferenceHub hub) => hub.characterClassManager.IsHuman();
        public static bool IsAlive(this ReferenceHub hub) => hub.characterClassManager.IsAlive;
		public static RoleType Role(this ReferenceHub player) => player.characterClassManager.CurClass;
		public static void ChangeRole(this ReferenceHub player, RoleType role) => player.characterClassManager.SetPlayersClass(role, player.gameObject);
		public static void ChangeRole(this ReferenceHub player, RoleType role, bool keepPosition, bool keepHP)
		{
			if (keepPosition && !keepHP)
			{
				player.characterClassManager.NetworkCurClass = role;
				player.playerStats.SetHPAmount(player.characterClassManager.Classes.SafeGet(player.Role()).maxHP);
			}
			else if (keepPosition && keepHP)
			{
				player.characterClassManager.NetworkCurClass = role;
			}
			else
				ChangeRole(player, role);
		}
		public static Vector3 Position(this ReferenceHub player) => player.playerMovementSync.transform.position;
		public static Vector2 Rotations(this ReferenceHub player) => player.playerMovementSync.NetworkRotationSync;
		public static Vector3 RotationVector(this ReferenceHub player) => player.characterClassManager.transform.forward;
		public static void SetPosition(this ReferenceHub player, float x, float y, float z) => player.playerMovementSync.OverridePosition(new Vector3(x, y, z), player.transform.rotation.eulerAngles.y);
		public static void SetPosition(this ReferenceHub player, Vector3 position) => player.SetPosition(position.x, position.y, position.z);
		public static void SetRotation(this ReferenceHub player, Vector2 rotations) => player.SetRotation(rotations.x, rotations.y);
		public static void SetRotation(this ReferenceHub player, float x, float y) => player.playerMovementSync.NetworkRotationSync = new Vector2(x, y);
		public static UserGroup Group(this ReferenceHub player) => player.serverRoles.UserGroup();
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

			if (ServerStatic.GetPermissionsHandler()._members().ContainsKey(player.UserId()))
			{
				ServerStatic.GetPermissionsHandler()._members()[player.UserId()] = rankName;
			}
			else
			{
				ServerStatic.GetPermissionsHandler()._members().Add(player.UserId(), rankName);
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
		public static string Name(this ReferenceHub player) => player.nicknameSync.Network_myNickSync;
		public static void SetName(this ReferenceHub player, string name)
		{
			player.nicknameSync.Network_myNickSync = name;
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
		public static void DelayedBroadcast(this ReferenceHub player, ushort time, float delay, string message, Broadcast.BroadcastFlags flag = 0) => Timing.CallDelayed(delay, () => Map.BroadcastComponent.TargetAddElement(player.scp079PlayerScript.connectionToClient, message, time, flag));
		public static void ClearBroadcasts(this ReferenceHub player) => Map.BroadcastComponent.TargetClearElements(player.scp079PlayerScript.connectionToClient);
		public static Team Team(this ReferenceHub player) => player.Role().Team();
		public static Team Team(this RoleType roleType)
		{
			switch (roleType)
			{
				case RoleType.ChaosInsurgency:
					return global::Team.CHI;
				case RoleType.Scientist:
					return global::Team.RSC;
				case RoleType.ClassD:
					return global::Team.CDP;
				case RoleType.Scp049:
				case RoleType.Scp93953:
				case RoleType.Scp93989:
				case RoleType.Scp0492:
				case RoleType.Scp079:
				case RoleType.Scp096:
				case RoleType.Scp106:
				case RoleType.Scp173:
					return global::Team.SCP;
				case RoleType.Spectator:
					return global::Team.RIP;
				case RoleType.FacilityGuard:
				case RoleType.NtfCadet:
				case RoleType.NtfLieutenant:
				case RoleType.NtfCommander:
				case RoleType.NtfScientist:
					return global::Team.MTF;
				case RoleType.Tutorial:
					return global::Team.TUT;
				default:
					return global::Team.RIP;
			}
		}
		public static Side Side(this RoleType type) => type.Team().Side();
		public static Side Side(this Team team)
		{
			switch (team)
			{
				case global::Team.SCP:
					return Objects.Side.SCP;
				case global::Team.MTF:
				case global::Team.RSC:
					return Objects.Side.MTF;
				case global::Team.CHI:
				case global::Team.CDP:
					return Objects.Side.CHAOS;
				case global::Team.TUT:
					return Objects.Side.TUTORIAL;
				case global::Team.RIP:
				default: return Objects.Side.NONE;
			}
		}
		public static Side Side(this ReferenceHub hub) => hub.Team().Side();
		public static ReferenceHub Get(this GameObject player) => ReferenceHub.GetHub(player);
		public static ReferenceHub Get(int playerId)
		{
			if (IdHubs.ContainsKey(playerId))
				return IdHubs[playerId];

			foreach (ReferenceHub hub in GetHubs())
			{
				if (hub.PlayerId() == playerId)
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
						if (player.UserId() == args)
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
						if (!player.Name().ToLower().Contains(args.ToLower()))
							continue;

						if (str1.Length < maxNameLength)
						{
							int nameDifference;
							int x = maxNameLength - str1.Length;
							int y = maxNameLength - player.Name().Length;
							string str2 = player.Name();
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
		public static Room CurrentRoom(this ReferenceHub player)
		{
			Vector3 playerPos = player.Position();
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
		public static bool GodMode(this ReferenceHub player) => player.characterClassManager.GodMode;
		public static void SetGodMode(this ReferenceHub player, bool enable) => player.characterClassManager.GodMode = enable;
		public static float HP(this ReferenceHub player) => player.playerStats.Health;
		public static void SetHP(this ReferenceHub player, float amount) => player.playerStats.Health = amount;
		public static void AddHP(this ReferenceHub player, float amount) => player.playerStats.Health += amount;
		public static void Heal(this ReferenceHub player, float amount) => player.playerStats.Health = Mathf.Clamp(player.playerStats.Health + amount, 1, player.playerStats.maxHP);
		public static void Heal(this ReferenceHub player) => player.playerStats.Health = player.playerStats.maxHP;
		public static void Damage(this ReferenceHub player, int amount, DamageTypes.DamageType damageType)
		{
			player.playerStats.HurtPlayer(new PlayerStats.HitInfo(amount, "WORLD", damageType, player.queryProcessor.PlayerId), player.gameObject);
		}
		public static float MaxHP(this ReferenceHub player) => player.playerStats.maxHP;
		public static void SetMaxHP(this ReferenceHub player, float amount) => player.playerStats.maxHP = (int)amount;
		public static float AHP(this ReferenceHub player) => player.playerStats.unsyncedArtificialHealth;
		public static void SetAHP(this ReferenceHub player, float amount) => player.playerStats.unsyncedArtificialHealth = amount;
		public static void AddAHP(this ReferenceHub player, float amount) => player.playerStats.unsyncedArtificialHealth += amount;
		public static float MaxAHP(this ReferenceHub player) => player.playerStats.maxArtificialHealth;
		public static float SetMaxAHP(this ReferenceHub player, int amount) => player.playerStats.maxArtificialHealth = amount;
		public static List<Inventory.SyncItemInfo> Items(this ReferenceHub player) => player.inventory.items.ToList();
		public static Inventory.SyncItemInfo CurrentItem(this ReferenceHub player) => player.inventory.GetItemInHand();
		public static void SetCurrentItem(this ReferenceHub player, ItemType itemType) => player.inventory.SetCurItem(itemType);
		public static void AddItem(this ReferenceHub player, ItemType itemType, float duration = float.NegativeInfinity, int sight = 0, int barrel = 0, int other = 0) =>
			player.inventory.AddNewItem(itemType, duration, sight, barrel, other);
		public static void AddItem(this ReferenceHub player, ItemType itemType) => player.inventory.AddNewItem(itemType);
		public static void AddItem(this ReferenceHub player, Inventory.SyncItemInfo item) => player.inventory.AddNewItem(item.id, item.durability, item.modSight, item.modBarrel, item.modOther);
		public static void DropItem(this ReferenceHub player, Inventory.SyncItemInfo item)
		{
			player.inventory.SetPickup(item.id, item.durability, player.Position(), player.inventory.camera.transform.rotation, item.modSight, item.modBarrel, item.modOther);
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
				handcuffs.NetworkCufferId = player.PlayerId();
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
					SendSpawnMessage?.Invoke(null, new object[] { player.GetComponent<NetworkIdentity>(), target.Connection() });
			}
			catch (Exception exception)
			{
				Log.Error($"SetScale error: {exception}");
			}
		}
		public static Vector3 Scale(this ReferenceHub player) => player.transform.localScale;
		public static NetworkConnection Connection(this ReferenceHub player) => player.scp079PlayerScript.connectionToClient;
		public static void Disconnect(this ReferenceHub player, string reason = null) => ServerConsole.Disconnect(player.gameObject, string.IsNullOrEmpty(reason) ? "" : reason);
		public static void Kill(this ReferenceHub player, DamageTypes.DamageType damageType = default) => player.playerStats.HurtPlayer(new PlayerStats.HitInfo(-1f, "WORLD", damageType, 0), player.gameObject);
		public static IEnumerable<ReferenceHub> GetHubs(this Team team) => GetHubs().Where(player => player.Team() == team);
		public static IEnumerable<ReferenceHub> GetHubs(this RoleType role) => GetHubs().Where(player => player.Role() == role);
		public static string GroupName(this ReferenceHub player) => ServerStatic.GetPermissionsHandler()._members()[player.UserId()];
		public static bool BypassMode(this ReferenceHub player) => player.serverRoles.BypassMode;
		public static void SetBypassMode(this ReferenceHub player, bool status) => player.serverRoles.BypassMode = status;
		public static void ConsoleMessage(this ReferenceHub player, string message, string color)
		{
			player.characterClassManager.TargetConsolePrint(player.Connection(), message, color);
		}
		public static void SetFriendlyFire(this ReferenceHub player, bool value) => player.weaponManager.GetShootPermission(player.characterClassManager, value);
		public static string BadgeName(this ReferenceHub rh) => rh.serverRoles.UserGroup().BadgeText;
		public static bool IsCuffed(this ReferenceHub rh) => rh.CufferId() != -1;
		public static int CufferId(this ReferenceHub rh) => rh.handcuffs.NetworkCufferId;
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
			RoleType FirstRole = player.Role();
			ccm.CurClass = newModel;
			ObjectDestroyMessage destroyMessage = new ObjectDestroyMessage
			{
				netId = identity.netId
			};
			foreach (ReferenceHub ply in GetHubs())
			{
				if (ply.PlayerId() == player.PlayerId())
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
			foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType<GameObject>()) { if (Vector3.Distance(obj.transform.position, player.Position()) <= range && !obj.name.Contains("mixamorig") && !obj.name.Contains("Pos")) { gameObjects.Add(obj.name.Trim() + "\n"); } }
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
            if (player.Connection() != null)
            {
                GameObject.Find("Host").GetComponent<Broadcast>().TargetClearElements(player.Connection());
            }
        }
		public static void DelayedPersonalClearBroadcasts(this ReferenceHub player, float delay)
		{
			if (player.Connection() != null)
			{
				Timing.CallDelayed(delay, () => GameObject.Find("Host").GetComponent<Broadcast>().TargetClearElements(player.Connection()));
			}
		}
	}
}