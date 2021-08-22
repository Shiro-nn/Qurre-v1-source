﻿using MapGeneration.Distributors;
using Mirror;
using UnityEngine;
namespace Qurre.API.Controllers
{
    public class Generator
    {
        internal Generator(Scp079Generator g)
        {
            generator = g;
            positionsync = generator.GetComponent<StructurePositionSync>();
        }
        private Scp079Generator generator;
        private readonly StructurePositionSync positionsync;
        public GameObject GameObject => generator.gameObject;
        public string Name => GameObject.name;
        public Transform Transform => GameObject.transform;
        public Vector3 Position
        {
            get => Transform.position;
            set => positionsync.Network_position = value;
        }
        public Quaternion Rotation
        {
            get => Transform.localRotation;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                Transform.localRotation = value;
                NetworkServer.Spawn(GameObject);
            }
        }
        public Vector3 Scale
        {
            get => Transform.localScale;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                Transform.localScale = value;
                NetworkServer.Spawn(GameObject);
            }
        }
        public bool Open
        {
            get => generator.HasFlag(generator._flags, Scp079Generator.GeneratorFlags.Open);
            set
            {
                generator.ServerSetFlag(Scp079Generator.GeneratorFlags.Open, value);
                generator._targetCooldown = generator._doorToggleCooldownTime;
            }
        }
        public bool Locked
        {
            get => !generator.HasFlag(generator._flags, Scp079Generator.GeneratorFlags.Unlocked);
            set
            {
                generator.ServerSetFlag(Scp079Generator.GeneratorFlags.Unlocked, !value);
                generator._targetCooldown = generator._unlockCooldownTime;
            }
        }
        public bool Active
        {
            get => generator.Activating;
            set
            {
                generator.Activating = value;
                if (value) generator._leverStopwatch.Restart();
                generator._targetCooldown = generator._doorToggleCooldownTime;
            }
        }
        public bool Engaged { get => generator.Engaged; set => generator.Engaged = value; }
        public short Time { get => generator._syncTime; set => generator.Network_syncTime = value; }
    }
}