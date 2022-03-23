
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
        public static bool GetAccess(Player target, IPermission permission)
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
        public static string GetAllPermissions(Player target)
        {
            string result = "";
            foreach (var per in Instance.CustomPermissions)
            {
                if (per.AllowedServerRoles.Contains(target.ServerRoles))
                {
                    result += $"{per.Name}\n";
                }
            }
            return result;
        }
    }
}
