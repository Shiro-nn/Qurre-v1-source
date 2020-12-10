#pragma warning disable SA1313
using System.Collections.Generic;
using HarmonyLib;
using Qurre.API.Events;
using Qurre.Events;
using UnityEngine;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetPlayersClass))]
    internal static class RoleChange
    {
        private static bool Prefix(CharacterClassManager __instance, ref RoleType classid, GameObject ply, bool lite = false, bool escape = false)
        {
            try
            {
                if (!ply.GetComponent<CharacterClassManager>().IsVerified)
                    return false;
                var sIL = new List<ItemType>();
                foreach (ItemType item in __instance.Classes.SafeGet(classid).startItems) sIL.Add(item);
                var cRE = new RoleChangeEvent(ReferenceHub.GetHub(ply), classid, sIL, lite, escape);
                Player.rolechange(cRE);
                lite = cRE.IsSavePos;
                escape = cRE.IsEscaped;
                if (classid != RoleType.Spectator && cRE.NewRole == RoleType.Spectator)
                    Player.died(new DiedEvent(API.Map.Host, cRE.Player, new PlayerStats.HitInfo(-1, "Dedicated Server", DamageTypes.None, 0)));
                classid = cRE.NewRole;
                if (escape)
                {
                    var eE = new EscapeEvent(ReferenceHub.GetHub(ply), classid);
                    Player.escape(eE);
                    if (!eE.IsAllowed)
                        return false;
                    classid = eE.NewRole;
                }
                ply.GetComponent<CharacterClassManager>().SetClassIDAdv(classid, lite, escape);
                ply.GetComponent<PlayerStats>().SetHPAmount(__instance.Classes.SafeGet(classid).maxHP);
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching PlayeR.RoleChange:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}