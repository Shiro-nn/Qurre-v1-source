using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.API.Addons
{
    public interface IPermission
    {
        [Description("Config name")]
        string Name { get; set; }
        List<ServerRoles> AllowedServerRoles { get; set; }
    }
}
