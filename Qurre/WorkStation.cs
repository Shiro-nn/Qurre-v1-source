using Mirror;
using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.API.Controllers
{
    public class WorkStation
    {
        private Pickup connectedtablet;
        private readonly global::WorkStation workStation;
        internal WorkStation(global::WorkStation station)
        {
            workStation = station;
        }
        public WorkStation(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            var bench = Object.Instantiate(NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name == "Work Station"));
            bench.gameObject.transform.position = position;
            bench.gameObject.transform.localScale = scale;
            NetworkServer.Spawn(bench);
            workStation = bench.GetComponent<global::WorkStation>();
            Map.WorkStations.Add(this);
        }
        public GameObject GameObject => workStation.gameObject;
        public string Name => GameObject.name;
        public Vector3 Position
        {
            get => GameObject.transform.position;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.position = value;
                NetworkServer.Spawn(GameObject);
            }
        }
        public Vector3 Scale
        {
            get => GameObject.transform.localScale;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.localScale = value;
                NetworkServer.Spawn(GameObject);
            }
        }
        public bool TabletConnected
        {
            get => workStation.NetworkisTabletConnected;
            set
            {

                if (value)
                {
                    workStation.NetworkisTabletConnected = true;
                    workStation.WS_animationCooldown(6.5f);
                }
                else
                {
                    if (ConnectedTablet != null && TabletOwner != null) TabletOwner.AddItem(ConnectedTablet.itemId, ConnectedTablet.durability);
                    TabletOwner = null;
                    workStation.NetworkisTabletConnected = false;
                    workStation.WS_animationCooldown(3.5f);

                }
            }
        }
        public Pickup ConnectedTablet
        {
            get => connectedtablet;
            set
            {
                connectedtablet = value;
                if (value != null)
                {
                    TabletConnected = true;
                    value.Delete();
                }
                else TabletConnected = false;
            }
        }
        public Player TabletOwner
        {
            get => workStation.Network_playerConnected == null ? null : Player.Get(workStation.Network_playerConnected);
            set
            {
                if (value == null) workStation.Network_playerConnected = null;
                else workStation.Network_playerConnected = value.GameObject;
            }
        }
        public static WorkStation Create(Vector3 position, Vector3 rotation, Vector3 scale) => new WorkStation(position, rotation, scale);
    }
}