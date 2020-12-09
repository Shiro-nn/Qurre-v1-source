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
		public static int GetLvl(this ReferenceHub player) => player.scp079PlayerScript.Lvl;
		public static void SetLvl(this ReferenceHub player, byte level, bool notifyUser = true)
		{
			if (player.scp079PlayerScript.Lvl == level)
				return;

			player.scp079PlayerScript.NetworkcurLvl = level;
		}
		public static float GetMana(this ReferenceHub player) => player.scp079PlayerScript.Mana;
		public static void SetMana(this ReferenceHub player, float amount) => player.scp079PlayerScript.NetworkcurMana = amount;
		public static void AddMana(this ReferenceHub player, float amount) => player.scp079PlayerScript.NetworkcurMana += amount;
		public static float GetMaxMana(this ReferenceHub player) => player.scp079PlayerScript.NetworkmaxMana;
		public static void SetMaxMana(this ReferenceHub player, float amount)
		{
			player.scp079PlayerScript.NetworkmaxMana = amount;
			player.scp079PlayerScript.levels[player.GetLvl()].maxMana = amount;
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
		public static string GetSpeaker(this ReferenceHub player) => player.scp079PlayerScript.Speaker;
		public static Camera079 GetCamera(this ReferenceHub player) => player.scp079PlayerScript.currentCamera;
		public static Camera079[] GetCamers() => Scp079PlayerScript.allCameras;
		public static void SetCamera(this ReferenceHub player, Camera079 camera, bool lookAtRotation = false) => player.SetCamera(camera.cameraId, lookAtRotation);
		public static void SetCamera(this ReferenceHub player, ushort cameraId, bool lookAtRotation = false) => player.scp079PlayerScript.CallRpcSwitchCamera(cameraId, lookAtRotation);
		public static Scp079PlayerScript.Level079[] GetLevels(this ReferenceHub player) => player.scp079PlayerScript.levels;
		public static void SetLevels(this ReferenceHub player, Scp079PlayerScript.Level079[] levels) => player.scp079PlayerScript.levels = levels;
		public static Scp079PlayerScript.Ability079[] GetAbilities(this ReferenceHub player) => player.scp079PlayerScript.abilities;
		public static void SetAbilities(this ReferenceHub player, Scp079PlayerScript.Ability079[] abilities) => player.scp079PlayerScript.abilities = abilities;
	}
}
