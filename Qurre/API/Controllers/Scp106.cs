using System.Collections.Generic;
using UnityEngine;
namespace Qurre.API.Controllers
{
    public class Scp106
    {
        internal Scp106(Player player) => this.player = player;
        private readonly Player player;
        private Scp106PlayerScript script => player.ClassManager.Scp106;
        public bool Is106 => player.Role == RoleType.Scp106;
        public Vector3 PortalPosition { get => script.NetworkportalPosition; set => script.SetPortalPosition(value); }
        public bool IsUsingPortal => script.goingViaThePortal;
        public HashSet<Player> PocketPlayers { get; } = new HashSet<Player>();
        public void CreatePortal() => script.CreatePortalInCurrentPosition();
        public void CreatePortal(Vector3 position)
        {
            script.CreatePortalInCurrentPosition();
            script.portalPosition = position;
        }
        public void UsePortal() => script.UseTeleport();
        public void DeletePortal() => script.DeletePortal();
        public void Contain() => script.Contain(player.ReferenceHub);
        public void CapturePlayer(Player player) => script.CallCmdMovePlayer(player.GameObject, ServerTime.time);
    }
}