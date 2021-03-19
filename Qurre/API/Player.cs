using Qurre.API.Objects;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static QurreModLoader.umm;
using Hints;
using Grenades;
using CustomPlayerEffects;
using RemoteAdmin;
using Qurre.API.Controllers;
namespace Qurre.API
{
	public class Player
	{
		private ReferenceHub rh;
		private GameObject go;
		private string ui;
		public Player(ReferenceHub RH)
		{
			rh = RH;
			Scp079Controller = new Scp079(this);
			Scp096Controller = new Scp096(this);
			Scp106Controller = new Scp106(this);
			Scp173Controller = new Scp173(this);
			Broadcasts = new ListBroadcasts(this);
		}
		public Player(GameObject gameObject) => rh = ReferenceHub.GetHub(gameObject);
		public static Dictionary<GameObject, Player> Dictionary { get; } = new Dictionary<GameObject, Player>();
		public static Dictionary<int, Player> IdPlayers = new Dictionary<int, Player>();
		public static Dictionary<string, Player> UserIDPlayers = new Dictionary<string, Player>();
		public static IEnumerable<Player> List => Dictionary.Values;
		public ReferenceHub ReferenceHub
		{
			get => rh;
			private set
			{
				if (value == null) throw new NullReferenceException("Player's ReferenceHub cannot be null!");
				rh = value;
				go = value.gameObject;
				ui = value.characterClassManager.UserId;
			}
		}
		public readonly Scp079 Scp079Controller;
		public readonly Scp096 Scp096Controller;
		public readonly Scp106 Scp106Controller;
		public readonly Scp173 Scp173Controller;
		public ListBroadcasts Broadcasts { get; }
		public GrenadeManager GrenadeManager => rh.GetComponent<GrenadeManager>();
		public GameConsoleTransmission GameConsoleTransmission => rh.GetComponent<GameConsoleTransmission>();
		public GameObject GameObject
		{
			get
			{
				if (rh == null || rh.gameObject == null) return go;
				else return rh.gameObject;
			}
		}
		public AmmoBox Ammo => rh.ammoBox;
		public HintDisplay HintDisplay => rh.hints;
		public Transform CameraTransform => rh.PlayerCameraReference;
		public Inventory Inventory => rh.inventory;
		public NetworkIdentity NetworkIdentity => rh.networkIdentity;
		public Handcuffs Handcuffs => rh.handcuffs;
		public ServerRoles ServerRoles => rh.serverRoles;
		public CharacterClassManager ClassManager => rh.characterClassManager;
		public WeaponManager WeaponManager => rh.weaponManager;
		public AnimationController AnimationController => rh.animationController;
		public PlayerStats PlayerStats => rh.playerStats;
		public Scp079PlayerScript Scp079PlayerScript => rh.scp079PlayerScript;
		public QueryProcessor QueryProcessor => rh.queryProcessor;
		public PlayerEffectsController PlayerEffectsController => rh.playerEffectsController;
		public NicknameSync NicknameSync => rh.nicknameSync;
		public int Id
		{
			get => rh.queryProcessor.NetworkPlayerId;
			set => rh.queryProcessor.NetworkPlayerId = value;
		}
		public string UserId
		{
			get
			{
				string _ = ClassManager.UserId;
				if (_.Contains("@"))
				{
					ui = _;
					return _;
				}
				else return ui;
			}
			set => ClassManager.NetworkSyncedUserId = value;
		}
		public string CustomUserId
		{
			get => ClassManager.UserId2;
			set => ClassManager.UserId2 = value;
		}
		public string DisplayNickname
		{
			get => rh.nicknameSync.Network_displayName;
			set => rh.nicknameSync.Network_displayName = value;
		}
		public string Nickname => rh.nicknameSync.Network_myNickSync;
		public bool DoNotTrack => ServerRoles.DoNotTrack;
		public bool RemoteAdminAccess => ServerRoles.RemoteAdmin;
		public bool Overwatch
		{
			get => ServerRoles.OverwatchEnabled();
			set => ServerRoles.CallTargetSetOverwatch(NetworkIdentity.connectionToClient, value);
		}
		public int CufferId
		{
			get => Handcuffs.NetworkCufferId;
			set => Handcuffs.NetworkCufferId = value;
		}
		public bool IsCuffed => CufferId != -1;
		public Vector3 Position
		{
			get => rh.playerMovementSync.GetRealPosition();
			set => rh.playerMovementSync.OverridePosition(value, 0f);
		}
		public Vector2 Rotations
		{
			get => rh.playerMovementSync.NetworkRotationSync;
			set => rh.playerMovementSync.NetworkRotationSync = value;
		}
		public Vector3 Rotation
		{
			get => rh.PlayerCameraReference.forward;
			set => rh.PlayerCameraReference.forward = value;
		}
		public Vector3 Scale
		{
			get => rh.transform.localScale;
			set
			{
				try
				{
					rh.transform.localScale = value;
					foreach (Player target in List) SendSpawnMessage?.Invoke(null, new object[] { ReferenceHub.characterClassManager.netIdentity, target.Connection });
				}
				catch (Exception ex)
				{
					Log.Error($"Scale error: {ex}");
				}
			}
		}
		public GameObject LookingAt
		{
			get
			{
				if (!Physics.Raycast(rh.PlayerCameraReference.transform.position, rh.PlayerCameraReference.transform.forward, out RaycastHit raycastthit, 100f))
					return null;
				return raycastthit.transform.gameObject;
			}
		}
		public Team Team => GetTeam(Role);
		public Side Side => GetSide(Team);
		public RoleType Role
		{
			get => ClassManager.NetworkCurClass;
			set => SetRole(value);
		}
		public bool IsReloading => WeaponManager.IsReloading();
		public bool IsZooming => WeaponManager.NetworksyncZoomed;
		public PlayerMovementState MoveState => AnimationController.MoveState;
		public bool IsJumping => AnimationController.curAnim == 2;
		public string IP => NetworkIdentity.connectionToClient.address;
		public NetworkConnection Connection => Scp079PlayerScript.connectionToClient;
		public bool IsHost => ClassManager.IsHost;
		public bool FriendlyFire { get; set; }
		public bool BypassMode
		{
			get => ServerRoles.BypassMode;
			set => ServerRoles.BypassMode = value;
		}
		public bool Muted
		{
			get => ClassManager.NetworkMuted;
			set => ClassManager.NetworkMuted = value;
		}
		public bool IntercomMuted
		{
			get => ClassManager.NetworkIntercomMuted;
			set => ClassManager.NetworkIntercomMuted = value;
		}
		public bool GodMode
		{
			get => ClassManager.GodMode;
			set => ClassManager.GodMode = value;
		}
		public float HP
		{
			get => PlayerStats.Health;
			set
			{
				if (value > MaxHP) MaxHP = (int)value;
			}
		}
		public int MaxHP
		{
			get => PlayerStats.maxHP;
			set => PlayerStats.maxHP = value;
		}
		public float AHP
		{
			get => PlayerStats.unsyncedArtificialHealth;
			set
			{
				PlayerStats.unsyncedArtificialHealth = value;
				if (value > MaxAHP) MaxAHP = (int)value;
			}
		}
		public int MaxAHP
		{
			get => PlayerStats.maxArtificialHealth;
			set => PlayerStats.maxArtificialHealth = value;
		}
		public Inventory.SyncItemInfo CurrentItem
		{
			get => Inventory.GetItemInHand();
			set => Inventory.SetCurItem(value.id);
		}
		public List<Inventory.SyncItemInfo> AllItems => Inventory.items.ToList();
		public int CurrentItemIndex => Inventory.GetItemIndex();
		public Stamina Stamina => rh.fpc.staminaController();
		public float StaminaUsage
		{
			get => rh.fpc.staminaController().StaminaUse * 100;
			set => rh.fpc.staminaController().StaminaUse = (value / 100f);
		}
		public string GroupName
		{
			get => ServerStatic.GetPermissionsHandler()._members().TryGetValue(UserId, out string groupName) ? groupName : null;
			set => ServerStatic.GetPermissionsHandler()._members()[UserId] = value;
		}
		public Room Room
		{
			get
			{
				if (Vector3.Distance(Vector3.up * -1997, Position) <= 50) return Extensions.GetRoom(RoomType.Pocket);
				return Map.Rooms.FirstOrDefault(x => x.GameObject == ReferenceHub.localCurrentRoomEffects.CurRoom());
			}
			set => Position = value.Position;
		}
		public CommandSender Sender
		{
			get
			{
				if (IsHost) return _scs;
				return QueryProcessor._sender();
			}
		}
		public bool GlobalRemoteAdmin => ServerRoles.RemoteAdminMode == ServerRoles.AccessMode.GlobalAccess;
		public UserGroup Group
		{
			get => ServerRoles.UserGroup();
			set => ServerRoles.SetGroup(value, false, false, value.Cover);
		}
		public string RoleColor
		{
			get => ServerRoles.NetworkMyColor;
			set => ServerRoles.SetColor(value);
		}
		public string RoleName
		{
			get => ServerRoles.NetworkMyText;
			set => ServerRoles.SetText(value);
		}
		public string UnitName
		{
			get => ClassManager.NetworkCurUnitName;
			set => ClassManager.NetworkCurUnitName = value;
		}
		public float AliveTime => ClassManager.AliveTime;
		public long DeathTime
		{
			get => ClassManager.DeathTime;
			set => ClassManager.DeathTime = value;
		}

