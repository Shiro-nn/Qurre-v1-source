using Qurre.API.Objects;
using UnityEngine;
namespace Qurre.API.Controllers
{
    public class Lights
    {
        public static bool IsLightsOff => Generator079.Generators[0].enabled;
        public static void TurnOff(float duration, bool onlyHeavy = false) => Generator079.Generators[0].ServerOvercharge(duration, onlyHeavy);
        public static void Intensivity(float intensive)
        {
            foreach (FlickerableLightController flickerableLightController in Object.FindObjectsOfType<FlickerableLightController>())
            {
                Scp079Interactable component = flickerableLightController.GetComponent<Scp079Interactable>();
                if (component != null && component.type == Scp079Interactable.InteractableType.LightController)
                {
                    flickerableLightController.ServerSetLightIntensity(intensive);
                }
            }
        }
        public static void Intensivity(float intensive, ZoneType zone)
        {
            foreach (FlickerableLightController flickerableLightController in Object.FindObjectsOfType<global::FlickerableLightController>())
            {
                Scp079Interactable component = flickerableLightController.GetComponent<Scp079Interactable>();
                if (component != null && component.type == Scp079Interactable.InteractableType.LightController && component.currentZonesAndRooms.Count != 0)
                {
                    string b;
                    switch (zone)
                    {
                        case ZoneType.LightContainment:
                            b = "LightRooms";
                            break;
                        case ZoneType.HeavyContainment:
                            b = "HeavyRooms";
                            break;
                        case ZoneType.Entrance:
                            b = "EntranceRooms";
                            break;
                        case ZoneType.Surface:
                            if (component.optionalObject.transform.position.y > 900f)
                            {
                                flickerableLightController.ServerSetLightIntensity(intensive);
                            }
                            return;
                        default:
                            Log.Debug("small, UNDEFINED zone");
                            return;
                    }
                    if (component.currentZonesAndRooms[0].currentZone == b)
                    {
                        flickerableLightController.ServerSetLightIntensity(intensive);
                    }
                }
            }
        }
    }
}