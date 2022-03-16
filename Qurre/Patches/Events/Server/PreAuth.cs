using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using LiteNetLib;
using LiteNetLib.Utils;
using Qurre.API.Events;
namespace Qurre.Patches.Events.Server
{
    [HarmonyPatch(typeof(CustomLiteNetLib4MirrorTransport), nameof(CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest))]
    internal static class PreAuth
    {
        private static void Postfix(ConnectionRequest request)
        {
            try
            {
                if (!request.Data.EndOfData) return;
                string userId = "";
                if (CustomLiteNetLib4MirrorTransport.UserIds.ContainsKey(request.RemoteEndPoint))
                    userId = CustomLiteNetLib4MirrorTransport.UserIds[request.RemoteEndPoint].UserId;

                var ev = new PreAuthEvent(userId, request, "No Reason");
                Qurre.Events.Invoke.Server.PreAuth(ev);
                if (ev.Allowed)
                {
                    request.Accept();
                    return;
                }
                var data = new NetDataWriter();
                data.Put((byte)10);
                data.Put(ev.Reason);
                request.RejectForce(data);
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Server -> [PreAuth]:\n{e}\n{e.StackTrace}");
                request.Accept();
            }
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            foreach (var code in codes.Select((x, i) => new { Value = x, Index = i }))
            {
                if (code.Value.opcode != OpCodes.Callvirt) continue;
                if (codes[code.Index + 2].opcode != OpCodes.Ldstr) continue;
                if (codes[code.Index + 2].operand as string == "Player {0} preauthenticated from endpoint {1}.") code.Value.opcode = OpCodes.Nop;
            }
            return codes.AsEnumerable();
        }
    }
}