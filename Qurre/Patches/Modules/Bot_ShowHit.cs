using HarmonyLib;
using InventorySystem.Items.Firearms.BasicMessages;
using InventorySystem.Items.Firearms.Modules;
using Qurre.API;
using System;
using UnityEngine;
namespace Qurre.Patches.Modules
{
    [HarmonyPatch(typeof(StandardHitregBase), nameof(StandardHitregBase.ShowHitIndicator))]
    internal static class Bot_ShowHit
    {
        private static bool Prefix(uint netId, float damage, Vector3 origin)
        {
            try
            {
                if (!ReferenceHub.TryGetHubNetID(netId, out ReferenceHub hub)) return false;
                var pl = Player.Get(hub);
                if (pl == null || pl.Bot) return false;
                foreach (ReferenceHub hub2 in hub.spectatorManager.ServerCurrentSpectatingPlayers)
                {
                    hub2.networkIdentity.connectionToClient.Send(new GunHitMessage
                    {
                        Weapon = ItemType.None,
                        Damage = (byte)Mathf.Round(damage * 2.5f),
                        DamagePosition = origin
                    });
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Modules [Bot ShowHit]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}