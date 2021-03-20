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
        public void Trigger() => Gate.CallRpcPlayAnimation();
        public void InstantTrigger() => Gate.RpcInstantTesla();
        public void Destroy() => Object.Destroy(Gate.gameObject);
    }
}