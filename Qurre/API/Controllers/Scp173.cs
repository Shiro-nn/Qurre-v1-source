using System.Collections.Generic;
namespace Qurre.API.Controllers
{
    public class Scp173
    {
        internal Scp173(Player _) => player = _;
        private Player player;
        public HashSet<Player> IgnoredPlayers { get; internal set; } = new HashSet<Player>();
        public HashSet<Player> ConfrontingPlayers { get; internal set; } = new HashSet<Player>();
    }
}