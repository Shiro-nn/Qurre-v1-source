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
using _ragdoll = Qurre.API.Controllers.Ragdoll;
using _workStation = Qurre.API.Controllers.WorkStation;
using Qurre.API.Controllers;
using static QurreModLoader.umm;
namespace Qurre.API
{
	public class Map
	{
		public static int roundtime = 0;
		public static Alpha Alpha { get; private set; } = new Alpha();
		public static Decontamination DecontaminationLCZ { get; private set; } = new Decontamination();
		public static Heavy Heavy { get; private set; } = new Heavy();
		public static Controllers.Scp914 Scp914 { get; private set; } = new Controllers.Scp914();
		public static List<_door> Doors { get; } = new List<_door>();
		public static List<_lift> Lifts { get; } = new List<_lift>();
		public static List<Generator> Generators { get; } = new List<Generator>();
		public static List<_ragdoll> Ragdolls { get; } = new List<_ragdoll>();
		public static List<Room> Rooms { get; } = new List<Room>();
		public static List<Tesla> Teslas { get; } = new List<Tesla>();
		public static List<_workStation> WorkStations { get; } = new List<_workStation>();
		public static Controllers.Broadcast Broadcast(string message, ushort duration, bool instant = false)
		{
			var bc = new Controllers.Broadcast(Server.Host, message, duration);
			Server.Host.Broadcasts.Add(bc, instant);
			return bc;
		}
		public static void ClearBroadcasts() => Server.Host.Broadcasts.Clear();
		public static Vector3 GetRandomSpawnPoint(RoleType roleType)
		{
			GameObject randomPosition = Object.FindObjectOfType<SpawnpointManager>().GetRandomPosition(roleType);
			return randomPosition == null ? Vector3.zero : randomPosition.transform.position;
		}
		public static void TurnOffLights(float duration, bool onlyHeavy = false) => Generator079.Generators[0].ServerOvercharge(duration, onlyHeavy);
		public static void SpawnGrenade(string grenadeType, Vector3 position)
		{
			GameObject grenade = Object.Instantiate(NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name == grenadeType));
			grenade.transform.position = position;
			NetworkServer.Spawn(grenade);
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
				if (health == -1)
					component.GodMode = true;

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
		public static void SetFemurBreakerState(bool enabled) => Object.FindObjectOfType<LureSubjectContainer>().SetState(enabled);
		public static void RemoveTeslaGates() { foreach (TeslaGate teslaGate in Object.FindObjectsOfType<TeslaGate>()) { Object.Destroy(teslaGate.gameObject); } }
		public static void RemoveDoors() { foreach (_door dr in Doors) { Object.Destroy(dr.GameObject); } }
		public static void SetElevatorsMovingSpeed(float newSpeed) { foreach (_lift lft in Lifts) { lft.MovingSpeed = newSpeed; } }
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
		public static void SetLightsIntensivity(float intensive)
		{
			foreach (FlickerableLightController flickerableLightController in Object.FindObjectsOfType<FlickerableLightController>())
			{
				Scp079Interactable component = flickerableLightController.GetComponent<Scp079Interactable>();
				if (component != null && component.type == Scp079Interactable.InteractableType.LightController)
				{
					flickerableLightController.ServerSetLightIntensity(intensive);
				}
			}
		}
		public static void SetLightsIntensivity(float intensive, ZoneType zone)
		{
			foreach (FlickerableLightController flickerableLightController in Object.FindObjectsOfType<global::FlickerableLightController>())
			{
				Scp079Interactable component = flickerableLightController.GetComponent<Scp079Interactable>();
				if (component != null && component.type == Scp079Interactable.InteractableType.LightController && component.currentZonesAndRooms.Count != 0)
				{
					string b;
					switch (zone)
					{
						case ZoneType.LightContainment:
							b = "LightRooms";
							break;
						case ZoneType.HeavyContainment:
							b = "HeavyRooms";
							break;
						case ZoneType.Entrance:
							b = "EntranceRooms";
							break;
						case ZoneType.Surface:
							if (component.optionalObject.transform.position.y > 900f)
							{
								flickerableLightController.ServerSetLightIntensity(intensive);
							}
							return;
						default:
							Log.Debug("smail, UNDEFINED zone");
							return;
					}
					if (component.currentZonesAndRooms[0].currentZone == b)
					{
						flickerableLightController.ServerSetLightIntensity(intensive);
					}
				}
			}
		}
		public static void SetLightsIntensivity(float intensive, string room)
		{
			foreach (FlickerableLightController flickerableLightController in Object.FindObjectsOfType<FlickerableLightController>())
			{
				Scp079Interactable component = flickerableLightController.GetComponent<Scp079Interactable>();
				if (component != null && component.type == Scp079Interactable.InteractableType.LightController &&
					component.currentZonesAndRooms.Count != 0 && component.currentZonesAndRooms[0].currentRoom.Contains(room))
				{
					flickerableLightController.ServerSetLightIntensity(intensive);
				}
			}
		}
		internal static void AddObjects()
		{
			DecontaminationLCZ = new Decontamination();
			Alpha = new Alpha();
			Heavy = new Heavy();
			Scp914 = new Controllers.Scp914();
			foreach (var tesla in Server.GetObjectsOf<TeslaGate>())
				Teslas.Add(new Tesla(tesla));
			foreach (var room in Server.GetObjectsOf<Transform>().Where(x => x.CompareTag("Room") || x.name == "Root_*&*Outside Cams" || x.name == "PocketWorld"))
				Rooms.Add(new Room(room.gameObject));
			foreach (var station in Server.GetObjectsOf<global::WorkStation>())
				WorkStations.Add(new _workStation(station));
			foreach (var door in Server.GetObjectsOf<DoorVariant>())
				Doors.Add(new _door(door));
			foreach (var interactable in Interface079.singleton.allInteractables)
			{
				foreach (var zoneroom in interactable.currentZonesAndRooms)
				{
					try
					{
						var room = Rooms.FirstOrDefault(x => x.Name == zoneroom.currentRoom);
						var door = interactable.GetComponentInParent<Interactables.Interobjects.DoorUtils.DoorVariant>();
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
			Rooms.Clear();
			Generators.Clear();
			WorkStations.Clear();
			Ragdolls.Clear();
			Heavy.Recontained079 = false;
		}
	}
}