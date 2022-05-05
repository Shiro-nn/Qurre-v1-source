using HarmonyLib;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using Qurre.API;
using UnityEngine;

namespace Qurre.Patches.Fixes
{
    [HarmonyPatch(typeof(FirearmExtensions), "ServerSendAudioMessage")]
    internal static class ShootDisconnect
    {
        internal static bool Prefix(Firearm firearm, byte clipId)
        {
            try
            {
                FirearmAudioClip firearmAudioClip = firearm.AudioClips[clipId];
                ReferenceHub owner = firearm.Owner;

                float num = firearmAudioClip.HasFlag(FirearmAudioFlags.ScaleDistance)
                    ? (firearmAudioClip.MaxDistance * firearm.AttachmentsValue(AttachmentParam.GunshotLoudnessMultiplier))
                    : firearmAudioClip.MaxDistance;

                if (firearmAudioClip.HasFlag(FirearmAudioFlags.IsGunshot) && owner.transform.position.y > 900f)
                    num *= 2.3f;
                float num2 = num * num;

                foreach (ReferenceHub hub in ReferenceHub.GetAllHubs().Values)
                {
                    Player ply = Player.Get(hub);
                    if (ply is not null && !ply.Bot && hub != firearm.Owner)
                    {
                        if (hub.characterClassManager.CurClass is RoleType.Spectator or RoleType.Scp079 || !((hub.transform.position - owner.transform.position).sqrMagnitude > num2))
                        {
                            hub.networkIdentity.connectionToClient.Send(new GunAudioMessage(owner, clipId, (byte)Mathf.RoundToInt(Mathf.Clamp(num, 0f, 255f)), hub));
                        }
                    }
                }
                FirearmExtensions.ServerSoundPlayed?.Invoke(firearm, clipId, num);
            }
            catch { }
            return false;
        }
    }
}
