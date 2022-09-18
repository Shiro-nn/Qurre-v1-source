using Qurre.API.Addons.Models;
using Qurre.API.Objects;
using System.Linq;
using UnityEngine;
namespace Qurre.API.Controllers
{
    public static class GlobalLights
    {
        public static void TurnOff(float duration)
        {
            foreach (var room in Map.Rooms)
                room.LightsOff(duration);
        }
        public static void TurnOff(float duration, ZoneType zone)
        {
            foreach (var room in Map.Rooms.Where(x => x.Zone == zone))
                room.LightsOff(duration);
        }
        public static void ChangeColor(Color color)
        {
            foreach (var room in Map.Rooms)
                room.Lights.Color = color;
            foreach (var room in CustomRoom._list)
                room.LightsController.Color = color;
        }
        public static void ChangeColor(Color color, ZoneType zone)
        {
            foreach (var room in Map.Rooms.Where(x => x.Zone == zone))
                room.Lights.Color = color;
        }
        public static void Intensivity(float intensive)
        {
            foreach (var room in Map.Rooms)
                room.Lights.Intensity = intensive;
            //set for custom rooms is not prefere
        }
        public static void Intensivity(float intensive, ZoneType zone)
        {
            foreach (var room in Map.Rooms.Where(x => x.Zone == zone))
                room.Lights.Intensity = intensive;
        }
    }
}