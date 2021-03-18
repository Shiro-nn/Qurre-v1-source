using Interactables.Interobjects.DoorUtils;
using Qurre.API.Controllers;
using Qurre.API.Objects;
using System.Linq;
using _door = Qurre.API.Controllers.Door;
using _lift = Qurre.API.Controllers.Lift;
using _workStation = Qurre.API.Controllers.WorkStation;
namespace Qurre.API
{
    public static class Extensions
	{
		public static DoorVariant DoorVariant { get; internal set; }
		public static Room GetRoom(RoomType type) => Map.Rooms.FirstOrDefault(x => x.Type == type);
		public static _door GetDoor(DoorType type) => Map.Doors.FirstOrDefault(x => x.Type == type);
		public static _lift GetLift(LiftType type) => Map.Lifts.FirstOrDefault(x => x.Type == type);
		public static _door GetDoor(this DoorVariant door) => Map.Doors.FirstOrDefault(x => x.GameObject == door.gameObject);
		public static Generator GetGenerator(this Generator079 generator079) => Map.Generators.FirstOrDefault(x => x.GameObject == generator079.gameObject);
		public static Tesla GetTesla(this TeslaGate teslaGate) => Map.Teslas.FirstOrDefault(x => x.GameObject == teslaGate.gameObject);
		public static _lift GetLift(this Lift lift) => Map.Lifts.FirstOrDefault(x => x.GameObject == lift.gameObject);
		public static _workStation GetWorkStation(this WorkStation station) => Map.WorkStations.FirstOrDefault(x => x.GameObject == station.gameObject);
	}
}