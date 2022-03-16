using Interactables.Interobjects.DoorUtils;
using UnityEngine;
namespace Qurre.API.Modules
{
    internal class DoorsUpdater : MonoBehaviour
    {
        internal DoorVariant Door;
        internal void Update()
        {
            if (Door is null) return;
            try { Door.netIdentity.UpdateData(); } catch { }
        }
    }
}