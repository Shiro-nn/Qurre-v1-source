using System.Collections.Generic;
using UnityEngine;
namespace Qurre.API.Controllers
{
    public class Camera
    {
        internal static readonly Dictionary<Camera079, Camera> Cameras = new Dictionary<Camera079, Camera>();
        private readonly Camera079 cmr;
        internal Camera(Camera079 camera, Room room)
        {
            cmr = camera;
            Room = room;
            if (Cameras.ContainsKey(camera)) Cameras.Remove(camera);
            Cameras.Add(camera, this);
        }
        public GameObject GameObject => cmr.gameObject;
        public Room Room { get; private set; }
        public string Name => cmr.cameraName;
        public ushort Id => cmr.cameraId;
        public bool Main => cmr.isMain;
    }
}