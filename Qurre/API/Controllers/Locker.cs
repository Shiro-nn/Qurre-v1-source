using Qurre.API.Objects;
using System.Collections.Generic;
using UnityEngine;
namespace Qurre.API.Controllers
{
    public class Locker
    {
        internal Locker(global::Locker locker) => _locker = locker;
        private global::Locker _locker;
        public GameObject GameObject => _locker.gameObject.gameObject;
        public Transform Transform => _locker.gameObject;
        public global::Locker GlobalLocker => _locker;
        public string Name => _locker.name;
        public Vector3 Position => Transform.position;
        public Quaternion Rotation => Transform.localRotation;
        public Vector3 Scale => Transform.localScale;
        public bool SpawnOnOpen { get => _locker.SpawnOnOpen; set => _locker.SpawnOnOpen = value; }
        public bool TriggeredByDoor { get => _locker.TriggeredByDoor; set => _locker.TriggeredByDoor = value; }
        public List<global::Locker.ItemToSpawn> ItemsToSpawn { get => _locker._itemsToSpawn; set => _locker._itemsToSpawn = value; }
        public List<Pickup> AssignedPickups { get => _locker._assignedPickups; set => _locker._assignedPickups = value; }
        public Vector3 SortingTorque { get => _locker.sortingTorque; set => _locker.sortingTorque = value; }
        public bool СhambersProcessed { get => _locker._chambersProcessed; set => _locker._chambersProcessed = value; }
        public int СhanceOfSpawn { get => _locker.chanceOfSpawn; set => _locker.chanceOfSpawn = value; }
        public LockerChamber[] Сhambers { get => _locker.chambers; set => _locker.chambers = value; }
        public bool AnyVirtual { get => _locker.AnyVirtual; set => _locker.AnyVirtual = value; }
        public bool Spawned { get => _locker.Spawned; set => _locker.Spawned = value; }
        public LockerType Type
        {
            get
            {
                return Name switch
                {
                    "Generator SCP-079" => LockerType.Generator,
                    "First Aid Kit" => LockerType.FirstAidKit,
                    "Misc Locker" => LockerType.MiscLocker,
                    "Glocker A" => LockerType.GlockerA,
                    "Glocker B" => LockerType.GlockerB,
                    "Pedestal" => LockerType.Pedestal,
                    _ => LockerType.Unknown,
                };
            }
        }
        public void AssignPickup(Pickup p) => _locker.AssignPickup(p);
        public void DoorTrigger() => _locker.DoorTrigger();
        public void LockPickups(bool state, uint chamberId, bool anyOpen) => _locker.LockPickups(state, chamberId, anyOpen);
        public void ProcessChambers() => _locker.ProcessChambers();
        public void SpawnItem(global::Locker.ItemToSpawn item) => _locker.SpawnItem(item);
    }
}