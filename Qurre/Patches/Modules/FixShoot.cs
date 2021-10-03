using HarmonyLib;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using UnityEngine;
namespace Qurre.Patches.Modules
{
    [HarmonyPatch(typeof(FirearmExtensions), nameof(FirearmExtensions.ServerSendAudioMessage))]
    internal static class FixShoot
    {
        private static bool Prefix(Firearm firearm, byte clipId)
        {
            try
            {
                FirearmAudioClip firearmAudioClip = firearm.AudioClips[clipId];
                ReferenceHub owner = firearm.Owner;
                float num = firearmAudioClip.HasFlag(FirearmAudioFlags.ScaleDistance) ?
                    (firearmAudioClip.MaxDistance * firearm.AttachmentsValue(AttachmentParam.GunshotLoudnessMultiplier)) : firearmAudioClip.MaxDistance;
                if (firearmAudioClip.HasFlag(FirearmAudioFlags.IsGunshot) && owner.transform.position.y > 900f) num *= 2.3f;
                float num2 = num * num;
                foreach (ReferenceHub value in ReferenceHub.GetAllHubs().Values)
                {
                    if (value != firearm.Owner)
                    {
                        RoleType curClass = value.characterClassManager.CurClass;
                        if (curClass == RoleType.Spectator || curClass == RoleType.Scp079 || !((value.transform.position - owner.transform.position).sqrMagnitude > num2))
                            value.networkIdentity.connectionToClient.Send(new GunAudioMessage(owner, clipId, (byte)Mathf.RoundToInt(Mathf.Clamp(num, 0f, 255f)), value));
                    }
                }
                FirearmExtensions.ServerSoundPlayed?.Invoke(firearm, clipId, num);
            }
            catch { }
			return false;
        }
    }
}