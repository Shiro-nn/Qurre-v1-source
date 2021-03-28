using QurreModLoader;
using System.Linq;
using UnityEngine;
namespace Qurre.API.Controllers
{
    public class Generator
    {
        internal Generator(Generator079 g, bool main)
        {
            generator = g;
            Main = main;
        }
        private Generator079 generator;
        private Pickup tablet;
        public GameObject GameObject => generator.gameObject;
        public readonly bool Main;
        public string Name => GameObject.name;
        public Vector3 Position => GameObject.transform.position;
        public bool Open
        {
            get => generator.NetworkisDoorOpen;
            set
            {
                generator.Gen_doorAnimationCooldown(1.5f);
                generator.NetworkisDoorOpen = value;
                generator.Generator_RpcDoSound(!value);
            }
        }
        public bool Locked
        {
            get => !generator.NetworkisDoorUnlocked;
            set
            {
                if (value == Locked) return;
                generator.NetworkisDoorUnlocked = !value;
                generator.Gen_doorAnimationCooldown(0.5f);
            }
        }
        public bool TabletConnected
        {
            get => generator.isTabletConnected;
            set
            {
                if (value)
                {
                    if (!TabletConnected) generator.NetworkisTabletConnected = true;
                }
                else
                {
                    if (TabletConnected) generator.EjectTablet();
                }
            }
        }
        public Pickup ConnectedTablet
        {
            get => tablet;
            set
            {
                tablet = value;
                if (value != null)
                {
                    TabletConnected = true;
                    value.Delete();
                }
                else TabletConnected = false;
            }
        }
        public float RemainingPowerUp
        {
            get => generator.remainingPowerup;
            set => generator.Generator_SetTime(value);
        }
        public Room Room => Map.Rooms.FirstOrDefault(x => x.Name.ToLower() == generator.CurRoom.ToLower());
        public Vector3 TabletEjectionPoint => generator.tabletEjectionPoint.position;
        public bool Overcharged => Heavy.ForcedOvercharge || generator.Generator_localTime() <= 0f;
        public void Overcharge()
        {
            if (Overcharged) return;
            Locked = false;
            Heavy.ActiveGenerators++;
            generator.NetworkremainingPowerup = 0f;
            generator.Generator_localTime(0f);
            generator.EjectTablet();
            generator.Generator_RpcNotify(Heavy.ActiveGenerators);
            if (Heavy.ActiveGenerators < 5)
                Respawning.RespawnEffectsController.PlayCassieAnnouncement(string.Concat(new object[]
                {
                        "JAM_",
                        UnityEngine.Random.Range(0, 100).ToString("000"),
                        "_",
                        UnityEngine.Random.Range(2, 5),
                        " SCP079RECON",
                        Heavy.ActiveGenerators
                }), false, true);
            else Recontainer079.BeginContainment(false);
        }
    }
}