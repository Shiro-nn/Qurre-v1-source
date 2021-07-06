using Qurre.API.Objects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Mirror;
using RemoteAdmin;
using Respawning;
using Interactables.Interobjects.DoorUtils;
using _door = Qurre.API.Controllers.Door;
using _lift = Qurre.API.Controllers.Lift;
using _locker = Qurre.API.Controllers.Locker;
using _ragdoll = Qurre.API.Controllers.Ragdoll;
using _workStation = Qurre.API.Controllers.WorkStation;
using Qurre.API.Controllers;
using static QurreModLoader.umm;
using Grenades;
using LightContainmentZoneDecontamination;
using MapGeneration;
namespace Qurre.API
{
	public static class Map
	{
		public static ListBroadcasts Broadcasts { get; private set; } = new ListBroadcasts(Server.Host);
		public static CassieList Cassies { get; private set; } = new CassieList();
		public static List<_door> Doors { get; } = new List<_door>();
		public static List<_lift> Lifts { get; } = new List<_lift>();
		public static List<_locker> Lockers { get; } = new List<_locker>();
		public static List<Generator> Generators { get; } = new List<Generator>();
		public static List<_ragdoll> Ragdolls { get; } = new List<_ragdoll>();
		public static List<Room> Rooms { get; } = new List<Room>();
		public static List<Tesla> Teslas { get; } = new List<Tesla>();
		public static List<_workStation> WorkStations { get; } = new List<_workStation>();

