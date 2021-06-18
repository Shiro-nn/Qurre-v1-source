using Mirror;
using static QurreModLoader.umm;
namespace Qurre.API.Controllers
{
    public class Scp079
    {
        internal Scp079(Player player) => this.player = player;
        private readonly Player player;
        private Scp079PlayerScript script => player.ClassManager.Scp079;
        public bool Is079 => player.Role == RoleType.Scp079;
        public Scp079PlayerScript.Ability079[] Abilities { get => script.abilities; set => script.abilities = value; }
        public byte Lvl { get => (byte)(script.Lvl + 1); set => script.NetworkcurLvl = (byte)(value - 1); }
        public Scp079PlayerScript.Level079[] Lvls { get => script.levels; set => script.levels = value; }
        public string Speaker { get => script.Speaker; set => script.Scp079_speaker(value); }
        public float Exp { get => script.Exp; set => script.NetworkcurExp = value; }
        public float Energy { get => script.Mana; set => script.NetworkcurMana = value; }
        public float MaxEnergy { get => script.maxMana; set => script.NetworkmaxMana = value; }
        public Camera079 Camera { get => script.currentCamera; set => script?.CallRpcSwitchCamera(value.cameraId, false); }
        public static Camera079[] Camers => Scp079PlayerScript.allCameras;
        public SyncListUInt LockedDoors { get => script.lockedDoors; set => script.lockedDoors = value; }
        public void GiveExp(float amount) => script.AddExperience(amount);
        public void ForceLevel(byte levelToForce, bool notifiyUser) => script.ForceLevel(levelToForce, notifiyUser);
        public void AddLockedDoor(uint doorID) { if (!script.lockedDoors.Contains(doorID)) script.lockedDoors.Add(doorID); }
        public void UnlockDoor(uint doorID) { if (script.lockedDoors.Contains(doorID)) script.lockedDoors.Remove(doorID); }
        public void UnlockDoors() => script.CmdResetDoors();
        public static int ActivatedGenerators => Generator079.mainGenerator.totalVoltage;
    }
}