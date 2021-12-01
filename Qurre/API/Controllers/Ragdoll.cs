using Mirror;
using PlayerStatsSystem;
using UnityEngine;
namespace Qurre.API.Controllers
{
    public class Ragdoll
    {
        internal Ragdoll(global::Ragdoll _, int id)
        {
            ragdoll = _;
            _id = id;
        }
        public Ragdoll(RoleType roletype, Vector3 pos, Quaternion rot, DamageHandlerBase handler, Player owner)
        {
            var role = Server.Host.ClassManager.Classes.SafeGet((int)roletype);
            var gameObject = Object.Instantiate(role.model_ragdoll, pos + role.model_offset.position, Quaternion.Euler(rot.eulerAngles + role.model_offset.rotation));
            if (!gameObject.TryGetComponent(out global::Ragdoll component)) return;
            ragdoll = component;
            ragdoll.NetworkInfo = new RagdollInfo(owner.ReferenceHub, handler, gameObject.transform.localPosition, gameObject.transform.localRotation);
            NetworkServer.Spawn(component.gameObject);
            _id = owner.Id;
            try
            {
                if (Owner != null)
                {
                    var s1 = Scale;
                    var s2 = Owner.Scale;
                    Scale = new Vector3(s1.x * s2.x, s1.y * s2.y, s1.z * s2.z);
                }
            }
            catch { }
            Map.Ragdolls.Add(this);
        }
        private int _id = 0;
        private readonly global::Ragdoll ragdoll;
        public GameObject GameObject => ragdoll.gameObject;
        public string Name => ragdoll.name;
        public Vector3 Position
        {
            get
            {
                try { return ragdoll.transform.position; }
                catch { return Vector3.zero; }
            }
            set
            {
                NetworkServer.UnSpawn(GameObject);
                ragdoll.transform.position = value;
                NetworkServer.Spawn(GameObject);
                var info = ragdoll.Info;
                ragdoll.NetworkInfo = new RagdollInfo(info.OwnerHub, info.Handler, value, info.StartRotation);
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
                var info = ragdoll.Info;
                ragdoll.NetworkInfo = new RagdollInfo(info.OwnerHub, info.Handler, info.StartPosition, value);
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
                var info = ragdoll.Info;
                ragdoll.NetworkInfo = new RagdollInfo(value.ReferenceHub, info.Handler, info.StartPosition, info.StartRotation);
            }
        }
        public void Destroy()
        {
            Object.Destroy(GameObject);
            Map.Ragdolls.Remove(this);
        }
        public static Ragdoll Create(RoleType roletype, Vector3 pos, Quaternion rot, DamageHandlerBase handler, Player owner)
            => new(roletype, pos, rot, handler, owner);
    }
}