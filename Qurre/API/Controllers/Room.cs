using MapGeneration;
using Mirror;
using Qurre.API.Objects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Qurre.API.Controllers
{
    public class Room
    {
        internal Room(RoomIdentifier identifier)
        {
            Identifier = identifier;
            if (Identifier == null) return;
            Type = Identifier.Name;
            Shape = Identifier.Shape;
            GameObject = identifier.gameObject;
            Id = Identifier.UniqueId;

            LightController = GameObject.GetComponentInChildren<FlickerableLightController>();

            foreach (var cam in GameObject.GetComponentsInChildren<Camera079>())
                Cameras.Add(new Camera(cam, this));
        }
        internal FlickerableLightController LightController { get; set; }
        public void LightsOff(float duration) => LightController.ServerFlickerLights(duration);
        public float LightIntensity
        {
            get => LightController.LightIntensityMultiplier;
            set => LightController.UpdateLightsIntensity(LightController.LightIntensityMultiplier, value);
        }
        public Color LightColor
        {
            get => LightController.Network_warheadLightColor;
            set
            {
                LightController.Network_warheadLightColor = value;
                LightOverride = true;
            }
        }
        public bool LightOverride
        {
            get => LightController.WarheadLightOverride;
            set => LightController.WarheadLightOverride = value;
        }
        public GameObject GameObject { get; }
        public RoomIdentifier Identifier { get; }
        public Transform Transform => GameObject.transform;
        public Vector3 Position
        {
            get => Transform.position;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                Transform.position = value;
                NetworkServer.Spawn(GameObject);
            }
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
        public string Name => GameObject.name;
        public List<Door> Doors { get; } = new List<Door>();
        public List<Camera> Cameras { get; } = new List<Camera>();
        public List<Player> Players => Player.List.Where(x => !x.IsHost && x.Room.Name == Name).ToList();
        public ZoneType Zone
        {
            get
            {
                if (zone != ZoneType.Unspecified)
                    return zone;
                zone = ZoneType.Unspecified;
                if (Position.y >= 0f && Position.y < 500f)
                    zone = ZoneType.Light;
                else if (Name.Contains("EZ") || Name.Contains("INTERCOM"))
                    zone = ZoneType.Office;
                else if (Position.y < -100 && Position.y > -1015f)
                    zone = ZoneType.Heavy;
                else if (Position.y >= 5)
                    zone = ZoneType.Surface;
                return zone;
            }
        }
        public RoomName Type { get; }
        public RoomShape Shape { get; }
        private ZoneType zone = ZoneType.Unspecified;
        public int Id { get; }
        public bool IsLightsOff => LightController && !LightController.IsEnabled();
    }
}