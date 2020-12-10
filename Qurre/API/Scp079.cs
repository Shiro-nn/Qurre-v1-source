using Mirror;
namespace Qurre.API
{
	public static class Scp079
	{
		public static float GetExp(this ReferenceHub player) => player.scp079PlayerScript.Exp;
		public static void SetExp(this ReferenceHub player, float amount)
		{
			player.scp079PlayerScript.NetworkcurExp = amount;
		}
		public static void AddExp(this ReferenceHub player, float amount) => player.scp079PlayerScript.AddExperience(amount);
		public static int Lvl(this ReferenceHub player) => player.scp079PlayerScript.Lvl;
		public static void Lvl(this ReferenceHub player, byte level)
		{
			if (player.scp079PlayerScript.Lvl == level)
				return;
			player.scp079PlayerScript.NetworkcurLvl = level;
		}
		public static float GetMana(this ReferenceHub player) => player.scp079PlayerScript.Mana;
		public static void SetMana(this ReferenceHub player, float amount) => player.scp079PlayerScript.NetworkcurMana = amount;
		public static void AddMana(this ReferenceHub player, float amount) => player.scp079PlayerScript.NetworkcurMana += amount;
		public static float MaxMana(this ReferenceHub player) => player.scp079PlayerScript.NetworkmaxMana;
		public static void MaxMana(this ReferenceHub player, float amount)
		{
			player.scp079PlayerScript.NetworkmaxMana = amount;
			player.scp079PlayerScript.levels[player.Lvl()].maxMana = amount;
		}
		public static SyncListString GetLockedDoors(this ReferenceHub player) => player.scp079PlayerScript.lockedDoors;
		public static void SetLockedDoors(this ReferenceHub player, SyncListString lockedDoors) => player.scp079PlayerScript.lockedDoors = lockedDoors;
		public static void AddLockedDoor(this ReferenceHub player, string doorName)
		{
			if (!player.scp079PlayerScript.lockedDoors.Contains(doorName))
				player.scp079PlayerScript.lockedDoors.Add(doorName);
		}
		public static void RemoveLockedDoor(this ReferenceHub player, string doorName)
		{
			if (player.scp079PlayerScript.lockedDoors.Contains(doorName))
				player.scp079PlayerScript.lockedDoors.Remove(doorName);
		}
		public static string Speaker(this ReferenceHub player) => player.scp079PlayerScript.Speaker;
		public static Camera079 Camera(this ReferenceHub player) => player.scp079PlayerScript.currentCamera;
		public static Camera079[] Camers() => Scp079PlayerScript.allCameras;
		public static void Camera(this ReferenceHub player, Camera079 camera, bool lookAtRotation = false) => player.Camera(camera.cameraId, lookAtRotation);
		public static void Camera(this ReferenceHub player, ushort cameraId, bool lookAtRotation = false) => player.scp079PlayerScript.CallRpcSwitchCamera(cameraId, lookAtRotation);
		public static Scp079PlayerScript.Level079[] Levels(this ReferenceHub player) => player.scp079PlayerScript.levels;
		public static void Levels(this ReferenceHub player, Scp079PlayerScript.Level079[] levels) => player.scp079PlayerScript.levels = levels;
		public static Scp079PlayerScript.Ability079[] Abilities(this ReferenceHub player) => player.scp079PlayerScript.abilities;
		public static void Abilities(this ReferenceHub player, Scp079PlayerScript.Ability079[] abilities) => player.scp079PlayerScript.abilities = abilities;
	}
}