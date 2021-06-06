using Qurre.API.Objects;
using UnityEngine;
using static QurreModLoader.umm;
using _lift = Lift;
namespace Qurre.API.Controllers
{
    public class Lift
    {
        internal Lift(_lift lift) => _lift = lift;
        private _lift _lift;
        public GameObject GameObject => _lift.gameObject;
        public _lift Elevator => _lift;
        public string Name => _lift.elevatorName;
        public Vector3 Position => GameObject.transform.position;
        public _lift.Status Status { get => _lift.status; set => _lift.SetStatus(value); }
        public bool Locked { get => _lift.LiftLock(); set => _lift.LiftLock(value); }
        public float MaxDistance { get => _lift.maxDistance; set => _lift.maxDistance = value; }
        public float MovingSpeed { get => _lift.movingSpeed; set => _lift.movingSpeed = value; }
        public bool Operative { get => _lift.operative; set => _lift.operative = value; }
        public LiftType Type
        {
            get
            {
                switch (Name)
                {
                    case "GateB": return LiftType.GateB;
                    case "GateA": return LiftType.GateA;
                    case "SCP-049": return LiftType.Scp049;
                    case "ElA": return LiftType.ElALeft;
                    case "ElA2": return LiftType.ElARight;
                    case "ElB": return LiftType.ElBLeft;
                    case "ElB2": return LiftType.ElBRight;
                    default: return LiftType.Unknown;
                }
            }
        }
        public void Use() => _lift.UseLift();
    }
}