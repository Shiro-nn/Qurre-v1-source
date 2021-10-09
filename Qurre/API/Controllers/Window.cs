using Footprinting;
using UnityEngine;
namespace Qurre.API.Controllers
{
    public class Window
    {
        internal Window(BreakableWindow window) => bw = window;
        private readonly BreakableWindow bw;
        public BreakableWindow Breakable => bw;
        public GameObject GameObject => bw.gameObject;
        public Transform Transform => bw.transform;
        public Vector3 Position => Transform.position;
        public Vector3 Size => bw.size;
        public Footprint LastAttacker => bw.LastAttacker;
        public bool Destroyed
        {
            get => bw.isBroken;
            set => bw.BreakWindow();
        }
        public float Hp
        {
            get => bw.health;
            set => bw.ServerDamageWindow(value);
        }
        public BreakableWindow.BreakableWindowStatus Status
        {
            get => bw.NetworksyncStatus;
            set => bw.UpdateStatus(value);
        }
    }
}