using UnityEngine;
namespace Qurre.API.Controllers
{
    public class Tesla
    {
        private TeslaGate Gate;
        internal Tesla(TeslaGate gate) => Gate = gate;
        public GameObject GameObject => Gate.gameObject;
        public Vector3 Position { get => Gate.localPosition; }
        public float SizeOfTrigger { get => Gate.sizeOfTrigger; set => Gate.sizeOfTrigger = value; }
        public void Trigger(bool Instant = false)
        {
            if (Instant)
                Gate.RpcInstantTesla();
            else
                Gate.CallRpcPlayAnimation();
        }
        public void Destroy() => Object.Destroy(Gate.gameObject);
    }
}
