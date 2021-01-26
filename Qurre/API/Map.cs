using Qurre.API.Objects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using LightContainmentZoneDecontamination;
using Mirror;
using RemoteAdmin;
using Scp914;
using Respawning;
using Interactables.Interobjects.DoorUtils;

namespace Qurre.API
{
	public static class Map
	{
        private static int roundtime = 0;
        private static ReferenceHub host;
		private static Inventory hinv;
		private static Broadcast bc;
		private static DecontaminationController dc;
		private static List<Room> rms = new List<Room>();
		private static List<DoorVariant> drs = new List<DoorVariant>();
		private static List<Lift> lfs = new List<Lift>();
		private static List<TeslaGate> tgs = new List<TeslaGate>();
		public static ReferenceHub Host
		{
			get
			{
				if (host == null)
					host = Player.Get(PlayerManager.localPlayer);
				return host;
			}
		}
		public static Inventory InventoryHost
		{
			get
			{
				if (hinv == null)
					hinv = Player.Get(PlayerManager.localPlayer).inventory;
				return hinv;
			}
		}
		public static bool FriendlyFire
		{
			get
			{
				return ServerConsole.FriendlyFire;
			}
			set
			{
				ServerConsole.FriendlyFire = value;
				foreach (ReferenceHub hub in Player.GetHubs())
					hub.SetFriendlyFire(value);
			}
		}
		internal static Broadcast BroadcastComponent
		{
			get
			{
				if (bc == null)
					bc = PlayerManager.localPlayer.GetComponent<Broadcast>();
				return bc;
			}
		}
		internal static DecontaminationController DecontaminationLCZ
		{
			get
			{
				if (dc == null)
					dc = PlayerManager.localPlayer.GetComponent<DecontaminationController>();

				return dc;
			}
		}
		public static List<Room> Rooms
		{
			get
			{
				if (rms == null || rms.Count == 0)
					rms = Object.FindObjectsOfType<Transform>().Where(transform => transform.CompareTag("Room")).Select(obj => new Room { Name = obj.name, Position = obj.position, Transform = obj }).ToList();
				return rms;
			}
		}
		public static List<DoorVariant> Doors
		{
			get
			{
				if (drs == null || drs.Count == 0)
					drs = Object.FindObjectsOfType<DoorVariant>().ToList();
				return drs;
			}
		}
		public static List<Lift> Lifts
		{
			get
			{
				if (lfs == null || lfs.Count == 0)
					lfs = Object.FindObjectsOfType<Lift>().ToList();
				return lfs;
			}
		}
		public static List<TeslaGate> TeslaGates
		{
			get
			{
				if (tgs == null || tgs.Count == 0)
					tgs = Object.FindObjectsOfType<TeslaGate>().ToList();

				return tgs;
			}
		}
		public static Pickup ItemSpawn(ItemType itemType, float durability, Vector3 position, Quaternion rotation = default, int sight = 0, int barrel = 0, int other = 0)
			=> InventoryHost.SetPickup(itemType, durability, position, rotation, sight, barrel, other);
		public static void Broadcast(string message, ushort duration, Broadcast.BroadcastFlags flag = 0)
			=> BroadcastComponent.RpcAddElement(message, duration, flag);
		public static void ClearBroadcasts() => BroadcastComponent.RpcClearElements();
		public static Vector3 GetRandomSpawnPoint(RoleType roleType)
		{
			GameObject randomPosition = Object.FindObjectOfType<SpawnpointManager>().GetRandomPosition(roleType);

			return randomPosition == null ? Vector3.zero : randomPosition.transform.position;
		}
		public static IEnumerable<Room> GetRooms() => Rooms;
		public static IEnumerable<ReferenceHub> HubsInRoom(this Room room) => ReferenceHub.GetAllHubs().Values.Where(player => !player.IsHost() && player.CurrentRoom().Name == room.Name);
		public static int ActivatedGenerators => Generator079.mainGenerator.totalVoltage;

        public static int Roundtime { get => roundtime; set => roundtime = value; }
        public static ReferenceHub Host1 { get => host; set => host = value; }
        public static Inventory Hinv { get => hinv; set => hinv = value; }
        public static Broadcast Bc { get => bc; set => bc = value; }
        public static DecontaminationController Dc { get => dc; set => dc = value; }
        public static List<Room> Rms { get => rms; set => rms = value; }
        public static List<DoorVariant> Drs { get => drs; set => drs = value; }
        public static List<Lift> Lfs { get => lfs; set => lfs = value; }
        public static List<TeslaGate> Tgs { get => tgs; set => tgs = value; }

