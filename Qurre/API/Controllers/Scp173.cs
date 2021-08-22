using System.Collections.Generic;
namespace Qurre.API.Controllers
{
    public class Scp173
    {
        public HashSet<Player> IgnoredPlayers { get; internal set; } = new HashSet<Player>();
    }
}