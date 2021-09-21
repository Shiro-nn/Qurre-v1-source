using Qurre.API.Objects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Mirror;
using Respawning;
using Interactables.Interobjects.DoorUtils;
using _lift = Qurre.API.Controllers.Lift;
using _locker = Qurre.API.Controllers.Locker;
using _ragdoll = Qurre.API.Controllers.Ragdoll;
using _workStation = Qurre.API.Controllers.WorkStation;
using Qurre.API.Controllers;
using LightContainmentZoneDecontamination;
using MapGeneration;
using InventorySystem.Items.Firearms.Attachments;
using Scp914;
using Qurre.API.Controllers.Items;
using InventorySystem.Items.Pickups;
using System;

namespace Qurre.API
{
	public static class Map
	{
		public static CassieList Cassies { get; private set; } = new CassieList();
		public static List<Door> Doors { get; } = new List<Door>();
		public static List<_lift> Lifts { get; } = new List<_lift>();
		public static List<_locker> Lockers { get; } = new List<_locker>();
		public static List<Generator> Generators { get; } = new List<Generator>();
		public static List<_ragdoll> Ragdolls { get; } = new List<_ragdoll>();
		public static List<Room> Rooms { get; } = new List<Room>();
		public static List<Controllers.Camera> Cameras { get; } = new List<Controllers.Camera>();
		public static List<Tesla> Teslas { get; } = new List<Tesla>();
		public static List<_workStation> WorkStations { get; } = new List<_workStation>();
		public static List<Bot> Bots { get; } = new List<Bot>();
		public static List<Pickup> Pickups
		{
			get
			{
				List<Pickup> pickups = new List<Pickup>();
				foreach (ItemPickupBase itemPickupBase in Object.FindObjectsOfType<ItemPickupBase>())
				{
					if (Pickup.Get(itemPickupBase) is Pickup pickup)
						pickups.Add(pickup);
				}
				return pickups;
			}
		}
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
			set => Object.FindObjectOfType<LureSubjectContainer>().SetState(FemurBreakerState, value);
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
		public static MapBroadcast Broadcast(string message, ushort duration, bool instant = false)
		{
			var bc = new MapBroadcast(message, duration, instant);
			return bc;
		}
		public static void ClearBroadcasts() => Server.Host.Broadcasts.Clear();
		public static Vector3 GetRandomSpawnPoint(RoleType roleType)
		{
			GameObject randomPosition = Object.FindObjectOfType<SpawnpointManager>().GetRandomPosition(roleType);
			return randomPosition == null ? Vector3.zero : randomPosition.transform.position;
		}
		private static readonly RaycastHit[] CachedRaycast = new RaycastHit[1];
		public static Room FindRoom(GameObject objectInRoom)
		{
			var rooms = Rooms;
			Room room = null;
			const string playerTag = "Player";
			if (!objectInRoom.CompareTag(playerTag)) room = objectInRoom.GetComponentInParent<Room>();
			else
			{
				var ply = Player.Get(objectInRoom);
				if (ply.Role == RoleType.Scp079) room = FindRoom(ply.Scp079Controller.Camera079.gameObject);
			}
			if (room == null)
			{
				Ray ray = new Ray(objectInRoom.transform.position, Vector3.down);
				if (Physics.RaycastNonAlloc(ray, CachedRaycast, 10, 1 << 0, QueryTriggerInteraction.Ignore) == 1)
					room = CachedRaycast[0].collider.gameObject.GetComponentInParent<Room>();
			}
			if (room == null && rooms.Count != 0)
				room = rooms[rooms.Count - 1];
			return room;
		}
		public static void SpawnGrenade(string grenadeType, Vector3 position)
		{
			GameObject grenade = Object.Instantiate(NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name == grenadeType));
			grenade.transform.position = position;
			NetworkServer.Spawn(grenade);
		}/*
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
		}*/
		public static void ContainSCP106(Player executor) => PlayerManager.localPlayer.GetComponent<PlayerInteract>().UserCode_RpcContain106(executor.GameObject);
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
			NineTailedFoxAnnouncer.AnnounceScpTermination(component2.Classes.SafeGet(rt), new PlayerStats.HitInfo(-1f, killer?.Nickname ?? "", DamageTypes.None, killer?.Id ?? (-1), false), "");
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
					foreach (Door door in Doors) Object.Destroy(door.GameObject);
					break;
				case RemovableObject.Generators:
					foreach (Generator generator in Generators) Object.Destroy(generator.GameObject);
					break;
				case RemovableObject.Lockers:
					foreach (_locker locker in Lockers) Object.Destroy(locker.GameObject);
					break;
				case RemovableObject.Teslagates:
					foreach (Tesla teslaGate in Teslas) Object.Destroy(teslaGate.GameObject);
					break;
				case RemovableObject.Windows:
					foreach (BreakableWindow window in Object.FindObjectsOfType<BreakableWindow>()) Object.Destroy(window.gameObject);
					break;
				case RemovableObject.Workstations:
					foreach (_workStation workStation in WorkStations) Object.Destroy(workStation.GameObject);
					break;
				case RemovableObject.Rooms:
					foreach (NetworkIdentity netId in Object.FindObjectsOfType<NetworkIdentity>()) if (netId.name.Contains("All")) Object.Destroy(netId);
					break;
			}
		}
		[Obsolete("Use Bot.Create")]
		public static GameObject SpawnBot(RoleType role, string name, float health, Vector3 position, Vector3 rotation, Vector3 scale) => null;
		[Obsolete("Use Bot.Create")]
		public static GameObject SpawnPlayer(RoleType role, string name, string userSteamID, Vector3 position, Vector3 rotation, Vector3 scale) => null;
		internal static void AddObjects()
		{
			Cassies = new CassieList();
			foreach (var room in RoomIdentifier.AllRoomIdentifiers)
			{
				var _room = new Room(room);
				Rooms.Add(_room);
				Cameras.AddRange(_room.Cameras);
			}
			foreach (var tesla in Server.GetObjectsOf<TeslaGate>()) Teslas.Add(new Tesla(tesla));
			foreach (var station in WorkstationController.AllWorkstations) WorkStations.Add(new _workStation(station));
			foreach (var door in Server.GetObjectsOf<DoorVariant>()) Doors.Add(new Door(door));
			foreach (var pair in Scp079Interactable.InteractablesByRoomId)
			{
				foreach (var interactable in pair.Value)
				{
					try
					{
						var room = Rooms.FirstOrDefault(x => x.Id == pair.Key);
						var door = interactable.GetComponentInParent<DoorVariant>();
						if (room == null || door == null) continue;
						var sdoor = door.GetDoor();
						sdoor.Rooms.Add(room);
						room.Doors.Add(sdoor);
					}
					catch { }
				}
			}
			Controllers.Scp914.Scp914Controller = Object.FindObjectOfType<Scp914Controller>();
			Item.BaseToItem.Clear();
			Pickup.BaseToItem.Clear();
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
			Patches.Events.player.DropItem.Data.Clear();
		}
	}
}