		public static float WalkSpeedMultiplier
		{
			get => ServerConfigSynchronizer.Singleton.NetworkHumanWalkSpeedMultiplier;
			set => ServerConfigSynchronizer.Singleton.NetworkHumanWalkSpeedMultiplier = value;
		}
		public static float SprintSpeedMultiplier
		{
			get => ServerConfigSynchronizer.Singleton.NetworkHumanSprintSpeedMultiplier;
			set => ServerConfigSynchronizer.Singleton.NetworkHumanSprintSpeedMultiplier = value;
		}
		public static bool DisabledLCZDecontamination
		{
			get => DecontaminationController.Singleton.disableDecontamination;
			set => DecontaminationController.Singleton.disableDecontamination = value;
		}
		public static Vector3 Gravitation
		{
			get => Physics.gravity;
			set => Physics.gravity = value;
		}
		public static float ElevatorsMovingSpeed
		{
			get => Object.FindObjectsOfType<Lift>()[0].movingSpeed;
			set
			{
				foreach (Lift lift in Object.FindObjectsOfType<Lift>()) lift.movingSpeed = value;
			}
		}
		public static bool FemurBreakerState
		{
			get => Object.FindObjectOfType<LureSubjectContainer>().allowContain;
			set => Object.FindObjectOfType<LureSubjectContainer>().SetState(value);
		}
		public static float Seed => SeedSynchronizer.Seed;
		public static float BreakableWindowHp
		{
			get => Object.FindObjectsOfType<BreakableWindow>()[0].health;
			set
			{
				foreach (BreakableWindow window in Object.FindObjectsOfType<BreakableWindow>()) window.health = value;
			}
		}
		public static List<Pickup> Pickups => Object.FindObjectsOfType<Pickup>().ToList();
		public static Controllers.Broadcast Broadcast(string message, ushort duration, bool instant = false)
		{
			var bc = new Controllers.Broadcast(Server.Host, message, duration);
			Broadcasts.Add(bc, instant);
			return bc;
		}
		public static void ClearBroadcasts() => Server.Host.Broadcasts.Clear();
		public static Vector3 GetRandomSpawnPoint(RoleType roleType)
		{
			GameObject randomPosition = Object.FindObjectOfType<SpawnpointManager>().GetRandomPosition(roleType);
			return randomPosition == null ? Vector3.zero : randomPosition.transform.position;
		}
		public static void SpawnGrenade(string grenadeType, Vector3 position)
		{
			GameObject grenade = Object.Instantiate(NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name == grenadeType));
			grenade.transform.position = position;
			NetworkServer.Spawn(grenade);
		}
		public static void SpawnGrenade(bool frag, Vector3 position)
		{
			GrenadeManager gm = Server.Host.GrenadeManager;
			if (frag)
			{
				GrenadeSettings settings = gm.availableGrenades.FirstOrDefault(g => g.inventoryID == ItemType.GrenadeFrag);
				FragGrenade grenade = GameObject.Instantiate(settings.grenadeInstance).GetComponent<FragGrenade>();
				grenade.fuseDuration = 2f;
				grenade.InitData(gm, Vector3.zero, Vector3.zero, 0f);
				grenade.transform.position = position;
				NetworkServer.Spawn(grenade.gameObject);
			}
			else
			{
				GrenadeSettings settings = gm.availableGrenades.FirstOrDefault(g => g.inventoryID == ItemType.GrenadeFlash);
				FlashGrenade grenade = GameObject.Instantiate(settings.grenadeInstance).GetComponent<FlashGrenade>();
				grenade.fuseDuration = 2f;
				grenade.InitData(gm, Vector3.zero, Vector3.zero, 0f);
				grenade.transform.position = position;
				NetworkServer.Spawn(grenade.gameObject);
			}
		}
		public static void Explode(Vector3 position, GrenadeType grenadeType = GrenadeType.Grenade, Player player = null)
		{
			if (player == null) player = Server.Host;
			var component = player.GrenadeManager;
			var component2 = Object.Instantiate(component.availableGrenades[(int)grenadeType].grenadeInstance).GetComponent<Grenades.Grenade>();
			component2.Grenade_FullInitData(component, position, Quaternion.identity, Vector3.zero, Vector3.zero, Team.RIP);
			component2.NetworkfuseTime = 0.10000000149011612;
			NetworkServer.Spawn(component2.gameObject);
		}
		public static GameObject SpawnBot(RoleType role, string name, float health, Vector3 position, Vector3 rotation, Vector3 scale)
		{
			GameObject gameObject = Object.Instantiate(NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name == "Player"));
			CharacterClassManager component = gameObject.GetComponent<CharacterClassManager>();
			if (component != null)
			{
				component.CurClass = role;
				gameObject.GetComponent<NicknameSync>().DisplayName = name;
				gameObject.GetComponent<PlayerStats>().Health = health;
				gameObject.GetComponent<QueryProcessor>().PlayerId = 1337228;
				gameObject.GetComponent<QueryProcessor>().NetworkPlayerId = 1337228;
				gameObject.transform.localScale = scale;
				gameObject.transform.position = position;
				gameObject.transform.eulerAngles = rotation;
				if (health == -1) component.GodMode = true;
				NetworkServer.Spawn(gameObject);
			}
			return gameObject;
		}
		public static GameObject SpawnPlayer(RoleType role, string name, string userSteamID, Vector3 position, Vector3 rotation, Vector3 scale)
		{
			ReferenceHub hub = ReferenceHub.GetHub(NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name == "Player"));

			hub.characterClassManager.NetworkCurClass = role;
			hub.characterClassManager.NetworkSyncedUserId = userSteamID;
			hub.nicknameSync.Network_displayName = name;
			hub.gameObject.name = "CAMERA1";
			hub.gameObject.tag = "BOT";
			hub.gameObject.transform.localScale = scale;
			hub.gameObject.transform.position = position;
			hub.gameObject.transform.eulerAngles = rotation;
			NetworkServer.Spawn(hub.gameObject);
			return hub.gameObject;
		}
		public static void ContainSCP106(ReferenceHub executor) => PlayerManager.localPlayer.GetComponent<PlayerInteract>().CallRpcContain106(executor.gameObject);
		public static void ShakeScreen(float times) => ExplosionCameraShake.singleton.Shake(times);
		public static void SetIntercomSpeaker(Player player)
		{
			if (player != null)
			{
				GameObject gameObject = player.GameObject;
				gameObject.GetComponent<CharacterClassManager>().IntercomMuted = false;
				Intercom.host.RequestTransmission(gameObject);
				return;
			}
			foreach (CharacterClassManager ccm in Object.FindObjectsOfType<CharacterClassManager>())
			{
				if (ccm.IntercomMuted) ccm.IntercomMuted = false;
			}
		}
		public static void PlayCIEntranceMusic() => RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.UponRespawn, SpawnableTeamType.ChaosInsurgency);
		public static void PlayIntercomSound(bool start) => PlayerManager.localPlayer.GetComponent<Intercom>().RpcPlaySound(start, 0);
		public static void PlaceBlood(Vector3 position, int type, float size) => PlayerManager.localPlayer.GetComponent<CharacterClassManager>().RpcPlaceBlood(position, type, size);
		public static void PlayAmbientSound(int id) => PlayerManager.localPlayer.GetComponent<AmbientSoundPlayer>().RpcPlaySound(id);
		public static void ShowHint(string message, float duration)
		{
			foreach (Player player in Player.List) player.ShowHint(message, duration);
		}
		public static void AnnounceNtfEntrance(int scpsLeft, int mtfNumber, char mtfLetter)
		{
			if (scpsLeft == 0) Cassie.Send($"MTFUnit epsilon 11 designated NATO_{mtfLetter} {mtfNumber} HasEntered AllRemaining NoSCPsLeft", true, true, true);
			else Cassie.Send($"MTFUnit epsilon 11 designated NATO_{mtfLetter} {mtfNumber} HasEntered AllRemaining AwaitingRecontainment {scpsLeft} scpsubjects", true, true, true);
		}
		public static void AnnounceScpKill(string scpNumber, Player killer = null)
		{
			GameObject gameObject = GameObject.Find("Host");
			RoleType rt;
			switch ("SCP-" + scpNumber)
			{
				case "49":
				case "049":
					rt = RoleType.Scp049;
					break;
				case "79":
				case "079":
					rt = RoleType.Scp079;
					break;
				case "96":
				case "096":
					rt = RoleType.Scp096;
					break;
				case "106":
					rt = RoleType.Scp106;
					break;
				case "173":
					rt = RoleType.Scp173;
					break;
				case "939-53":
				case "939_53":
				case "93953":
					rt = RoleType.Scp93953;
					break;
				case "939-89":
				case "939_89":
				case "93989":
					rt = RoleType.Scp93989;
					break;
				default:
					rt = RoleType.None;
					break;
			}
			CharacterClassManager component2 = gameObject.GetComponent<CharacterClassManager>();
			NineTailedFoxAnnouncer.AnnounceScpTermination(component2.Classes.SafeGet(rt), new PlayerStats.HitInfo(-1f, killer?.Nickname ?? "", DamageTypes.None, killer?.Id ?? (-1)), "");
		}
		public static void DecontaminateLCZ()
		{
			DecontaminationController.Singleton.FinishDecontamination();
			DecontaminationController.Singleton.NetworkRoundStartTime = -1.0;
		}
		public static void Remove(RemovableObject removable)
		{
			switch (removable)
			{
				case RemovableObject.Doors:
					foreach (DoorVariant door in Object.FindObjectsOfType<DoorVariant>()) Object.Destroy(door.gameObject);
					break;
				case RemovableObject.Generators:
					foreach (Generator079 generator in Object.FindObjectsOfType<Generator079>()) Object.Destroy(generator.gameObject);
					break;
				case RemovableObject.Lockers:
					foreach (LockerManager locker in Object.FindObjectsOfType<LockerManager>()) Object.Destroy(locker.gameObject);
					break;
				case RemovableObject.Teslagates:
					foreach (TeslaGate teslaGate in Object.FindObjectsOfType<TeslaGate>()) Object.Destroy(teslaGate.gameObject);
					break;
				case RemovableObject.Windows:
					foreach (BreakableWindow window in Object.FindObjectsOfType<BreakableWindow>()) Object.Destroy(window.gameObject);
					break;
				case RemovableObject.Workstations:
					foreach (WorkStation workStation in Object.FindObjectsOfType<WorkStation>()) Object.Destroy(workStation.gameObject);
					break;
				case RemovableObject.Rooms:
					foreach (NetworkIdentity netId in Object.FindObjectsOfType<NetworkIdentity>()) if (netId.name.Contains("All")) Object.Destroy(netId);
					break;
			}
		}
		internal static void AddObjects()
		{
			Broadcasts = new ListBroadcasts(Server.Host);
			Cassies = new CassieList();
			foreach (var room in Server.GetObjectsOf<Transform>().Where(x => x.CompareTag("Room") || x.name == "Root_*&*Outside Cams" || x.name == "PocketWorld"))
				Rooms.Add(new Room(room.gameObject));
			foreach (var tesla in Server.GetObjectsOf<TeslaGate>()) Teslas.Add(new Tesla(tesla));
			foreach (var station in Server.GetObjectsOf<WorkStation>()) WorkStations.Add(new _workStation(station));
			foreach (var door in Server.GetObjectsOf<DoorVariant>()) Doors.Add(new _door(door));
			foreach (var locker in LockerManager.singleton.lockers) Lockers.Add(new _locker(locker));
			foreach (var interactable in Interface079.singleton.allInteractables)
			{
				foreach (var zoneroom in interactable.currentZonesAndRooms)
				{
					try
					{
						var room = Rooms.FirstOrDefault(x => x.Name == zoneroom.currentRoom);
						var door = interactable.GetComponentInParent<DoorVariant>();
						if (room == null || door == null) continue;
						var sdoor = Doors.FirstOrDefault(x => x.GameObject == door.gameObject);
						sdoor.Rooms.Add(room);
						room.Doors.Add(sdoor);
					}
					catch { }
				}
			}
		}
		internal static void ClearObjects()
		{
			Teslas.Clear();
			Doors.Clear();
			Lifts.Clear();
			Lockers.Clear();
			Rooms.Clear();
			Generators.Clear();
			WorkStations.Clear();
			Ragdolls.Clear();
			Heavy.Recontained079 = false;
		}
	}
}