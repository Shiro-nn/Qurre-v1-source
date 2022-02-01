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
using Assets._Scripts.Dissonance;
using MapGeneration;
using NorthwoodLib;
using PlayerStatsSystem;
using Qurre.API.Addons;
namespace Qurre.API
{
	public class Player
	{
		private readonly ReferenceHub rh;
		private readonly GameObject go;
		private string ui;
		private string _tag = "";
		private Radio radio;
		private Escape escape;
		internal readonly List<Item> ItemsValue = new(8);
		internal List<KillElement> _kills = new();
		/// <summary>
		/// [�ڧߧڧ�ڧѧݧڧ٧ѧ�ڧ�<see cref="Player"/>]
		/// [��ʼ�� <see cref="Player"/>]
		/// </summary>
		public Player(ReferenceHub RH)
		{
			rh = RH;
			go = RH.gameObject;
			ui = RH.characterClassManager.UserId;
			Scp079Controller = new Scp079(this);
			Scp096Controller = new Scp096(this);
			Scp106Controller = new Scp106(this);
			Scp173Controller = new Scp173();
			Broadcasts = new ListBroadcasts();
			Ammo = new AmmoBoxManager(this);
			BlockSpawnTeleport = false;
		}
		/// <summary>
		/// [���ݧ� ���ݧ��֧ߧڧ� ��ܧѧ٧ѧߧߧ�ԧ� �ڧԧ��ܧ� �ڧ���ݧ�٧�֧��� <see cref="UnityEngine.GameObject"/>]
		/// [ͨ��<see cref="UnityEngine.GameObject"/>��ȡָ�����]
		/// </summary>
		public Player(GameObject gameObject) => new Player(ReferenceHub.GetHub(gameObject));
		
