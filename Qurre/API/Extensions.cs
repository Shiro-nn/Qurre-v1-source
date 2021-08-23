using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Pickups;
using MapGeneration;
using MapGeneration.Distributors;
using Qurre.API.Controllers;
using Qurre.API.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using _lift = Qurre.API.Controllers.Lift;
using _locker = Qurre.API.Controllers.Locker;
using _workStation = Qurre.API.Controllers.WorkStation;
namespace Qurre.API
{
	public static class Extensions
	{
		public static DoorVariant DoorPrefabLCZ { get; internal set; }
		public static DoorVariant DoorPrefabHCZ { get; internal set; }
		public static DoorVariant DoorPrefabEZ { get; internal set; }
		public static DoorVariant GetDoorPrefab(DoorPrefabs prefab)
		{
			if (prefab == DoorPrefabs.DoorLCZ) return DoorPrefabLCZ;
			else if (prefab == DoorPrefabs.DoorHCZ) return DoorPrefabHCZ;
			else return DoorPrefabEZ;
		}
		public static Room GetRoom(RoomName type) => Map.Rooms.FirstOrDefault(x => x.Type == type);
		public static Door GetDoor(DoorType type) => Map.Doors.FirstOrDefault(x => x.Type == type);
		public static _lift GetLift(LiftType type) => Map.Lifts.FirstOrDefault(x => x.Type == type);
		public static Door GetDoor(this DoorVariant door) => Map.Doors.FirstOrDefault(x => x.GameObject == door.gameObject);
		public static Generator GetGenerator(this Scp079Generator generator079) => Map.Generators.FirstOrDefault(x => x.GameObject == generator079.gameObject);
		public static Tesla GetTesla(this TeslaGate teslaGate) => Map.Teslas.FirstOrDefault(x => x.GameObject == teslaGate.gameObject);
		public static _lift GetLift(this Lift lift) => Map.Lifts.FirstOrDefault(x => x.GameObject == lift.gameObject);
		public static _locker GetLocker(this MapGeneration.Distributors.Locker locker) => Map.Lockers.FirstOrDefault(x => x.Transform == locker.gameObject);
		public static _workStation GetWorkStation(this WorkstationController station) => Map.WorkStations.FirstOrDefault(x => x.GameObject == station.gameObject);
		public static ItemType GetItemType(this AmmoType type)
		{
            return type switch
            {
                AmmoType.Ammo556 => ItemType.Ammo556x45,
                AmmoType.Ammo762 => ItemType.Ammo762x39,
                AmmoType.Ammo9 => ItemType.Ammo9x19,
                AmmoType.Ammo12Gauge => ItemType.Ammo12gauge,
                AmmoType.Ammo44Cal => ItemType.Ammo44cal,
                _ => ItemType.None,
            };
		}
		public static AmmoType GetAmmoType(this ItemType type)
		{
			return type switch
			{
				ItemType.Ammo9x19 => AmmoType.Ammo9,
				ItemType.Ammo556x45 => AmmoType.Ammo556,
				ItemType.Ammo762x39 => AmmoType.Ammo762,
				ItemType.Ammo12gauge => AmmoType.Ammo12Gauge,
				ItemType.Ammo44cal => AmmoType.Ammo44Cal,
				_ => AmmoType.None,
			};
		}
		public static System.Random Random { get; } = new System.Random();
		public static void Shuffle<T>(this IList<T> list)
		{
			RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
			int n = list.Count;
			while (n > 1)
			{
				byte[] box = new byte[1];
				do provider.GetBytes(box);
				while (!(box[0] < n * (byte.MaxValue / n)));
				int k = (box[0] % n);
				n--;
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}
}