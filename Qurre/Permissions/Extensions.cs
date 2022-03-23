
using Qurre.API;
using Qurre.API.Addons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.Permissions
{
    public static class Extensions
    {
        public static bool GetAccess(this Player target, IPermission permission)
        {
            bool result;
            if (permission.AllowedServerRoles.Contains(target.ServerRoles))
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
    }
}
