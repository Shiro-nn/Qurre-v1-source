using Qurre.API.Objects;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Hints;
using CustomPlayerEffects;
using RemoteAdmin;
using Qurre.API.Controllers;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Disarming;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.Firearms.Attachments;
using Qurre.API.Controllers.Items;
namespace Qurre.API
{
	public class Player
	{
		private ReferenceHub rh;
		private GameObject go;
		private string ui;
		private string _tag = "";
		private Radio radio;
		private Escape escape;
		internal readonly List<Item> ItemsValue = new List<Item>(8);
		public Player(ReferenceHub RH)
		{
			rh = RH;
			Scp079Controller = new Scp079(this);
			Scp096Controller = new Scp096(this);
			Scp106Controller = new Scp106(this);
			Scp173Controller = new Scp173();
			Broadcasts = new ListBroadcasts(this);
			Ammo = new AmmoBoxManager(this);
		}
		public Player(GameObject gameObject) => rh = ReferenceHub.GetHub(gameObject);
		public static Dictionary<GameObject, Player> Dictionary { get; } = new Dictionary<GameObject, Player>();
		public static Dictionary<int, Player> IdPlayers = new Dictionary<int, Player>();
		public static Dictionary<string, Player> UserIDPlayers { get; set; } = new Dictionary<string, Player>();
		public static Dictionary<string, Player> ArgsPlayers { get; set; } = new Dictionary<string, Player>();
		public static IEnumerable<Player> List => Dictionary.Values.Where(x => !x.Bot);
		public ReferenceHub ReferenceHub
		{
			get => rh;
			private set
			{
				rh = value ?? throw new NullReferenceException("Player's ReferenceHub cannot be null.");
				go = value.gameObject;
				ui = value.characterClassManager.UserId;
			}
		}
		public readonly Scp079 Scp079Controller;
		public readonly Scp096 Scp096Controller;
		public readonly Scp106 Scp106Controller;
		public readonly Scp173 Scp173Controller;
		public ListBroadcasts Broadcasts { get; }
		public GameConsoleTransmission GameConsoleTransmission => rh.GetComponent<GameConsoleTransmission>();
		public GameObject GameObject
		{
			get
			{
				if (rh == null || rh.gameObject == null) return go;
				else return rh.gameObject;
			}
		}
		public Radio Radio { get { if (radio == null) { radio = GameObject.GetComponent<Radio>(); } return radio; } }
		public Escape Escape { get { if (escape == null) { escape = ClassManager.GetComponent<Escape>(); } return escape; } }
		public AmmoBoxManager Ammo { get; }
		public HintDisplay HintDisplay => rh.hints;
		public Transform CameraTransform => rh.PlayerCameraReference;
		public Transform Transform => rh.transform;
		public Inventory Inventory => rh.inventory;
		public NetworkIdentity NetworkIdentity => rh.networkIdentity;
		public ServerRoles ServerRoles => rh.serverRoles;
		public CharacterClassManager ClassManager => rh.characterClassManager;
		public AnimationController AnimationController => rh.animationController;
		public PlayerStats PlayerStats => rh.playerStats;
		public Scp079PlayerScript Scp079PlayerScript => rh.scp079PlayerScript;
		public QueryProcessor QueryProcessor => rh.queryProcessor;
		public PlayerEffectsController PlayerEffectsController => rh.playerEffectsController;
		public NicknameSync NicknameSync => rh.nicknameSync;
		public PlayerMovementSync PlayerMovementSync => rh.playerMovementSync;
		public string Tag
		{
			get => _tag;
			set => _tag = value;
		}
		public int Id
		{
			get => rh.queryProcessor.NetworkPlayerId;
			set => rh.queryProcessor.NetworkPlayerId = value;
		}
		public string UserId
		{
			get
			{
				if (Bot)
				{
					if (ui == null) ui = $"7{UnityEngine.Random.Range(0, 99999999)}{UnityEngine.Random.Range(0, 99999999)}@bot";
					return ui;
				}
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
		public string Nickname
        {
			get => rh.nicknameSync.Network_myNickSync;
			internal set => rh.nicknameSync.Network_myNickSync = value;
		}
		public bool DoNotTrack => ServerRoles.DoNotTrack;
		public bool RemoteAdminAccess => ServerRoles.RemoteAdmin;
		public bool Overwatch
		{
			get => ServerRoles.OverwatchEnabled;
			set => ServerRoles.UserCode_TargetSetOverwatch(NetworkIdentity.connectionToClient, value);
		}
		public Player Cuffer
		{
			get
			{
				foreach (DisarmedPlayers.DisarmedEntry disarmed in DisarmedPlayers.Entries)
					if (Get(disarmed.DisarmedPlayer) == this) return Get(disarmed.Disarmer);
				return null;
			}

			set
			{
				for (int i = 0; i < DisarmedPlayers.Entries.Count; i++)
				{
					if (DisarmedPlayers.Entries[i].DisarmedPlayer == Inventory.netId)
					{
						DisarmedPlayers.Entries.RemoveAt(i);
						break;
					}
				}

				if (value != null)
					Inventory.SetDisarmedStatus(value.Inventory);
			}
		}
		public bool Cuffed => DisarmedPlayers.IsDisarmed(Inventory);
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
			get => CameraTransform.rotation.eulerAngles;
			set => CameraTransform.rotation = Quaternion.Euler(value);
		}
		public Quaternion FullRotation
		{
			get => rh.PlayerCameraReference.localRotation;
			set => rh.PlayerCameraReference.localRotation = value;
		}
		public Vector3 Scale
		{
			get => rh.transform.localScale;
			set
			{
				try
				{
					rh.transform.localScale = value;
					foreach (Player target in List) SendSpawnMessage?.Invoke(null, new object[] { ClassManager.netIdentity, target.Connection });
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
		public bool Noclip
		{
			get => rh.characterClassManager.NoclipEnabled;
			set => rh.characterClassManager.SetNoclip(value);
		}
		public Team Team => GetTeam(Role);
		public Side Side => GetSide(Team);
		public Faction Faction => ClassManager.Faction;
		public RoleType Role
		{
			get => ClassManager.CurClass;
			set => SetRole(value);
		}
		public PlayerMovementState MoveState
		{
			get => (PlayerMovementState)AnimationController.Network_curMoveState;
			set => AnimationController.Network_curMoveState = (byte)value;
		}
		public bool IsJumping => AnimationController.curAnim == 2;
		public string Ip => NetworkIdentity.connectionToClient.address;
		public NetworkConnection Connection => Scp079PlayerScript.connectionToClient;
		public bool IsHost => ClassManager.IsHost;
		public bool FriendlyFire { get; set; }
		public bool Zoomed { get; internal set; } = false;
		public bool UseStamina { get; set; } = true;
		public bool Invisible { get; set; }
		public bool Bot { get; internal set; } = false;
		///<summary>
		///<para>After spawning, it becomes false again.</para>
		///</summary>
		public bool BlockSpawnTeleport { get; set; } = false;
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
		public float Hp
		{
			get => PlayerStats.Health;
			set
			{
				PlayerStats.Health = value;
				if (value > MaxHp) MaxHp = (int)value;
			}
		}
		public int MaxHp
		{
			get => PlayerStats.maxHP;
			set => PlayerStats.maxHP = value;
		}
		public float AhpDecay
		{
			get => PlayerStats.NetworkArtificialHpDecay;
			set => PlayerStats.NetworkArtificialHpDecay = value;
		}
		public float Ahp
		{
			get => PlayerStats.GetAhpValue();
			set
			{
				PlayerStats.SafeSetAhpValue(value);
				if (value > MaxAhp) MaxAhp = (int)value;
			}
		}
		public int MaxAhp
		{
			get => PlayerStats.NetworkMaxArtificialHealth;
			set => PlayerStats.NetworkMaxArtificialHealth = value;
		}
		public ItemIdentifier CurrentItem
		{
			get => Inventory.NetworkCurItem;
			set => Inventory.NetworkCurItem = value;
		}
		public ItemBase CurInstance
		{
			get => Inventory.CurInstance;
			set => Inventory.CurInstance = value;
		}
		public IReadOnlyCollection<Item> AllItems => ItemsValue.AsReadOnly();
		public ItemType ItemTypeInHand => Inventory.CurItem.TypeId;
		public Item ItemInHand
		{
			get => Item.Get(Inventory.CurInstance);

			set
			{
				if (!Inventory.UserInventory.Items.TryGetValue(value.Serial, out _))
					AddItem(value.Base);

				Inventory.ServerSelectItem(value.Serial);
			}
		}
		public Stamina Stamina => rh.fpc.staminaController;
		public float StaminaUsage
		{
			get => rh.fpc.staminaController.StaminaUse * 100;
			set => rh.fpc.staminaController.StaminaUse = (value / 100f);
		}
		public string GroupName
		{
			get => ServerStatic.GetPermissionsHandler()._members.TryGetValue(UserId, out string groupName) ? groupName : null;
			set => ServerStatic.GetPermissionsHandler()._members[UserId] = value;
		}
		public Room Room
		{
			get => Map.FindRoom(GameObject);
			set => Position = value.Position + Vector3.up * 2;
		}
		public CommandSender Sender
		{
			get
			{
				if (IsHost) return ServerConsole._scs;
				return QueryProcessor._sender;
			}
		}
		public bool GlobalRemoteAdmin => ServerRoles.RemoteAdminMode == ServerRoles.AccessMode.GlobalAccess;
		public UserGroup Group
		{
			get => ServerRoles.Group;
			set => ServerRoles.SetGroup(value, false, false, value.Cover);
		}
		public string RoleColor
		{
			get => ServerRoles.Network_myColor;
			set => ServerRoles.SetColor(value);
		}
		public string RoleName
		{
			get => ServerRoles.Network_myText;
			set => ServerRoles.SetText(value);
		}
		public string UnitName
		{
			get => ClassManager.NetworkCurUnitName;
			set
			{
				if (Respawning.NamingRules.UnitNamingManager.RolesWithEnforcedDefaultName.TryGetValue(Role, out Respawning.SpawnableTeamType spawnableTeamType))
					ClassManager.NetworkCurSpawnableTeamType = (byte)spawnableTeamType;
				ClassManager.NetworkCurUnitName = value;
			}
		}
		public float AliveTime => ClassManager.AliveTime;
		public long DeathTime
		{
			get => ClassManager.DeathTime;
			set => ClassManager.DeathTime = value;
		}
		public string GlobalBadge => ServerRoles.NetworkGlobalBadge.Split(new string[] { "Badge text: [" }, StringSplitOptions.None)[1].Split(']')[0];
		public int Ping => Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorServer.Peers[Connection.connectionId].Ping;
		public ushort Ammo12Gauge
		{
			get { try { return Ammo[AmmoType.Ammo12Gauge]; } catch { return 0; } }
			set { try { Ammo[AmmoType.Ammo12Gauge] = value; } catch { } }
		}
		public ushort Ammo556
		{
			get { try { return Ammo[AmmoType.Ammo556]; } catch { return 0; } }
			set { try { Ammo[AmmoType.Ammo556] = value; } catch { } }
		}
		public ushort Ammo44Cal
		{
			get { try { return Ammo[AmmoType.Ammo44Cal]; } catch { return 0; } }
			set { try { Ammo[AmmoType.Ammo44Cal] = value; } catch { } }
		}
		public ushort Ammo762
		{
			get { try { return Ammo[AmmoType.Ammo762]; } catch { return 0; } }
			set { try { Ammo[AmmoType.Ammo762] = value; } catch { } }
		}
		public ushort Ammo9
		{
			get { try { return Ammo[AmmoType.Ammo9]; } catch { return 0; } }
			set { try { Ammo[AmmoType.Ammo9] = value; } catch { } }
		}
		public static IEnumerable<Player> Get(Team team) => List.Where(player => player.Team == team);
		public static IEnumerable<Player> Get(RoleType role) => List.Where(player => player.Role == role);
		public static Player Get(CommandSender sender) => sender == null ? null : Get(sender.SenderId);
		public static Player Get(ReferenceHub referenceHub) => referenceHub == null ? null : Get(referenceHub.gameObject);
		public static Player Get(GameObject gameObject)
		{
			if (gameObject == null) return null;
			Dictionary.TryGetValue(gameObject, out Player player);
			return player;
		}
		public static Player Get(uint netId) => ReferenceHub.TryGetHubNetID(netId, out ReferenceHub hub) ? Get(hub) : null;
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
				if (ArgsPlayers.ContainsKey(args)) return ArgsPlayers[args];
				Player playerFound = null;
				if (short.TryParse(args, out short playerId)) return Get(playerId);
				if (args.EndsWith("@steam") || args.EndsWith("@discord") || args.EndsWith("@northwood") || args.EndsWith("@patreon"))
					foreach (Player pl in List.Where(x => x.UserId == args)) playerFound = pl;
				if (playerFound == null)
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
				if (playerFound != null) ArgsPlayers.Add(args, playerFound);
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
			ServerRoles.Permissions = ServerRoles._globalPerms;
			ServerRoles.RemoteAdminMode = GlobalRemoteAdmin ? ServerRoles.AccessMode.GlobalAccess : ServerRoles.AccessMode.PasswordOverride;
			ServerRoles.TargetOpenRemoteAdmin(false);
		}
		public void RaLogout()
		{
			ServerRoles.RemoteAdmin = false;
			ServerRoles.RemoteAdminMode = ServerRoles.AccessMode.LocalAccess;
			ServerRoles.TargetCloseRemoteAdmin();
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
			writer.WriteInt32(type);
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
		public void SetRole(RoleType newRole, bool lite = false, CharacterClassManager.SpawnReason reason = 0) => ClassManager.SetClassIDAdv(newRole, lite, reason);
		public void ChangeBody(RoleType newRole, bool spawnRagdoll = false, Vector3 newPosition = default, Vector3 newRotation = default, DamageTypes.DamageType damageType = null)
		{
			if (spawnRagdoll) Controllers.Ragdoll.Create(Role, Position, default, default, new PlayerStats.HitInfo(999, Nickname, damageType, Id, false), false, Nickname, 0, Id);
			var items = new Dictionary<ItemType, float>(8);
			foreach (var item in AllItems.Where(x => x != null && x.Base != null))
			{
				float ammo = 0;
				if (item is MicroHid microHid) ammo = microHid.Energy;
				else if (item is Firearm firearm) ammo = firearm.Ammo;
				items.Add(item.Type, ammo);
			}
			var _ahp = Ahp;
			if (damageType == null) damageType = DamageTypes.Com15;
			if (newPosition == default) newPosition = Position;
			if (newRotation == default) newRotation = Rotation;
			BlockSpawnTeleport = true;
			if(newRole == Role)
            {
				Position = newPosition;
				Rotation = newRotation;
				return;
            }
			SetRole(newRole, false, CharacterClassManager.SpawnReason.Respawn);
			MEC.Timing.CallDelayed(0.3f, () =>
			{
				Ahp = _ahp;
				Rotation = newRotation;
				MEC.Timing.CallDelayed(0.3f, () => ResetInventory(items));
				MEC.Timing.CallDelayed(0.2f, () => Position = newPosition);
			});
			void ResetInventory(Dictionary<ItemType, float> newItems)
			{
				ClearInventory();
				if (newItems.Count > 0)
				{
					foreach (var item in newItems)
						AddItem(item.Key, item.Value);
				}
				Item AddItem(ItemType itemType, float ammo)
				{
					Item item = Item.Get(Inventory.ServerAddItem(itemType));
					AttachmentsServerHandler.SetupProvidedWeapon(ReferenceHub, item.Base);
					if (item is MicroHid microHid) microHid.Energy = ammo;
					else if (item is Firearm firearm) firearm.Ammo = (byte)ammo;
					return item;
				}
			}
		}
		public Controllers.Broadcast Broadcast(string message, ushort time, bool instant = false) => Broadcast(time, message, instant);
		public Controllers.Broadcast Broadcast(ushort time, string message, bool instant = false)
		{
			var bc = new Controllers.Broadcast(this, message, time);
			Broadcasts.Add(bc, instant);
			return bc;
		}
		public void ClearBroadcasts() => Broadcasts.Clear();
		public bool Damage(int amount, DamageTypes.DamageType damageType, Player attacker = null)
		{
			if (attacker == null) attacker = this;
			return attacker.PlayerStats.HurtPlayer(new PlayerStats.HitInfo(amount, attacker.Nickname, damageType, attacker.Id, false), GameObject);
		}
		public void Damage(PlayerStats.HitInfo info) => PlayerStats.HurtPlayer(info, GameObject);
		public Item AddItem(ItemType itemType)
		{
			Item item = Item.Get(Inventory.ServerAddItem(itemType));
			AttachmentsServerHandler.SetupProvidedWeapon(ReferenceHub, item.Base);
			if (item is Firearm firearm) firearm.Ammo = firearm.MaxAmmo;
			return item;
		}
		public void AddItem(ItemType itemType, int amount)
		{
			if (amount > 0)
			{
				for (int i = 0; i < amount; i++)
					AddItem(itemType);
			}
		}
		public void AddItem(List<ItemType> items)
		{
			if (items.Count > 0)
			{
				for (int i = 0; i < items.Count; i++)
					AddItem(items[i]);
			}
		}
		public void AddItem(Item item) => AddItem(item.Base);
		public Item AddItem(Pickup pickup) => Item.Get(Inventory.ServerAddItem(pickup.Type, pickup.Serial, pickup.Base));
		public Item AddItem(ItemBase itemBase)
		{
			Item item = Item.Get(itemBase);
			Inventory.UserInventory.Items[itemBase.PickupDropModel.NetworkInfo.Serial] = itemBase;

			itemBase.OnAdded(itemBase.PickupDropModel);
            if (itemBase is InventorySystem.Items.Firearms.Firearm)
				AttachmentsServerHandler.SetupProvidedWeapon(ReferenceHub, itemBase);
			ItemsValue.Add(item);

			Inventory.SendItemsNextFrame = true;
			return item;
		}
		public void AddItem(Item item, int amount)
		{
			if (amount > 0)
			{
				for (int i = 0; i < amount; i++)
					AddItem(item);
			}
		}
		public void AddItem(List<Item> items)
		{
			if (items.Count > 0)
			{
				for (int i = 0; i < items.Count; i++)
					AddItem(items[i]);
			}
		}
		public void DropItem(Item item) => Inventory.ServerDropItem(item.Serial);
		public bool HasItem(ItemType item) => Inventory.UserInventory.Items.Any(tempItem => tempItem.Value.ItemTypeId == item);
		public int CountItems(ItemType item) => Inventory.UserInventory.Items.Count(tempItem => tempItem.Value.ItemTypeId == item);
		public bool RemoveItem(ushort id)
		{
			if (!Inventory.UserInventory.Items.TryGetValue(id, out ItemBase @base))
				return false;
			Inventory.ServerRemoveItem(id, @base.PickupDropModel);
			return true;
		}
		public bool RemoveItem(ItemBase item)
		{
			if (ItemsValue.All(i => i.Base != item))
				return false;
			ItemsValue.Remove(Item.Get(item));
			Inventory.ServerRemoveItem(item.PickupDropModel.NetworkInfo.Serial, item.PickupDropModel);
			return true;
		}
		public bool RemoveItem(Item item) => RemoveItem(item.Base);
		public bool RemoveHandItem() => RemoveItem(ItemInHand);
		public void ResetInventory(List<ItemType> newItems)
		{
			ClearInventory();
			if (newItems.Count > 0)
			{
				foreach (ItemType item in newItems)
					AddItem(item);
			}
		}
		public void ResetInventory(List<ItemBase> newItems)
		{
			ClearInventory();
			if (newItems.Count > 0)
			{
				foreach (ItemBase item in newItems)
					AddItem(item);
			}
		}
		public void ClearInventory()
		{
			Inventory.UserInventory.Items.Clear();
			Inventory.SendItemsNextFrame = true;
			ItemsValue.Clear();
		}
		public void DropItems() => Inventory.ServerDropEverything();
		public Throwable ThrowGrenade(GrenadeType type, bool fullForce = true)
		{
			Throwable throwable = type switch
			{
				GrenadeType.Flashbang => new FlashGrenade(ItemType.GrenadeFlash),
				_ => new ExplosiveGrenade(type == GrenadeType.Scp018 ? ItemType.SCP018 : ItemType.GrenadeHE),
			};
			ThrowItem(throwable, fullForce);
			return throwable;
		}
		public void ThrowItem(Throwable throwable, bool fullForce = true)
		{
			throwable.Base.Owner = ReferenceHub;
			throwable.Throw(fullForce);
		}
		public void Ban(int duration, string reason, string issuer = "API") => PlayerManager.localPlayer.GetComponent<BanPlayer>().BanUser(GameObject, duration, reason, issuer, false);
		public void Kick(string reason, string issuer = "API") => Ban(0, reason, issuer);
		public void Disconnect(string reason = null) => ServerConsole.Disconnect(GameObject, string.IsNullOrEmpty(reason) ? "" : reason);
		public void Kill(DamageTypes.DamageType damageType = default) => PlayerStats.HurtPlayer(new PlayerStats.HitInfo(-1f, "WORLD", damageType, 0, true), GameObject);
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
			if (PlayerEffectsController.AllEffects.TryGetValue(typeof(T), out PlayerEffect playerEffect)) return playerEffect.IsEnabled;
			return false;
		}
		public void DisableAllEffects()
		{
			foreach (KeyValuePair<Type, PlayerEffect> effect in PlayerEffectsController.AllEffects) if (effect.Value.IsEnabled) effect.Value.IsEnabled = false;
		}
		public void DisableEffect<T>() where T : PlayerEffect => PlayerEffectsController.DisableEffect<T>();
		public void DisableEffect(EffectType effect)
		{
			if (TryGetEffect(effect, out var pEffect)) pEffect.IsEnabled = false;
		}
		public void EnableEffect<T>(float duration = 0f, bool addDurationIfActive = false) where T : PlayerEffect => PlayerEffectsController.EnableEffect<T>(duration, addDurationIfActive);
		public bool EnableEffect(string effect, float duration = 0f, bool addDurationIfActive = false) => PlayerEffectsController.EnableByString(effect, duration, addDurationIfActive);
		public void EnableEffect(EffectType effect, float duration = 0f, bool addDurationIfActive = false)
		{
			if (TryGetEffect(effect, out var pEffect)) PlayerEffectsController.EnableEffect(pEffect, duration, addDurationIfActive);
		}
		public void EnableEffect(PlayerEffect effect, float duration = 0, bool addDurationIfActive = false)
		{
			PlayerEffectsController.EnableEffect(effect, duration, addDurationIfActive);
		}
		public T GetEffect<T>() where T : PlayerEffect
		{
			return PlayerEffectsController.GetEffect<T>();
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
		public void ShowHitmark() => GameObject.GetComponent<SingleBulletHitreg>().ShowHitIndicator(PlayerStats.netId, 0.01f, Position);
		public void PlayFallSound() => rh.falldamage.UserCode_RpcDoSound();
		public void Redirect(float timeOffset, ushort port) => PlayerStats.UserCode_RpcRoundrestartRedirect(timeOffset, port);

		private Team GetTeam(RoleType rt)
		{
			switch (rt)
			{
				case RoleType.ChaosConscript:
				case RoleType.ChaosMarauder:
				case RoleType.ChaosRepressor:
				case RoleType.ChaosRifleman:
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
				case RoleType.NtfCaptain:
				case RoleType.NtfPrivate:
				case RoleType.NtfSergeant:
				case RoleType.NtfSpecialist:
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
		internal void CheckEscape()
		{
			RoleType newRole = RoleType.None;
			var changeTeam = false;

			if (Cuffed && CharacterClassManager.CuffedChangeTeam)
			{
				switch (Role)
				{
					case RoleType.Scientist when Cuffer.Faction == Faction.FoundationEnemy:
						changeTeam = true;
						break;

					case RoleType.ClassD when Cuffer.Faction == Faction.FoundationStaff:
						changeTeam = true;
						break;
				}
			}

			switch (Role)
			{
				case RoleType.ClassD when changeTeam:
					newRole = RoleType.NtfPrivate;
					break;

				case RoleType.ClassD:
				case RoleType.Scientist when changeTeam:
					newRole = RoleType.ChaosConscript;
					break;

				case RoleType.Scientist:
					newRole = RoleType.NtfSpecialist;
					break;
			}
			if (newRole == RoleType.None) return;
			var ev = new Events.EscapeEvent(this, newRole);
			Qurre.Events.Invoke.Player.Escape(ev);
			if (!ev.Allowed) return;
			newRole = ev.NewRole;

			var isClassD = Role == RoleType.ClassD;

			ClassManager.SetPlayersClass(newRole, GameObject, CharacterClassManager.SpawnReason.Escaped, true);

			Escape.TargetShowEscapeMessage(Connection, isClassD, changeTeam);

			var tickets = Respawning.RespawnTickets.Singleton;
			switch (Team)
			{
				case Team.MTF when changeTeam:
					RoundSummary.escaped_scientists++;
					tickets.GrantTickets(Respawning.SpawnableTeamType.NineTailedFox,
						GameCore.ConfigFile.ServerConfig.GetInt("respawn_tickets_mtf_classd_cuffed_count", 1), false);
					break;

				case Team.MTF:
					RoundSummary.escaped_scientists++;
					tickets.GrantTickets(Respawning.SpawnableTeamType.NineTailedFox,
						GameCore.ConfigFile.ServerConfig.GetInt("respawn_tickets_mtf_scientist_count", 1), false);
					break;

				case Team.CHI when changeTeam:
					RoundSummary.escaped_ds++;
					tickets.GrantTickets(Respawning.SpawnableTeamType.NineTailedFox,
						GameCore.ConfigFile.ServerConfig.GetInt("respawn_tickets_ci_scientist_cuffed_count", 1), false);
					break;

				case Team.CHI:
					RoundSummary.escaped_ds++;
					tickets.GrantTickets(Respawning.SpawnableTeamType.NineTailedFox,
						GameCore.ConfigFile.ServerConfig.GetInt("respawn_tickets_ci_classd_count", 1), false);
					break;
			}
		}
		public void TeleportToRoom(RoomType room)
		{
			Vector3 roompos = Extensions.GetRoom(room).Position + Vector3.up * 2;
			Position = roompos;
		}
		public void TeleportToRandomRoom()
		{
			RoomType room = (RoomType)UnityEngine.Random.Range(1, 48);
			Position = Extensions.GetRoom(room).Position + Vector3.up * 2;
		}
		public void TeleportToDoor(DoorType door)
		{
			Vector3 doorpos = Extensions.GetDoor(door).Position + Vector3.up;
			Position = doorpos;
		}
		public void TeleportToRandomDoor()
		{
			DoorType door = (DoorType)UnityEngine.Random.Range(1, 41);
			Position = Extensions.GetDoor(door).Position + Vector3.up;
		}
		public class AmmoBoxManager
		{
			private readonly Player player;
			internal AmmoBoxManager(Player pl) => player = pl;
			public ushort this[AmmoType ammo]
			{
				get => player.Inventory.UserInventory.ReserveAmmo[ammo.GetItemType()];
				set => player.Inventory.UserInventory.ReserveAmmo[ammo.GetItemType()] = value;
			}
		}
	}
}