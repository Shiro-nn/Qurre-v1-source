#pragma warning disable SA1313
using GameCore;
using HarmonyLib;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.BanUser), new[] { typeof(GameObject), typeof(int), typeof(string), typeof(string), typeof(bool) })]
    internal static class BanAndKick
    {
        private static bool Prefix(GameObject user, int duration, string reason, string issuer, bool isGlobalBan)
        {
            try
            {
                if (isGlobalBan && ConfigFile.ServerConfig.GetBool("gban_ban_ip", false))
                    duration = int.MaxValue;
                string userId = null;
                string address = user.GetComponent<NetworkIdentity>().connectionToClient.address;
                Player targetPlayer = Player.Get(user);
                Player issuerPlayer = Player.Get(issuer) ?? API.Server.Host;
                try { if (ConfigFile.ServerConfig.GetBool("online_mode", false)) userId = targetPlayer.UserId; }
                catch { return false; }
                string umm = (duration > 0) ? Plugin.Config.GetString("Qurre_banned", "banned") : Plugin.Config.GetString("Qurre_kicked", "kicked");
                string message = Plugin.Config.GetString("Qurre_BanOrKick_msg", $"You have been %bok%.").Replace("%bok%", umm);
                if (!string.IsNullOrEmpty(reason))
                    message = $"{message} {Plugin.Config.GetString("Qurre_reason", "Reason")}: {reason}";
                if (!IsVerified() || !targetPlayer.BypassMode)
                {
                    if (duration > 0)
                    {
                        var ev = new BanEvent(targetPlayer, issuerPlayer, duration, reason, message);
                        Qurre.Events.Player.ban(ev);
                        duration = ev.Duration;
                        reason = ev.Reason;
                        message = ev.FullMessage;
                        if (!ev.Allowed) return false;
                        string originalName = string.IsNullOrEmpty(targetPlayer.Nickname)
                            ? "(no nick)"
                            : targetPlayer.Nickname;
                        long issuanceTime = TimeBehaviour.CurrentTimestamp();
                        long banExpieryTime = TimeBehaviour.GetBanExpirationTime((uint)duration);
                        try
                        {
                            if (userId != null && !isGlobalBan)
                            {
                                BanHandler.IssueBan(
                                    new BanDetails
                                    {
                                        OriginalName = originalName,
                                        Id = userId,
                                        IssuanceTime = issuanceTime,
                                        Expires = banExpieryTime,
                                        Reason = reason,
                                        Issuer = issuer,
                                    }, BanHandler.BanType.UserId);
                                if (!string.IsNullOrEmpty(targetPlayer.UserId))
                                {
                                    BanHandler.IssueBan(
                                        new BanDetails
                                        {
                                            OriginalName = originalName,
                                            Id = targetPlayer.UserId,
                                            IssuanceTime = issuanceTime,
                                            Expires = banExpieryTime,
                                            Reason = reason,
                                            Issuer = issuer,
                                        }, BanHandler.BanType.UserId);
                                }
                            }
                        }
                        catch { return false; }
                        try
                        {
                            if (ConfigFile.ServerConfig.GetBool("ip_banning", false) || isGlobalBan)
                            {
                                BanHandler.IssueBan(
                                    new BanDetails
                                    {
                                        OriginalName = originalName,
                                        Id = address,
                                        IssuanceTime = issuanceTime,
                                        Expires = banExpieryTime,
                                        Reason = reason,
                                        Issuer = issuer,
                                    }, BanHandler.BanType.IP);
                            }
                        }
                        catch { return false; }
                    }
                    else if (duration == 0)
                    {
                        var ev = new KickEvent(targetPlayer, issuerPlayer, reason, message);
                        Qurre.Events.Player.kick(ev);
                        reason = ev.Reason;
                        message = ev.FullMessage;
                        if (!ev.Allowed) return false;
                    }
                }
                ServerConsole.Disconnect(targetPlayer.GameObject, message);
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Player [BanAndKick]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}