        public static void TurnOffLights(float duration, bool onlyHeavy = false) => Generator079.Generators[0].ServerOvercharge(duration, onlyHeavy);
		public static void SpawnRagdoll(RoleType role, string name, Vector3 position, Quaternion rotation, string ownerID, string ownerNickname, int playerID) => PlayerManager.localPlayer.GetComponent<RagdollManager>().SpawnRagdoll(new Vector3(position.x, position.y, position.z), rotation, new Vector3(0, 0, 0), (int)role, new PlayerStats.HitInfo(), false, ownerID, ownerNickname, playerID);
		public static GameObject SpawnBrokenRagdoll(RoleType role, Vector3 position, Vector3 rotation, Vector3 scale)
		{
			GameObject ragdoll = Object.Instantiate(NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name == "Ragdoll_" + ((int)role).ToString()));
			ragdoll.transform.position = position;
			ragdoll.transform.eulerAngles = rotation;
			ragdoll.transform.localScale = scale;
			NetworkServer.Spawn(ragdoll);
			return ragdoll;
		}
		public static void SpawnGrenade(string grenadeType, Vector3 position)
		{
			GameObject grenade = Object.Instantiate(NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name == grenadeType));
			grenade.transform.position = position;
			NetworkServer.Spawn(grenade);
		}
		/*public static GameObject SpawnWorkstation(bool isTabletConnected, Vector3 position, Vector3 rotation, Vector3 scale)
		{
			GameObject bench = Object.Instantiate(NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name == "Work Station"));
			Offset offset = new Offset();
			offset.position = position;
			offset.rotation = rotation;
			offset.scale = Vector3.one;
			bench.transform.localScale = scale;
			bench.GetComponent<WorkStation>().Networkposition = offset;
			bench.GetComponent<WorkStation>().NetworkisTabletConnected = isTabletConnected;
			NetworkServer.Spawn(bench);
			return bench;
		}*/
		public static GameObject SpawnItem(ItemType itemType, Vector3 position, Vector3 rotation, Vector3 scale)
		{
			Pickup yesnt = PlayerManager.localPlayer.GetComponent<Inventory>().SetPickup((ItemType)itemType, -4.656647E+11f, position, Quaternion.identity, 0, 0, 0);

			GameObject gameObject = yesnt.gameObject;
			gameObject.transform.localScale = scale;
			gameObject.transform.eulerAngles = rotation;

			NetworkServer.UnSpawn(gameObject);
			NetworkServer.Spawn(yesnt.gameObject);
			return yesnt.gameObject;
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
		public static void CallCICar() => RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.Selection, SpawnableTeamType.ChaosInsurgency);
		public static void CallMTFHelicopter() => RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.Selection, SpawnableTeamType.NineTailedFox);
		public static void PlayCIEntranceMusic() => RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.UponRespawn, SpawnableTeamType.ChaosInsurgency);
		public static void ContainSCP106(ReferenceHub executor) => PlayerManager.localPlayer.GetComponent<PlayerInteract>().CallRpcContain106(executor.gameObject);
		public static void ShakeScreen(float times) => ExplosionCameraShake.singleton.Shake(times);
		public static void SetFemurBreakerState(bool enabled) => Object.FindObjectOfType<LureSubjectContainer>().SetState(enabled);
		public static void RemoveTeslaGates() { foreach (TeslaGate teslaGate in Object.FindObjectsOfType<TeslaGate>()) { Object.Destroy(teslaGate.gameObject); } }
		public static void RemoveDoors() { foreach (DoorVariant dr in drs) { Object.Destroy(dr.gameObject); } }
		public static void SetLiftMovingSpeed(float Speed) { foreach (Lift lft in lfs) { lft.movingSpeed = Speed; } }

		public static void SetIntercomSpeaker(ReferenceHub player)
		{
			if (player != null)
			{
				GameObject gameObject = player.gameObject;
				gameObject.GetComponent<CharacterClassManager>().IntercomMuted = false;
				Intercom.host.RequestTransmission(gameObject);
				return;
			}
			foreach (CharacterClassManager ccm in Object.FindObjectsOfType<CharacterClassManager>())
			{
				if (ccm.IntercomMuted)
				{
					ccm.IntercomMuted = false;
				}
			}
		}
		public static int GetMaxPlayers()
		{
			CustomNetworkManager nm = new CustomNetworkManager();
			return nm.maxConnections;
		}
		public static void SetMaxPlayers(int amount)
		{
			CustomNetworkManager nm = new CustomNetworkManager();
			nm.maxConnections = amount;
		}
		public static void DisableDecontamination(bool value) => DecontaminationController.Singleton.disableDecontamination = value;
		public static void PlayIntercomSound(bool start) => PlayerManager.localPlayer.GetComponent<Intercom>().RpcPlaySound(start, 0);
		public static void PlaceBlood(Vector3 position, int type, float size) => PlayerManager.localPlayer.GetComponent<CharacterClassManager>().RpcPlaceBlood(position, type, size);
		public static void PlayAmbientSound(int id) => PlayerManager.localPlayer.GetComponent<AmbientSoundPlayer>().RpcPlaySound(id);

		public static void PlayChaosSound()
		{
			RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.UponRespawn, SpawnableTeamType.ChaosInsurgency);
		}
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
							Log.Debug("Map: Tryed to use UNDEFINED zone");
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
				if (component != null && component.type == Scp079Interactable.InteractableType.LightController && component.currentZonesAndRooms.Count != 0 && component.currentZonesAndRooms[0].currentRoom.Contains(room))
				{
					flickerableLightController.ServerSetLightIntensity(intensive);
				}
			}
		}

		public static void Shake()
		{
			AlphaWarheadController host = AlphaWarheadController.Host;
			if (host != null)
			{
				host.CallRpcShake(true);
			}
		}
		public static void ActivateSCP914()
		{
			Scp914Machine.singleton.RpcActivate(NetworkTime.time);
		}
		public static void ActivateSCP914(float time)
		{
			Scp914Machine.singleton.RpcActivate(time);
		}
	}
}
