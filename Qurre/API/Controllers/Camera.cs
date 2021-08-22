using UnityEngine;
namespace Qurre.API.Controllers
{
    public class Camera
    {
        private readonly Camera079 cmr;
        internal Camera(Camera079 camera, Room room)
        {
            cmr = camera;
            Room = room;
        }
        public GameObject GameObject => cmr.gameObject;
        public Room Room { get; private set; }
        public string Name => cmr.cameraName;
        public ushort Id => cmr.cameraId;
        public bool Main => cmr.isMain;
    }
}