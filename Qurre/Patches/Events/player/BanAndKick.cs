using GameCore;
using HarmonyLib;
using Mirror;
using Qurre.API.Events;
using UnityEngine;
using Newtonsoft.Json.Linq;
namespace Qurre.Patches.Events.Player
{
    using Qurre.API;
    [HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.BanUser), new[] { typeof(GameObject), typeof(long), typeof(string), typeof(string), typeof(bool) })]
    internal static class BanAndKick
    {
        internal static string Banned { get; private set; } = "banned";
        internal static string Kicked { get; private set; } = "kicked";
        internal static string BanOrKick { get; private set; } = "You have been %bok%.";
        internal static string Reason { get; private set; } = "Reason";

        internal static void SetUpConfigs()
        {
            var par = Loader.Config.JsonArray["translation"];
            if (par is null)
            {
                par = JObject.Parse("{ }");
                Loader.Config.JsonArray["translation"] = par;
            }
            Banned = Loader.Config.SafeGetValue("Banned", "banned", source: par);
            Kicked = Loader.Config.SafeGetValue("Kicked", "kicked", source: par);
            BanOrKick = Loader.Config.SafeGetValue("BanOrKick", "You have been %bok%.", source: par);
            Reason = Loader.Config.SafeGetValue("Reason", "Reason", source: par);
        }

        private static bool Prefix(GameObject user, long duration, string reason, string issuer, bool isGlobalBan)
        {
            try
            {
                if (isGlobalBan && ConfigFile.ServerConfig.GetBool("gban_ban_ip", false))
                    duration = int.MaxValue;
                string userId = null;
                string address = user.GetComponent<NetworkIdentity>().connectionToClient.address;
                Player targetPlayer = Player.Get(user);
                Player issuerPlayer;
                if (issuer.Contains("(")) issuerPlayer = Player.Get(issuer.Substring(issuer.LastIndexOf('(') + 1).TrimEnd(')')) ?? Server.Host;
                else issuerPlayer = Server.Host;
                try { if (ConfigFile.ServerConfig.GetBool("online_mode", false)) userId = targetPlayer.UserId; }
                catch { return false; }
                string umm = (duration > 0) ? Banned : Kicked;
                string message = BanOrKick.Replace("%bok%", umm);
                if (!string.IsNullOrEmpty(reason)) message = $"{message} {Reason}: {reason}";
                if (duration > 0)
                {
                    var ev = new BanEvent(targetPlayer, issuerPlayer, duration, reason, message);
                    Qurre.Events.Invoke.Player.Ban(ev);
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
                    Qurre.Events.Invoke.Player.Kick(ev);
                    reason = ev.Reason;
                    message = ev.FullMessage;
                    if (!ev.Allowed) return false;
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