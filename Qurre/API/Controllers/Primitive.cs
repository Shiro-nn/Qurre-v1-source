using System.Collections.Generic;
using AdminToys;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
namespace Qurre.API.Controllers
{
    public class Primitive
    {
        public Primitive(PrimitiveType type) : this(type, Vector3.zero) { }
        public Primitive(PrimitiveType type, Vector3 position, Color color = default, Quaternion rotation = default, Vector3 size = default)
        {
            try
            {
                var data = NetworkClient.prefabs.Values.ToList().Where(x => x.name == "PrimitiveObjectToy");
                if (data.Count() == 0) return;
                var mod = data.First();
                if (!mod.TryGetComponent<AdminToyBase>(out var primitiveToyBase)) return;
                var prim = UnityEngine.Object.Instantiate(primitiveToyBase, position, rotation);
                Base = (PrimitiveObjectToy)prim;
                Base.PrimitiveType = type;
                Map.Primitives.Add(this);
                NetworkServer.Spawn(Base.gameObject);
                Color = color == default ? Color.white : color;
                Base.transform.position = position;
                Base.transform.rotation = rotation;
                Base.transform.localScale = size == default ? Vector3.one : size;
            }
            catch (Exception e)
            {
                Log.Error($"{e}\n{e.StackTrace}");
            }
        }
        public Vector3 Position
        {
            get => Base.transform.position;
            set
            {
                NetworkServer.UnSpawn(Base.gameObject);
                Base.transform.position = value;
                NetworkServer.Spawn(Base.gameObject);
            }
        }
        public Vector3 Scale
        {
            get => Base.transform.localScale;
            set
            {
                NetworkServer.UnSpawn(Base.gameObject);
                Base.transform.localScale = value;
                NetworkServer.Spawn(Base.gameObject);
            }
        }
        public Quaternion Rotation
        {
            get => Base.transform.localRotation;
            set
            {
                NetworkServer.UnSpawn(Base.gameObject);
                Base.transform.localRotation = value;
                NetworkServer.Spawn(Base.gameObject);
            }
        }
        public Color Color
        {
            get => Base.MaterialColor;
            set => Base.NetworkMaterialColor = value;
        }
        public PrimitiveType Type
        {
            get => Base.PrimitiveType;
            set => Base.NetworkPrimitiveType = value;
        }
        public void Destroy()
        {
            NetworkServer.Destroy(Base.gameObject);
            Map.Primitives.Remove(this);
        }
        public PrimitiveObjectToy Base { get; }
    }
}