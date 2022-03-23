using Qurre.API;
using Qurre.API.Addons;
using Qurre.API.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.NodeDeserializers;

namespace Qurre.Permissions
{
    public class Instance
    {
        public static HashSet<IPermission> CustomPermissions { get; } = new();
        public bool Register(IPermission permission)
        {
            if (permission == null)
            {
                Log.Error("Failed to register a null permission");
                return false;
            }

            if (CustomPermissions.Add(permission))
            {
                Log.Error("This permission is already to register");
                return false;
            }

            CustomPermissions.Add(permission);
            Log.Debug($"IPermission-{permission.Name} has been register!");
            return true;
        }
        public bool Unregister()
        {
            if (CustomPermissions.Count > 0)
            {
                CustomPermissions.Clear();
                Log.Debug("All Permissions have been destory");
                return true;
            }
            Log.Error("There isn't a ipermission in CustomPermissions HashSet");
            return false;
        }
        public IPermission Get(string name)
        {
            IPermission permission1 = default;
            foreach (var permission in CustomPermissions)
            {
                if (permission.Name == name) permission1 = permission;
            }
            return permission1;
        }
    }
}