		public static Dictionary<GameObject, Player> Dictionary { get; } = new();
		public static Dictionary<int, Player> IdPlayers = new();
		public static Dictionary<string, Player> UserIDPlayers { get; set; } = new();
		public static Dictionary<string, Player> ArgsPlayers { get; set; } = new();
		public static IEnumerable<Player> List => Dictionary.Values.Where(x => !x.Bot);
		public IEnumerator<KillElement> Kills => (IEnumerator<KillElement>)_kills;
		public int KillsCount => _kills.Count();
		public int DeathsCount { get; internal set; }
		public ReferenceHub ReferenceHub => rh;
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
		public DissonanceUserSetup Dissonance => rh.dissonanceUserSetup;
		public ServerRoles ServerRoles => rh.serverRoles;
		public CharacterClassManager ClassManager => rh.characterClassManager;
		public AnimationController AnimationController => rh.animationController;
		public PlayerStats PlayerStats => rh.playerStats;
		public Scp079PlayerScript Scp079PlayerScript => rh.scp079PlayerScript;
		public Scp106PlayerScript Scp106PlayerScript => rh.scp106PlayerScript;
		public QueryProcessor QueryProcessor => rh.queryProcessor;
		public PlayerEffectsController PlayerEffectsController => rh.playerEffectsController;
		public NicknameSync NicknameSync => rh.nicknameSync;
		public PlayerMovementSync Movement => rh.playerMovementSync;
		public string Tag
		{
			get => _tag;
			set
			{
				if (value is null) return;
				_tag = value;
			}
		}
		public int Id
		{
			get => rh.queryProcessor.NetworkPlayerId;
			set => rh.queryProcessor.NetworkPlayerId = value;
		}
		public AuthType AuthType
		{
			get
			{
				if (string.IsNullOrEmpty(UserId))
					return AuthType.Unknown;
				int index = UserId.LastIndexOf('@');
				if (index == -1) return AuthType.Unknown;
				return UserId.Substring(index + 1) switch
				{
					"steam" => AuthType.Steam,
					"discord" => AuthType.Discord,
					"northwood" => AuthType.Northwood,
					_ => AuthType.Unknown,
				};
			}
		}
		/// <summary>
		/// [Steam64Id (SomeId@steam)]
		/// </summary>
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
		/// <summary>
		/// [����֧ާ֧ߧߧ�� ����٧ӧڧ�� �� �ڧԧ��]
		/// [��Ϸ�ڵ���ʱ�ǳ�]
		/// </summary>
		public string DisplayNickname
		{
			get => rh.nicknameSync.Network_displayName;
			set => rh.nicknameSync.Network_displayName = value;
		}
		/// <summary>
		/// [����٧ӧڧ��]
		/// [�ǳ�]
		/// </summary>
		public string Nickname
		{
			get => rh.nicknameSync.Network_myNickSync;
			internal set => rh.nicknameSync.Network_myNickSync = value;
		}
		public bool DoNotTrack => ServerRoles.DoNotTrack;
		/// <summary>
		/// [�� �է������� �ѧէާڧߧڧ���ѧ���� �ڧݧ� �ߧ֧�]
		/// [�Ƿ�ӵ�й���ԱȨ��]
		/// </summary>
		public bool RemoteAdminAccess => ServerRoles.RemoteAdmin;
		/// <summary>
		/// [���ܧݧ��ڧ�� ��֧اڧ� Overwatch �ڧݧ� �ߧ֧�]
		/// [�Ƿ���Overwatchģʽ]
		/// </summary>
		public bool Overwatch
		{
			get => ServerRoles.OverwatchEnabled;
			set => ServerRoles.UserCode_TargetSetOverwatch(NetworkIdentity.connectionToClient, value);
		}
		/// <summary>
		/// [���ѧܧ�ӧ�ڧܧ��]
		/// [������]
		/// </summary>
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
		/// <summary>
		/// [���ӧ�٧ѧ� ��� �ڧݧ� �ߧ֧�]
		/// [�Ƿ�����]
		/// </summary>
		public bool Cuffed => DisarmedPlayers.IsDisarmed(Inventory);
		/// <summary>
		/// []
		/// []
		/// </summary>
		public Vector3 Position
		{
			get => rh.playerMovementSync.GetRealPosition();
			set => rh.playerMovementSync.OverridePosition(value, 0f);
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public Vector2 Rotation
		{
			get => Movement.RotationSync;
			set => Movement.NetworkRotationSync = value;
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
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
		/// <summary>
		/// []
		/// []
		/// </summary>
		public GameObject LookingAt
		{
			get
			{
				if (!Physics.Raycast(rh.PlayerCameraReference.transform.position, rh.PlayerCameraReference.transform.forward, out RaycastHit raycastthit, 100f))
					return null;
				return raycastthit.transform.gameObject;
			}
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public bool Noclip
		{
			get => rh.characterClassManager.NoclipEnabled;
			set => rh.characterClassManager.SetNoclip(value);
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public Team Team => GetTeam(Role);
		/// <summary>
		/// []
		/// []
		/// </summary>
		public Side Side => GetSide(Team);
		/// <summary>
		/// []
		/// []
		/// </summary>
		public Faction Faction => ClassManager.Faction;
		/// <summary>
		/// []
		/// []
		/// </summary>
		public RoleType Role
		{
			get => ClassManager.CurClass;
			set => SetRole(value);
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public PlayerMovementState MoveState
		{
			get => AnimationController.MoveState;
			set => AnimationController.MoveState = value;
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public string Ip => NetworkIdentity.connectionToClient.address;
		/// <summary>
		/// []
		/// []
		/// </summary>
		public NetworkConnection Connection => Scp079PlayerScript.connectionToClient;
		/// <summary>
		/// []
		/// []
		/// </summary>
		public bool IsHost => ClassManager.IsHost;
		/// <summary>
		/// []
		/// []
		/// </summary>
		public bool FriendlyFire { get; set; }
		/// <summary>
		/// []
		/// []
		/// </summary>
		public bool Zoomed { get; internal set; } = false;
		/// <summary>
		/// []
		/// []
		/// </summary>
		public bool UseStamina { get; set; } = true;
		/// <summary>
		/// []
		/// []
		/// </summary>
		public bool Invisible { get; set; }
		/// <summary>
		/// []
		/// []
		/// </summary>
		public bool Bot { get; internal set; } = false;
		///<summary>
		///<para>After spawning, it becomes false again.</para>
		///</summary>
		public bool BlockSpawnTeleport { get; set; } = false;
		/// <summary>
		/// []
		/// []
		/// </summary>
		public bool BypassMode
		{
			get => ServerRoles.BypassMode;
			set => ServerRoles.BypassMode = value;
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public bool Muted
		{
			get => rh.dissonanceUserSetup.AdministrativelyMuted;
			set
			{
				if (value) MuteHandler.IssuePersistentMute(UserId);
				else MuteHandler.RevokePersistentMute(UserId);
			}
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public bool IntercomMuted
		{
			get => ClassManager.NetworkIntercomMuted;
			set => ClassManager.NetworkIntercomMuted = value;
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public bool GodMode
		{
			get => ClassManager.GodMode;
			set => ClassManager.GodMode = value;
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public float Hp
		{
			get => PlayerStats.StatModules[0].CurValue;
			set
			{
				PlayerStats.StatModules[0].CurValue = value;

				if (value > MaxHp)
					MaxHp = (int)value;
			}
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		private int mhp = 100;
		/// <summary>
		/// []
		/// []
		/// </summary>
		public int MaxHp
		{
			get => mhp;
			set => mhp = value;
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public float Ahp
		{
			get => PlayerStats.StatModules[1].CurValue;
			set
			{
				if (value > MaxAhp)
					MaxAhp = Mathf.CeilToInt(value);
				PlayerStats.StatModules[1].CurValue = value;
			}
		}
		public float MaxAhp
		{
			get => ((AhpStat)PlayerStats.StatModules[1])._maxSoFar;
			set => ((AhpStat)PlayerStats.StatModules[1])._maxSoFar = value;
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public List<AhpStat.AhpProcess> AhpActiveProcesses
		{
			get => ((AhpStat)PlayerStats.StatModules[1])._activeProcesses;
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public ItemIdentifier CurrentItem
		{
			get => Inventory.NetworkCurItem;
			set => Inventory.NetworkCurItem = value;
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public ItemBase CurInstance
		{
			get => Inventory.CurInstance;
			set => Inventory.CurInstance = value;
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public IReadOnlyCollection<Item> AllItems => ItemsValue.AsReadOnly();
		/// <summary>
		/// []
		/// []
		/// </summary>
		public ItemType ItemTypeInHand => Inventory.CurItem.TypeId;
		/// <summary>
		/// []
		/// []
		/// </summary>
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
		/// <summary>
		/// []
		/// []
		/// </summary>
		public Stamina Stamina => rh.fpc.staminaController;
		/// <summary>
		/// []
		/// []
		/// </summary>
		public float StaminaUsage
		{
			get => Stamina.StaminaUse * 100;
			set => Stamina.StaminaUse = (value / 100f);
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public void ResetStamina() => rh.fpc.ResetStamina();
		/// <summary>
		/// []
		/// []
		/// </summary>
		public PlayableScpsController ScpsController => rh.scpsController;
		/// <summary>
		/// []
		/// []
		/// </summary>
		public PlayableScps.PlayableScp CurrentScp
		{
			get => ScpsController.CurrentScp;
			set => ScpsController.CurrentScp = value;
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public void HideTag() => ClassManager.CmdRequestHideTag();
		/// <summary>
		/// []
		/// []
		/// </summary>
		public void ShowTag() => ClassManager.CmdRequestShowTag(false);
		/// <summary>
		/// []
		/// []
		/// </summary>
		public string HiddenBadge => ServerRoles.HiddenBadge;
		/// <summary>
		/// []
		/// []
		/// </summary>
		public bool BadgeHidden
		{
			get
			{
				if (string.IsNullOrEmpty(HiddenBadge) || string.IsNullOrWhiteSpace(HiddenBadge)) return false;
				else return true;
			}
			set
			{
				if (value) HideTag();
				else ShowTag();
			}
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public ZoneType Zone => Room.Zone;
		/// <summary>
		/// []
		/// []
		/// </summary>
		public Room Room
		{
			get => RoomIdUtils.RoomAtPosition(Position).GetRoom() ?? Map.Rooms.OrderBy(x => Vector3.Distance(x.Position, Position)).FirstOrDefault();
			set => Position = value.Position + Vector3.up * 2;
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public CommandSender Sender
		{
			get
			{
				if (IsHost) return ServerConsole._scs;
				return QueryProcessor._sender;
			}
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public bool GlobalRemoteAdmin => ServerRoles.RemoteAdminMode == ServerRoles.AccessMode.GlobalAccess;
		/// <summary>
		/// []
		/// []
		/// </summary>
		public string GroupName
		{
			get => ServerStatic.GetPermissionsHandler()._members.TryGetValue(UserId, out string groupName) ? groupName : null;
			set => ServerStatic.GetPermissionsHandler()._members[UserId] = value;
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public UserGroup Group
		{
			get => ServerRoles.Group;
			set => ServerRoles.SetGroup(value, false, false, value.Cover);
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public string RoleColor
		{
			get => ServerRoles.Network_myColor;
			set => ServerRoles.SetColor(value);
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public string RoleName
		{
			get => ServerRoles.Network_myText;
			set => ServerRoles.SetText(value);
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
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
		/// <summary>
		/// []
		/// []
		/// </summary>
		public float AliveTime => ClassManager.AliveTime;
		/// <summary>
		/// []
		/// []
		/// </summary>
		public long DeathTime
		{
			get => ClassManager.DeathTime;
			set => ClassManager.DeathTime = value;
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public string GlobalBadge
		{
			get
			{
				if (string.IsNullOrEmpty(ServerRoles.NetworkGlobalBadge)) return string.Empty;
				return ServerRoles.NetworkGlobalBadge.Split(new string[] { "Badge text: [" }, StringSplitOptions.None)[1].Split(']')[0];
			}
		}
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
		/// <summary>
		/// []
		/// []
		/// </summary>
		public static IEnumerable<Player> Get(Team team) => List.Where(player => player.Team == team);
		/// <summary>
		/// []
		/// []
		/// </summary>
		public static IEnumerable<Player> Get(RoleType role) => List.Where(player => player.Role == role);
		/// <summary>
		/// []
		/// []
		/// </summary>
		public static Player Get(CommandSender sender) => sender == null ? null : Get(sender.SenderId);
		/// <summary>
		/// []
		/// []
		/// </summary>
		public static Player Get(ReferenceHub referenceHub) => referenceHub == null ? null : Get(referenceHub.gameObject);
		/// <summary>
		/// []
		/// []
		/// </summary>
		public static Player Get(GameObject gameObject)
		{
			if (gameObject == null) return null;
			Dictionary.TryGetValue(gameObject, out Player player);
			return player;
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public static Player Get(uint netId) => ReferenceHub.TryGetHubNetID(netId, out ReferenceHub hub) ? Get(hub) : null;
		/// <summary>
		/// []
		/// []
		/// </summary>
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
		/// <summary>
		/// []
		/// []
		/// </summary>
		public static Player Get(string args)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(args))
					return null;
				if (ArgsPlayers.TryGetValue(args, out Player playerFound) && playerFound?.ReferenceHub != null)
					return playerFound;
				if (int.TryParse(args, out int id))
					return Get(id);
				if (args.EndsWith("@steam") || args.EndsWith("@discord") || args.EndsWith("@northwood") || args.EndsWith("@patreon"))
				{
					foreach (Player player in Dictionary.Values)
					{
						if (player.UserId == args)
						{
							playerFound = player;
							break;
						}
					}
				}
				else
				{
					int lastnameDifference = 31;
					string firstString = args.ToLower();
					foreach (Player player in Dictionary.Values)
					{
						if (player.Nickname == null) continue;
						if (!player.Nickname.Contains(args, StringComparison.OrdinalIgnoreCase))
							continue;
						string secondString = player.Nickname;
						int nameDifference = secondString.Length - firstString.Length;
						if (nameDifference < lastnameDifference)
						{
							lastnameDifference = nameDifference;
							playerFound = player;
						}
					}
				}
				if (playerFound != null)
					ArgsPlayers[args] = playerFound;
				return playerFound;
			}
			catch (Exception ex)
			{
				Log.Error($"[API.Player.Get(string)] umm, error: {ex}");
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
		/// <summary>
		/// []
		/// []
		/// </summary>
		public void OpenReportWindow(string text) => GameConsoleTransmission.SendToClient(Connection, "[REPORTING] " + text, "white");
		/// <summary>
		/// []
		/// []
		/// </summary>
		public void RemoveDisplayInfo(PlayerInfoArea playerInfo) => NicknameSync.Network_playerInfoToShow &= ~playerInfo;
		public void AddDisplayInfo(PlayerInfoArea playerInfo) => NicknameSync.Network_playerInfoToShow |= playerInfo;
		/// <summary>
		/// []
		/// []
		/// </summary>
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
		/// <summary>
		/// []
		/// []
		/// </summary>
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
		/// <summary>
		/// []
		/// []
		/// </summary>
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
		/// <summary>
		/// []
		/// []
		/// </summary>
		public void SetRole(RoleType newRole, bool lite = false, CharacterClassManager.SpawnReason reason = 0) => ClassManager.SetClassIDAdv(newRole, lite, reason);
		/// <summary>
		/// []
		/// []
		/// </summary>
		public void ChangeBody(RoleType newRole, bool spawnRagdoll = false, Vector3 newPosition = default, Vector2 newRotation = default, string deathReason = "")
		{
			if (spawnRagdoll) Controllers.Ragdoll.Create(Role, Position, default, new CustomReasonDamageHandler(deathReason), this);
			if (newPosition == default) newPosition = Position;
			if (newRotation == default) newRotation = Rotation;
			ChangeModel(newRole);
			Position = newPosition;
			Rotation = newRotation;
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public Controllers.Broadcast Broadcast(string message, ushort time, bool instant = false) => Broadcast(time, message, instant);
		public Controllers.Broadcast Broadcast(ushort time, string message, bool instant = false)
		{
			var bc = new Controllers.Broadcast(this, message, time);
			Broadcasts.Add(bc, instant);
			return bc;
		}
		public void ClearBroadcasts() => Broadcasts.Clear();
		/// <summary>
		/// []
		/// []
		/// </summary>
		public bool Damage(float damage, string deathReason)
		{
			return PlayerStats.DealDamage(new CustomReasonDamageHandler(deathReason, damage));
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public bool Damage(float damage, DeathTranslation deathReason)
		{
			return PlayerStats.DealDamage(new UniversalDamageHandler(damage, deathReason));
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public bool Damage(float damage, DeathTranslation deathReason, Player attacker)
		{
			if (attacker == null) return PlayerStats.DealDamage(new UniversalDamageHandler(damage, deathReason));
			return PlayerStats.DealDamage(new ScpDamageHandler(attacker.ReferenceHub, damage, deathReason));
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public bool DealDamage(DamageHandlerBase handler) => PlayerStats.DealDamage(handler);
		public Item AddItem(ItemType itemType)
		{
			Item item = Item.Get(Inventory.ServerAddItem(itemType));
			AttachmentsServerHandler.SetupProvidedWeapon(ReferenceHub, item.Base);
			if (item is Firearm firearm) firearm.Ammo = firearm.MaxAmmo;
			return item;
		}
		public void AddItem(ItemType itemType, int amount)
		{
			if (0 >= amount) return;
			for (int i = 0; i < amount; i++)
				AddItem(itemType);
		}
		public void AddItem(List<ItemType> items)
		{
			if (0 >= items.Count) return;
			for (int i = 0; i < items.Count; i++)
				AddItem(items[i]);
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
			if (0 >= amount) return;
			for (int i = 0; i < amount; i++)
				AddItem(item);
		}
		public void AddItem(List<Item> items)
		{
			if (0 >= items.Count) return;
			for (int i = 0; i < items.Count; i++)
				AddItem(items[i]);
		}
		public void DropItem(Item item) => Inventory.ServerDropItem(item.Serial);
		public bool HasItem(ItemType item) => Inventory.UserInventory.Items.Any(tempItem => tempItem.Value.ItemTypeId == item);
		public int CountItems(ItemType item) => Inventory.UserInventory.Items.Count(tempItem => tempItem.Value.ItemTypeId == item);
		public bool RemoveItem(Item item, bool destroy = true)
		{
			if (!ItemsValue.Contains(item)) return false;
			if (!Inventory.UserInventory.Items.ContainsKey(item.Serial))
			{
				ItemsValue.Remove(item);
				return false;
			}
			if (destroy) Inventory.ServerRemoveItem(item.Serial, null);
			else
			{
				if (ItemInHand != null && ItemInHand.Serial == item.Serial)
					Inventory.NetworkCurItem = ItemIdentifier.None;
				Inventory.UserInventory.Items.Remove(item.Serial);
				ItemsValue.Remove(item);
				Inventory.SendItemsNextFrame = true;
			}

			return true;
		}
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
		/// <summary>
		/// []
		/// []
		/// </summary>
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
		/// <summary>
		/// []
		/// []
		/// </summary>
		public void ThrowItem(Throwable throwable, bool fullForce = true)
		{
			throwable.Base.Owner = ReferenceHub;
			throwable.Throw(fullForce);
		}
		public void Ban(int duration, string reason, string issuer = "API") => PlayerManager.localPlayer.GetComponent<BanPlayer>().BanUser(GameObject, duration, reason, issuer, false);
		public void Kick(string reason, string issuer = "API") => Ban(0, reason, issuer);
		public void Disconnect(string reason = null) => ServerConsole.Disconnect(GameObject, string.IsNullOrEmpty(reason) ? "" : reason);
		public void Kill(DeathTranslation deathReason) => PlayerStats.DealDamage(new UniversalDamageHandler(-1, deathReason));
		public void Kill(string deathReason = "") => PlayerStats.DealDamage(new CustomReasonDamageHandler(deathReason));
		/// <summary>
		/// []
		/// []
		/// </summary>
		public void ChangeModel(RoleType newModel)
		{
			GameObject gameObject = GameObject;
			CharacterClassManager ccm = gameObject.GetComponent<CharacterClassManager>();
			NetworkIdentity identity = gameObject.GetComponent<NetworkIdentity>();
			RoleType FirstRole = Role;
			ccm.CurClass = newModel;
			ObjectDestroyMessage destroyMessage = new() { netId = identity.netId };
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
		public void EnableEffect(EffectType effect, float duration = 0f, bool addDurationIfActive = false)
		{
			if (TryGetEffect(effect, out var pEffect)) PlayerEffectsController.EnableEffect(pEffect, duration, addDurationIfActive);
		}
		public void EnableEffect<T>(float duration = 0f, bool addDurationIfActive = false) where T : PlayerEffect => PlayerEffectsController.EnableEffect<T>(duration, addDurationIfActive);
		public bool EnableEffect(string effect, float duration = 0f, bool addDurationIfActive = false) => PlayerEffectsController.EnableByString(effect, duration, addDurationIfActive);
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
		/// <summary>
		/// []
		/// []
		/// </summary>
		public void BodyDelete()
		{
			foreach (var doll in Map.Ragdolls.Where(x => x.Owner == this)) doll.Destroy();
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
			return team switch
			{
				Team.SCP => Side.SCP,
				Team.MTF or Team.RSC => Side.MTF,
				Team.CHI or Team.CDP => Side.CHAOS,
				Team.TUT => Side.TUTORIAL,
				_ => Side.NONE,
			};
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

			if (!Server.RealEscape)
			{
				Position = Map.GetRandomSpawnPoint(newRole);
				BlockSpawnTeleport = true;
				DropItems();
			}
			ClassManager.SetPlayersClass(newRole, GameObject, CharacterClassManager.SpawnReason.Escaped, Server.RealEscape);

			Escape.TargetShowEscapeMessage(Connection, isClassD, changeTeam);

			var tickets = Respawning.RespawnTickets.Singleton;
			switch (Team)
			{
				case Team.MTF when changeTeam:
					RoundSummary.EscapedScientists++;
					tickets.GrantTickets(Respawning.SpawnableTeamType.NineTailedFox,
						GameCore.ConfigFile.ServerConfig.GetInt("respawn_tickets_mtf_classd_cuffed_count", 1), false);
					break;

				case Team.MTF:
					RoundSummary.EscapedScientists++;
					tickets.GrantTickets(Respawning.SpawnableTeamType.NineTailedFox,
						GameCore.ConfigFile.ServerConfig.GetInt("respawn_tickets_mtf_scientist_count", 1), false);
					break;

				case Team.CHI when changeTeam:
					RoundSummary.EscapedClassD++;
					tickets.GrantTickets(Respawning.SpawnableTeamType.NineTailedFox,
						GameCore.ConfigFile.ServerConfig.GetInt("respawn_tickets_ci_scientist_cuffed_count", 1), false);
					break;

				case Team.CHI:
					RoundSummary.EscapedClassD++;
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
			DoorType door = (DoorType)UnityEngine.Random.Range(1, 42);
			Position = Extensions.GetDoor(door).Position + Vector3.up;
		}
		/// <summary>
		/// []
		/// []
		/// </summary>
		public float DistanceTo(Player player) => Vector3.Distance(Position, player.Position);
		/// <summary>
		/// []
		/// []
		/// </summary>
		public float DistanceTo(Vector3 position) => Vector3.Distance(Position, position);
		/// <summary>
		/// []
		/// []
		/// </summary>
		public float DistanceTo(GameObject Object) => Vector3.Distance(Position, Object.transform.localPosition);
		public class AmmoBoxManager
		{
			private readonly Player player;
			internal AmmoBoxManager(Player pl) => player = pl;
			public ushort this[AmmoType ammo]
			{
				get
				{
					if (player.Inventory.UserInventory.ReserveAmmo.TryGetValue(ammo.GetItemType(), out var amount))
						return amount;
					return 0;
				}
				set
				{
					player.Inventory.UserInventory.ReserveAmmo[ammo.GetItemType()] = value;
					player.Inventory.SendAmmoNextFrame = true;
				}
			}
		}
		[Obsolete("Use 'Movement'")]
		public PlayerMovementSync PlayerMovementSync => rh.playerMovementSync;
		[Obsolete("Use 'HiddenBadge'")]
		public string HiddenTag => HiddenBadge;

		[Obsolete("Use 'BadgeHidden'")]
		public bool TagHidden
		{
			get => BadgeHidden;
			set => BadgeHidden = value;
		}
	}
}