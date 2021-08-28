using Mirror;
using UnityEngine;
namespace Qurre.API.Controllers
{
    public class Ragdoll
    {
        internal Ragdoll(global::Ragdoll _)
        {
            ragdoll = _;
            _id = ragdoll.owner.PlayerId;
        }
        public Ragdoll(RoleType roletype, Vector3 pos, Quaternion rot, Vector3 velocity, PlayerStats.HitInfo info, bool allowRecall, Player owner)
        {
            var role = Server.Host.ClassManager.Classes.SafeGet((int)roletype);
            var gameobject = Object.Instantiate(role.model_ragdoll, pos + role.ragdoll_offset.position, Quaternion.Euler(rot.eulerAngles + role.ragdoll_offset.rotation));
            ragdoll = gameobject.GetComponent<global::Ragdoll>();
            ragdoll.Networkowner = new global::Ragdoll.Info(owner.UserId, owner.Nickname, info, role, owner.Id);
            ragdoll.NetworkallowRecall = allowRecall;
            ragdoll.NetworkPlayerVelo = velocity;
            NetworkServer.Spawn(gameobject);
            _id = owner.Id;
            Map.Ragdolls.Add(this);
        }
        public Ragdoll(RoleType roletype, Vector3 pos, Quaternion rot, Vector3 velocity, PlayerStats.HitInfo info, bool allowRecall, string nickname, int id, int ownerId)
        {
            var role = Server.Host.ClassManager.Classes.SafeGet((int)roletype);
            var gameobject = Object.Instantiate(role.model_ragdoll, pos + role.ragdoll_offset.position, Quaternion.Euler(rot.eulerAngles + role.ragdoll_offset.rotation));
            ragdoll = gameobject.GetComponent<global::Ragdoll>();
            ragdoll.Networkowner = new global::Ragdoll.Info(nickname, nickname, info, role, id);
            ragdoll.NetworkallowRecall = allowRecall;
            ragdoll.NetworkPlayerVelo = velocity;
            NetworkServer.Spawn(gameobject);
            _id = ownerId;
            Map.Ragdolls.Add(this);
        }
        private int _id = 0;
        private readonly global::Ragdoll ragdoll;
        public GameObject GameObject => ragdoll.gameObject;
        public string Name => ragdoll.name;
        public Vector3 Position
        {
            get => ragdoll.transform.position;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                ragdoll.transform.position = value;
                NetworkServer.Spawn(GameObject);
            }
        }
        public Quaternion Rotation
        {
            get => ragdoll.transform.localRotation;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                ragdoll.transform.localRotation = value;
                NetworkServer.Spawn(GameObject);
            }
        }
        public Vector3 Scale
        {
            get => ragdoll.transform.localScale;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                ragdoll.transform.localScale = value;
                NetworkServer.Spawn(GameObject);
            }
        }
        public Player Owner
        {
            get => Player.Get(_id);
            set
            {
                _id = value.Id;
                ragdoll.owner.PlayerId = value.Id;
                ragdoll.owner.Nick = value.Nickname;
                ragdoll.owner.ownerHLAPI_id = value.UserId;
            }
        }
        public bool AllowRecall
        {
            get => ragdoll.allowRecall;
            set => ragdoll.allowRecall = value;
        }
        public void Destroy()
        {
            Object.Destroy(GameObject);
            Map.Ragdolls.Remove(this);
        }
        public static Ragdoll Create(RoleType roletype, Vector3 pos, Quaternion rot, Vector3 velocity, PlayerStats.HitInfo info, bool allowRecall, Player owner)
            => new Ragdoll(roletype, pos, rot, velocity, info, allowRecall, owner);
        public static Ragdoll Create(RoleType roletype, Vector3 pos, Quaternion rot, Vector3 velocity, PlayerStats.HitInfo info, bool allowRecall, string nickname, int id, int ownerId)
            => new Ragdoll(roletype, pos, rot, velocity, info, allowRecall, nickname, id, ownerId);
    }
}