#pragma warning disable SA1313
using System.Collections.Generic;
using HarmonyLib;
using Qurre.API.Events;
using Qurre.Events;
using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetPlayersClass))]
    internal static class RoleChange
    {
        private static bool Prefix(CharacterClassManager __instance, ref RoleType classid, GameObject ply, bool lite = false, bool escape = false)
        {
            try
            {
                if (!ply.GetComponent<CharacterClassManager>().IsVerified) return false;
                API.Player pl = API.Player.Get(ply);
                var sIL = new List<ItemType>();
                foreach (ItemType item in __instance.Classes.SafeGet(classid).startItems) sIL.Add(item);
                var cRE = new RoleChangeEvent(pl, classid, sIL, lite, escape);
                if (cRE.NewRole == RoleType.Spectator) cRE.Player.DropItems();
                Player.rolechange(cRE);
                lite = cRE.SavePos;
                escape = cRE.Escaped;
                if (classid != RoleType.Spectator && cRE.NewRole == RoleType.Spectator)
                    Player.dead(new DeadEvent(API.Server.Host, cRE.Player, new PlayerStats.HitInfo(-1, "Dedicated Server", DamageTypes.None, 0)));
                classid = cRE.NewRole;
                if (escape)
                {
                    var eE = new EscapeEvent(pl, classid);
                    Player.escape(eE);
                    if (!eE.Allowed) return false;
                    classid = eE.NewRole;
                }
                pl.ClassManager.SetClassIDAdv(classid, lite, escape);
                pl.PlayerStats.SetHPAmount(__instance.Classes.SafeGet(classid).maxHP);
                if (lite) return false;
                Inventory inv = ply.GetComponent<Inventory>();
                List<Inventory.SyncItemInfo> list = new List<Inventory.SyncItemInfo>();
                if (escape && CharacterClassManager.KeepItemsAfterEscaping)
                    foreach (Inventory.SyncItemInfo item in inv.items)
                        list.Add(item);
                inv.items.Clear();
                foreach (ItemType id in cRE.Items)
                    inv.AddNewItem(id, -4.65664672E+11f, 0, 0, 0);
                if (escape && CharacterClassManager.KeepItemsAfterEscaping)
                    foreach (Inventory.SyncItemInfo syncItemInfo in list)
                        if (CharacterClassManager.PutItemsInInvAfterEscaping)
                        {
                            Item itemByID = inv.GetItemByID(syncItemInfo.id);
                            bool flag = false;
                            InventoryCategory[] categories = __instance._search().categories;
                            int i = 0;
                            while (i < categories.Length)
                            {
                                InventoryCategory inventoryCategory = categories[i];
                                if (inventoryCategory.itemType == itemByID.itemCategory && (itemByID.itemCategory != ItemCategory.None || itemByID.itemCategory != ItemCategory.None))
                                {
                                    int num = 0;
                                    foreach (Inventory.SyncItemInfo syncItemInfo2 in inv.items)
                                        if (inv.GetItemByID(syncItemInfo2.id).itemCategory == itemByID.itemCategory)
                                            num++;
                                    if (num >= inventoryCategory.maxItems)
                                    {
                                        flag = true;
                                        break;
                                    }
                                    break;
                                }
                                else i++;
                            }
                            if (inv.items.Count >= 8 || flag)
                                inv.SetPickup(syncItemInfo.id, syncItemInfo.durability, __instance._pms().RealModelPosition, Quaternion.Euler(__instance._pms().Rotations.x, __instance._pms().Rotations.y, 0f), syncItemInfo.modSight, syncItemInfo.modBarrel, syncItemInfo.modOther);
                            else
                                inv.AddNewItem(syncItemInfo.id, syncItemInfo.durability, syncItemInfo.modSight, syncItemInfo.modBarrel, syncItemInfo.modOther);
                        }
                        else
                            inv.SetPickup(syncItemInfo.id, syncItemInfo.durability, __instance._pms().RealModelPosition, Quaternion.Euler(__instance._pms().Rotations.x, __instance._pms().Rotations.y, 0f), syncItemInfo.modSight, syncItemInfo.modBarrel, syncItemInfo.modOther);
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