		public int Ping => Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorServer.Peers[Connection.connectionId].Ping;
		public uint Ammo5
		{
			get { try { return Ammo[0]; } catch { return 0; } }
			set { try { Ammo[0] = value; } catch { } }
		}

		public uint Ammo7
		{
			get { try { return Ammo[1]; } catch { return 0; } }
			set { try { Ammo[1] = value; } catch { } }
		}

		public uint Ammo9
		{
			get { try { return Ammo[2]; } catch { return 0; } }
			set { try { Ammo[2] = value; } catch { } }
		}
		public static IEnumerable<Player> Get(Team team) => List.Where(player => player.Team == team);
		public static IEnumerable<Player> Get(RoleType role) => List.Where(player => player.Role == role);
		public static Player Get(CommandSender sender) => Get(sender.SenderId);
		public static Player Get(ReferenceHub referenceHub) => referenceHub == null ? null : Get(referenceHub.gameObject);
		public static Player Get(GameObject gameObject)
		{
			if (gameObject == null) return null;
			Dictionary.TryGetValue(gameObject, out Player player);
			return player;
		}
		public static Player Get(int playerId)
		{
			if (IdPlayers.ContainsKey(playerId)) return IdPlayers[playerId];
			foreach (Player pl in List.Where(x => x.Id == playerId))
			{
				IdPlayers.Add(playerId, pl);
				return pl;
			}
			return null;
		}
		public static Player Get(string args)
		{
			try
			{
				if (UserIDPlayers.ContainsKey(args)) return UserIDPlayers[args];
				Player playerFound = null;
				if (short.TryParse(args, out short playerId)) return Get(playerId);
				if (args.EndsWith("@steam") || args.EndsWith("@discord") || args.EndsWith("@northwood") || args.EndsWith("@patreon"))
					foreach (Player pl in List.Where(x => x.UserId == args)) playerFound = pl;
				else
				{
					if (args == "WORLD" || args == "SCP-018" || args == "SCP-575" || args == "SCP-207") return null;
					int maxNameLength = 31, lastnameDifference = 31;
					string str1 = args.ToLower();
					foreach (Player pl in List)
					{
						if (!pl.Nickname.ToLower().Contains(args.ToLower())) continue;
						if (str1.Length < maxNameLength)
						{
							int nameDifference;
							int x = maxNameLength - str1.Length;
							int y = maxNameLength - pl.Nickname.Length;
							string str2 = pl.Nickname;
							for (int i = 0; i < x; i++) str1 += "z";
							for (int i = 0; i < y; i++) str2 += "z";
							int n = str1.Length;
							int m = str2.Length;
							int[,] d = new int[n + 1, m + 1];
							if (n == 0) nameDifference = m;
							if (m == 0) nameDifference = n;
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
								playerFound = pl;
							}
						}
					}
				}
				if (playerFound != null) UserIDPlayers.Add(args, playerFound);
				return playerFound;
			}
			catch (Exception exception)
			{
				Log.Error($"GetPlayer error: {exception}");
				return null;
			}
		}
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
		public void RAMessage(string message, bool success = true, string pluginName = null) =>
			Sender.RaReply((pluginName ?? Assembly.GetCallingAssembly().GetName().Name) + "#" + message, success, true, string.Empty);
		public void SendConsoleMessage(string message, string color)
		{
			try { ClassManager.TargetConsolePrint(Connection, message, color); }
			catch { rh.GetComponent<GameConsoleTransmission>().SendToClient(Connection, message, color); }
		}
		public void RaLogin()
		{
			ServerRoles.RemoteAdmin = true;
			ServerRoles.Permissions = ServerRoles._globalPerms();
			ServerRoles.RemoteAdminMode = GlobalRemoteAdmin ? ServerRoles.AccessMode.GlobalAccess : ServerRoles.AccessMode.PasswordOverride;
			ServerRoles.TargetOpenRemoteAdmin(Connection, false);
		}
		public void RaLogout()
		{
			ServerRoles.RemoteAdmin = false;
			ServerRoles.RemoteAdminMode = ServerRoles.AccessMode.LocalAccess;
			ServerRoles.TargetCloseRemoteAdmin(Connection);
		}
		public void ExecuteCommand(string command, bool RA = true) => GameCore.Console.singleton.TypeCommand(RA ? "/" : "" + command, Sender);
		public void OpenReportWindow(string text) => GameConsoleTransmission.SendToClient(Connection, "[REPORTING] " + text, "white");
		public void RemoveDisplayInfo(PlayerInfoArea playerInfo) => NicknameSync.Network_playerInfoToShow &= ~playerInfo;
		public void AddDisplayInfo(PlayerInfoArea playerInfo) => NicknameSync.Network_playerInfoToShow |= playerInfo;
		public void DimScreen()
		{
			var component = RoundSummary.singleton;
			var writer = NetworkWriterPool.GetWriter();
			var msg = new RpcMessage
			{
				netId = component.netId,
				componentIndex = component.ComponentIndex,
				functionHash = typeof(RoundSummary).FullName.GetStableHashCode() * 503 + "RpcDimScreen".GetStableHashCode(),
				payload = writer.ToArraySegment()
			};
			Connection.Send(msg);
			NetworkWriterPool.Recycle(writer);
		}
		public void ShakeScreen(bool achieve = false)
		{
			var component = AlphaWarheadController.Host;
			var writer = NetworkWriterPool.GetWriter();
			writer.WriteBoolean(achieve);
			var msg = new RpcMessage
			{
				netId = component.netId,
				componentIndex = component.ComponentIndex,
				functionHash = typeof(AlphaWarheadController).FullName.GetStableHashCode() * 503 + "RpcShake".GetStableHashCode(),
				payload = writer.ToArraySegment()
			};
			Connection.Send(msg);
			NetworkWriterPool.Recycle(writer);
		}
		public void PlaceBlood(Vector3 pos, int type = 1, float size = 2f)
		{
			var component = ClassManager;
			var writer = NetworkWriterPool.GetWriter();
			writer.WriteVector3(pos);
			writer.WritePackedInt32(type);
			writer.WriteSingle(size);
			var msg = new RpcMessage
			{
				netId = component.netId,
				componentIndex = component.ComponentIndex,
				functionHash = typeof(CharacterClassManager).FullName.GetStableHashCode() * 503 + "RpcPlaceBlood".GetStableHashCode(),
				payload = writer.ToArraySegment()
			};
			Connection.Send(msg);
			NetworkWriterPool.Recycle(writer);
		}
		public void SetRole(RoleType newRole, bool lite = false, bool escape = false) => ClassManager.SetClassIDAdv(newRole, lite, escape);
		public Controllers.Broadcast Broadcast(ushort time, string message, bool instant = false)
		{
			var bc = new Controllers.Broadcast(this, message, time);
			Broadcasts.Add(bc, instant);
			return bc;
		}
		public void ClearBroadcasts() => Broadcasts.Clear();
		public void Damage(int amount, DamageTypes.DamageType damageType) => PlayerStats.HurtPlayer(new PlayerStats.HitInfo(amount, "WORLD", damageType, QueryProcessor.PlayerId), GameObject);
		public void AddItem(ItemType itemType, float duration = float.NegativeInfinity, int sight = 0, int barrel = 0, int other = 0) =>
			Inventory.AddNewItem(itemType, duration, sight, barrel, other);
		public void AddItem(ItemType itemType) => Inventory.AddNewItem(itemType);
		public void AddItem(Inventory.SyncItemInfo item) => Inventory.AddNewItem(item.id, item.durability, item.modSight, item.modBarrel, item.modOther);
		public void DropItem(Inventory.SyncItemInfo item)
		{
			Inventory.SetPickup(item.id, item.durability, Position, Inventory.camera.transform.rotation, item.modSight, item.modBarrel, item.modOther);
			Inventory.items.Remove(item);
		}
		public void DropItems() => Inventory.ServerDropAll();
		public bool HasItem(ItemType targetItem)
		{
			foreach (Inventory.SyncItemInfo item in Inventory.items.Where(x => x.id == targetItem)) return true;
			return false;
		}
		public void RemoveItem(Inventory.SyncItemInfo item) => Inventory.items.Remove(item);
		public void RemoveItem() => Inventory.items.Remove(ReferenceHub.inventory.GetItemInHand());
		public void SetInventory(List<Inventory.SyncItemInfo> items)
		{
			ClearInventory();
			foreach (Inventory.SyncItemInfo item in items) Inventory.AddNewItem(item.id, item.durability, item.modSight, item.modBarrel, item.modOther);
		}
		public void ClearInventory() => Inventory.items.Clear();
		public void Ban(int duration, string reason, string issuer = "API") => PlayerManager.localPlayer.GetComponent<BanPlayer>().BanUser(GameObject, duration, reason, issuer, false);
		public void Kick(string reason, string issuer = "API") => Ban(0, reason, issuer);
		public void Handcuff(Player player)
		{
			if (Handcuffs == null) { return; }
			if (Handcuffs.CufferId < 0 &&
				player.Inventory.items.Any((Inventory.SyncItemInfo item) => item.id == ItemType.Disarmer) &&
				Vector3.Distance(player.Position, Position) <= 130f)
				Handcuffs.NetworkCufferId = player.Id;
		}
		public void Uncuff() => Handcuffs.NetworkCufferId = -1;
		public void Disconnect(string reason = null) => ServerConsole.Disconnect(GameObject, string.IsNullOrEmpty(reason) ? "" : reason);
		public void Kill(DamageTypes.DamageType damageType = default) => PlayerStats.HurtPlayer(new PlayerStats.HitInfo(-1f, "WORLD", damageType, 0), GameObject);
		public void ChangeModel(RoleType newModel)
		{
			GameObject gameObject = GameObject;
			CharacterClassManager ccm = gameObject.GetComponent<CharacterClassManager>();
			NetworkIdentity identity = gameObject.GetComponent<NetworkIdentity>();
			RoleType FirstRole = Role;
			ccm.CurClass = newModel;
			ObjectDestroyMessage destroyMessage = new ObjectDestroyMessage { netId = identity.netId };
			foreach (Player pl in List)
			{
				if (pl.Id == Id) continue;
				GameObject gameObject2 = pl.GameObject;
				NetworkConnection playerCon = gameObject2.GetComponent<NetworkIdentity>().connectionToClient;
				playerCon.Send(destroyMessage, 0);
				object[] parameters = new object[] { identity, playerCon };
				typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", parameters);
			}
			ccm.CurClass = FirstRole;
		}
		public void SizeCamera(Vector3 size)
		{
			GameObject target = GameObject;
			NetworkIdentity component = target.GetComponent<NetworkIdentity>();
			target.transform.localScale = size;
			ObjectDestroyMessage objectDestroyMessage = default;
			objectDestroyMessage.netId = component.netId;
			foreach (GameObject ply in PlayerManager.players)
			{
				NetworkConnection connectionToClient = ply.GetComponent<NetworkIdentity>().connectionToClient;
				if (ply != target) connectionToClient.Send(objectDestroyMessage, 0);
				object[] param = new object[] { component, connectionToClient };
				typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", param);
			}
		}
		public bool GetEffectActive<T>() where T : PlayerEffect
		{
			if (PlayerEffectsController.AllEffects.TryGetValue(typeof(T), out PlayerEffect playerEffect)) return playerEffect.Enabled;
			return false;
		}
		public void DisableAllEffects()
		{
			foreach (KeyValuePair<Type, PlayerEffect> effect in PlayerEffectsController.AllEffects) if (effect.Value.Enabled) effect.Value.ServerDisable();
		}
		public void DisableEffect<T>() where T : PlayerEffect => PlayerEffectsController.DisableEffect<T>();
		public void DisableEffect(EffectType effect)
		{
			if (TryGetEffect(effect, out var pEffect)) pEffect.ServerDisable();
		}
		public void EnableEffect<T>(float duration = 0f, bool addDurationIfActive = false) where T : PlayerEffect => PlayerEffectsController.EnableEffect<T>(duration, addDurationIfActive);
		public bool EnableEffect(string effect, float duration = 0f, bool addDurationIfActive = false) => PlayerEffectsController.EnableByString(effect, duration, addDurationIfActive);
		public void EnableEffect(EffectType effect, float duration = 0f, bool addDurationIfActive = false)
		{
			if (TryGetEffect(effect, out var pEffect)) PlayerEffectsController.EnableEffect(pEffect, duration, addDurationIfActive);
		}
		public PlayerEffect GetEffect(EffectType effect)
		{
			var type = effect.Type();
			PlayerEffectsController.AllEffects.TryGetValue(type, out var pEffect);
			return pEffect;
		}
		public bool TryGetEffect(EffectType effect, out PlayerEffect playerEffect)
		{
			playerEffect = GetEffect(effect);
			return playerEffect != null;
		}
		public byte GetEffectIntensity<T>() where T : PlayerEffect
		{
			if (PlayerEffectsController.AllEffects.TryGetValue(typeof(T), out PlayerEffect playerEffect)) return playerEffect.Intensity;
			throw new ArgumentException("The given type is invalid.");
		}
		public void ChangeEffectIntensity<T>(byte intensity) where T : PlayerEffect => PlayerEffectsController.ChangeEffectIntensity<T>(intensity);
		public void ChangeEffectIntensity(string effect, byte intensity, float duration = 0) => PlayerEffectsController.ChangeByString(effect, intensity, duration);
		public void ShowHint(string text, float duration = 1f) =>
			HintDisplay.Show(new TextHint(text, new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 1f, 0f), duration));
		public void BodyDelete()
		{
			foreach (Ragdoll doll in UnityEngine.Object.FindObjectsOfType<Ragdoll>().Where(x => x.owner.PlayerId == QueryProcessor.PlayerId)) NetworkServer.Destroy(doll.gameObject);
		}
		public List<string> GetGameObjectsInRange(float range)
		{
			List<string> gameObjects = new List<string>();
			foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType<GameObject>()) { if (Vector3.Distance(obj.transform.position, Position) <= range && !obj.name.Contains("mixamorig") && !obj.name.Contains("Pos")) { gameObjects.Add(obj.name.Trim() + "\n"); } }
			return gameObjects;
		}
		public void Reconnect()
		{
			GameObject localPlayer = PlayerManager.localPlayer;
			NetworkIdentity component = localPlayer.GetComponent<NetworkIdentity>();
			ObjectDestroyMessage msg = default(ObjectDestroyMessage);
			msg.netId = component.netId;
			NetworkConnection connectionToClient = GameObject.GetComponent<NetworkIdentity>().connectionToClient;
			if (GameObject != localPlayer)
			{
				connectionToClient.Send(msg, 0);
				object[] param = new object[] { component, connectionToClient };
				typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", param);
			}
		}
		public static void ShowHitmark()
		{
			foreach (Player pl in List) pl.WeaponManager.CallRpcConfirmShot(true, 13);
		}
		public void Blink() => rh.GetComponent<Scp173PlayerScript>().CallRpcBlinkTime();
		public void PlayNeckSnapSound() => rh.GetComponent<Scp173PlayerScript>().CallRpcSyncAudio();
		public void PlayFallSound() => rh.falldamage.CallRpcDoSound();
		public void Redirect(float timeOffset, ushort port) => PlayerStats.CallRpcRoundrestartRedirect(timeOffset, port);
		public Vector3 Get106Portal()
		{
			if (!rh.GetComponent<Scp106PlayerScript>().iAm106) return Vector3.zero;
			return rh.GetComponent<Scp106PlayerScript>().NetworkportalPosition;
		}
		public void PlayReloadAnimation(sbyte weapon = 0) => WeaponManager.CallRpcReload(weapon);
		public void Play106TeleportAnimation() => rh.scp106PlayerScript.CallRpcTeleportAnimation();
		public void Play106ContainAnimation() => rh.scp106PlayerScript.CallRpcContainAnimation();
		public void Create106Portal() => rh.scp106PlayerScript.CallCmdMakePortal();
		public void Use106Portal() => rh.scp106PlayerScript.CallCmdUsePortal();

		private Team GetTeam(RoleType rt)
		{
			switch (rt)
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
		private Side GetSide(Team team)
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
	}
}