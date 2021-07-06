using Interactables.Interobjects.DoorUtils;
using Qurre.API.Controllers;
using Qurre.API.Objects;
using System.Linq;
using _door = Qurre.API.Controllers.Door;
using _lift = Qurre.API.Controllers.Lift;
using _locker = Qurre.API.Controllers.Locker;
using _workStation = Qurre.API.Controllers.WorkStation;
namespace Qurre.API
{
	public static class Extensions
	{
		public static WeaponType GetWeaponType(this ItemType item)
		{
			if (item == ItemType.GunCOM15) return WeaponType.Com15;
			if (item == ItemType.GunE11SR) return WeaponType.Epsilon11;
			if (item == ItemType.GunLogicer) return WeaponType.Logicer;
			if (item == ItemType.GunMP7) return WeaponType.MP7;
			if (item == ItemType.GunProject90) return WeaponType.Project90;
			if (item == ItemType.GunUSP) return WeaponType.USP;
			if (item == ItemType.MicroHID) return WeaponType.MicroHID;
			return WeaponType.None;
		}
		public static DoorVariant DoorPrefabLCZ { get; internal set; }
		public static DoorVariant DoorPrefabHCZ { get; internal set; }
		public static DoorVariant DoorPrefabEZ { get; internal set; }
		public static DoorVariant GetDoorPrefab(DoorPrefabs prefab)
		{
			if (prefab == DoorPrefabs.DoorLCZ) return DoorPrefabLCZ;
			else if (prefab == DoorPrefabs.DoorHCZ) return DoorPrefabHCZ;
			else return DoorPrefabEZ;
		}
		public static Room GetRoom(RoomType type) => Map.Rooms.FirstOrDefault(x => x.Type == type);
		public static _door GetDoor(DoorType type) => Map.Doors.FirstOrDefault(x => x.Type == type);
		public static _lift GetLift(LiftType type) => Map.Lifts.FirstOrDefault(x => x.Type == type);
		public static _door GetDoor(this DoorVariant door) => Map.Doors.FirstOrDefault(x => x.GameObject == door.gameObject);
		public static Generator GetGenerator(this Generator079 generator079) => Map.Generators.FirstOrDefault(x => x.GameObject == generator079.gameObject);
		public static Tesla GetTesla(this TeslaGate teslaGate) => Map.Teslas.FirstOrDefault(x => x.GameObject == teslaGate.gameObject);
		public static _lift GetLift(this Lift lift) => Map.Lifts.FirstOrDefault(x => x.GameObject == lift.gameObject);
		public static _locker GetLocker(this Locker locker) => Map.Lockers.FirstOrDefault(x => x.Transform == locker.gameObject);
		public static _workStation GetWorkStation(this WorkStation station) => Map.WorkStations.FirstOrDefault(x => x.GameObject == station.gameObject);
	}
}