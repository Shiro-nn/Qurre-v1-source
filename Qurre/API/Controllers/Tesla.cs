using Mirror;
using System.Collections.Generic;
using UnityEngine;
namespace Qurre.API.Controllers
{
    public class Tesla
    {
        private readonly TeslaGate Gate;
        internal Tesla(TeslaGate gate) => Gate = gate;
        public GameObject GameObject => Gate.gameObject;
        public Transform Transform => GameObject.transform;
        private string name;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name)) return GameObject.name;
                return name;
            }
            set => name = value;
        }
        public Vector3 Position => Transform.position;
        public Quaternion Rotation => Transform.localRotation;
        public Vector3 Scale
        {
            get => Transform.localScale;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                Transform.localScale = value;
                SizeOfKiller = value;
                NetworkServer.Spawn(GameObject);
            }
        }
        public Vector3 SizeOfKiller { get => Gate.sizeOfKiller; set => Gate.sizeOfKiller = value; }
        public bool InProgress { get => Gate.InProgress; set => Gate.InProgress = value; }
        public float SizeOfTrigger { get => Gate.sizeOfTrigger; set => Gate.sizeOfTrigger = value; }
        public bool Enable { get; set; } = true;
        public bool Allow079Interact { get; set; } = true;
        public readonly List<RoleType> ImmunityRoles = new();
        public readonly List<Player> ImmunityPlayers = new();
        public void Trigger(bool instant = false)
        {
            if (instant) Gate.RpcInstantBurst();
            else Gate.RpcPlayAnimation();
        }
        public void Destroy() => Object.Destroy(Gate.gameObject);
    }
}