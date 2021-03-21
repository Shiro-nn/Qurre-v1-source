using Qurre.API.Objects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Qurre.API.Controllers
{
    public class Room
    {
        internal Room(GameObject gameObject)
        {
            GameObject = gameObject;
            name = gameObject.name;
            LightController = GameObject.GetComponentInChildren<FlickerableLightController>();
            var info = gameObject.GetComponentInChildren<RoomInformation>();
            RoomInformationType = info.CurrentRoomType;
        }
        internal FlickerableLightController LightController { get; set; }
        public void LightsOff(float duration) => LightController.ServerFlickerLights(duration);
        public bool IsLightsOff => FlickerableLightController && FlickerableLightController.IsEnabled();
        private FlickerableLightController FlickerableLightController { get; set; }
        public void SetLightIntensity(float intensity) => LightController.ServerSetLightIntensity(intensity);
        public GameObject GameObject { get; }
        public Transform Transform => GameObject.transform;
        public Vector3 Position => GameObject.transform.position;
        public string Name => name;
        public List<Door> Doors { get; } = new List<Door>();
        public List<Player> Players => Player.List.Where(x => !x.IsHost && x.Room.Name == Name).ToList();
        public ZoneType Zone
        {
            get
            {
                if (zone != ZoneType.Unspecified)
                    return zone;
                zone = ZoneType.Unspecified;
                if (Position.y == -1997f)
                    zone = ZoneType.Unspecified;
                else if (Position.y >= 0f && Position.y < 500f)
                    zone = ZoneType.LightContainment;
                else if (Position.y < -100 && Position.y > -1000f)
                    zone = ZoneType.HeavyContainment;
                else if (Name.Contains("ENT") || Name.Contains("INTERCOM"))
                    zone = ZoneType.Entrance;
                else if (Position.y >= 5)
                    zone = ZoneType.Surface;
                return zone;
            }
        }
        public RoomType Type
        {
            get
            {
                var rawName = Name;
                var bracketStart = rawName.IndexOf('(') - 1;
                if (bracketStart > 0) rawName = rawName.Remove(bracketStart, rawName.Length - bracketStart);
                switch (rawName)
                {
                    case "LCZ_Armory":
                        return RoomType.LczArmory;
                    case "LCZ_Curve":
                        return RoomType.LczCurve;
                    case "LCZ_Straight":
                        return RoomType.LczStraight;
                    case "LCZ_012":
                        return RoomType.Lcz012;
                    case "LCZ_914":
                        return RoomType.Lcz914;
                    case "LCZ_Crossing":
                        return RoomType.LczCrossing;
                    case "LCZ_TCross":
                        return RoomType.LczTCross;
                    case "LCZ_Cafe":
                        return RoomType.LczCafe;
                    case "LCZ_Plants":
                        return RoomType.LczPlants;
                    case "LCZ_Toilets":
                        return RoomType.LczToilets;
                    case "LCZ_Airlock":
                        return RoomType.LczAirlock;
                    case "LCZ_173":
                        return RoomType.Lcz173;
                    case "LCZ_ClassDSpawn":
                        return RoomType.LczClassDSpawn;
                    case "LCZ_ChkpB":
                        return RoomType.LczChkpB;
                    case "LCZ_372":
                        return RoomType.Lcz372;
                    case "LCZ_ChkpA":
                        return RoomType.LczChkpA;
                    case "HCZ_079":
                        return RoomType.Hcz079;
                    case "HCZ_EZ_Checkpoint":
                        return RoomType.HczEzCheckpoint;
                    case "HCZ_Room3ar":
                        return RoomType.HczArmory;
                    case "HCZ_Testroom":
                        return RoomType.Hcz939;
                    case "HCZ_Hid":
                        return RoomType.HczHid;
                    case "HCZ_049":
                        return RoomType.Hcz049;
                    case "HCZ_ChkpA":
                        return RoomType.HczChkpA;
                    case "HCZ_Crossing":
                        return RoomType.HczCrossing;
                    case "HCZ_106":
                        return RoomType.Hcz106;
                    case "HCZ_Nuke":
                        return RoomType.HczNuke;
                    case "HCZ_Tesla":
                        return RoomType.HczTesla;
                    case "HCZ_Servers":
                        return RoomType.HczServers;
                    case "HCZ_ChkpB":
                        return RoomType.HczChkpB;
                    case "HCZ_Room3":
                        return RoomType.HczTCross;
                    case "HCZ_457":
                        return RoomType.Hcz096;
                    case "HCZ_Curve":
                        return RoomType.HczCurve;
                    case "EZ_Endoof":
                        return RoomType.EzVent;
                    case "EZ_Intercom":
                        return RoomType.EzIntercom;
                    case "EZ_GateA":
                        return RoomType.EzGateA;
                    case "EZ_PCs_small":
                        return RoomType.EzDownstairsPcs;
                    case "EZ_Curve":
                        return RoomType.EzCurve;
                    case "EZ_PCs":
                        return RoomType.EzPcs;
                    case "EZ_Crossing":
                        return RoomType.EzCrossing;
                    case "EZ_CollapsedTunnel":
                        return RoomType.EzCollapsedTunnel;
                    case "EZ_Smallrooms2":
                        return RoomType.EzConference;
                    case "EZ_Straight":
                        return RoomType.EzStraight;
                    case "EZ_Cafeteria":
                        return RoomType.EzCafeteria;
                    case "EZ_upstairs":
                        return RoomType.EzUpstairsPcs;
                    case "EZ_GateB":
                        return RoomType.EzGateB;
                    case "EZ_Shelter":
                        return RoomType.EzShelter;
                    case "PocketWorld":
                        return RoomType.Pocket;
                    case "Outside":
                        return RoomType.Surface;
                    default:
                        return RoomType.Unknown;
                }
            }
        }
        private ZoneType zone = ZoneType.Unspecified;
        internal string name;
        public RoomInformation.RoomType RoomInformationType { get; }
    }
}