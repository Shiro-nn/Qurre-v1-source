using HarmonyLib;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using System;
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
				if (firearmAudioClip.HasFlag(FirearmAudioFlags.IsGunshot) && owner.transform.position.y > 900f)
					num *= 2.3f;
				float num2 = num * num;
				foreach (ReferenceHub referenceHub in ReferenceHub.GetAllHubs().Values)
				{
					if (referenceHub != firearm.Owner)
					{
						RoleType curClass = referenceHub.characterClassManager.CurClass;
						if (curClass == RoleType.Spectator || curClass == RoleType.Scp079 || (referenceHub.transform.position - owner.transform.position).sqrMagnitude <= num2)
							referenceHub.networkIdentity.connectionToClient.Send(new GunAudioMessage(owner, clipId, (byte)Mathf.RoundToInt(Mathf.Clamp(num, 0f, 255f)), referenceHub), 0);
					}
				}
				Action<Firearm, byte, float> serverSoundPlayed = FirearmExtensions.ServerSoundPlayed;
				if (serverSoundPlayed == null) return false;
				serverSoundPlayed(firearm, clipId, num);
            }
            catch { }
			return false;
        }
    }
}