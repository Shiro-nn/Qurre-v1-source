using Qurre.API.Addons.Models;
using Qurre.API.Objects;
using System.Linq;
using UnityEngine;
namespace Qurre.API.Controllers
{
    public static class GlobalLights
    {
        static public void TurnOff(float duration)
        {
            foreach (var room in Map.Rooms)
                room.LightsOff(duration);
        }
        static public void TurnOff(float duration, ZoneType zone)
        {
            foreach (var room in Map.Rooms.Where(x => x.Zone == zone))
                room.LightsOff(duration);
        }
        static public void ChangeColor(Color color, bool customToo = true)
        {
            foreach (var room in Map.Rooms)
                room.Lights.Color = color;
            if (customToo) foreach (var room in CustomRoom._list)
                    room.LightsController.Color = color;
        }
        static public void ChangeColor(Color color, ZoneType zone)
        {
            foreach (var room in Map.Rooms.Where(x => x.Zone == zone))
                room.Lights.Color = color;
        }
        static public void Intensivity(float intensive, bool customToo = false)
        {
            foreach (var room in Map.Rooms)
                room.Lights.Intensity = intensive;
            if (customToo) foreach (var room in CustomRoom._list)
                    room.LightsController.Intensity = intensive;
        }
        static public void Intensivity(float intensive, ZoneType zone)
        {
            foreach (var room in Map.Rooms.Where(x => x.Zone == zone))
                room.Lights.Intensity = intensive;
        }
        static public void SetToDefault(bool customToo = true)
        {
            foreach (var room in Map.Rooms)
                room.Lights.Override = false;
            if (customToo) foreach (var room in CustomRoom._list)
                    room.LightsController.Override = false;
        }
    }
}