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
    internal class Instance
    {
        public List<IPermission> CustomPermissions { get; } = new();
        public static string PermissionConfigsDirectory { get; private set; } = Path.Combine(Qurre.PluginManager.ConfigsPath, "Permissions");
        public void Save(IPermission per)
        {
            var path = Path.Combine(PermissionConfigsDirectory, $"Permissions-{Server.Port}.yaml");
            string list_content = "";
            for (int i = 0; i < per.AllowedServerRoles.Count; i++) list_content += "-"+ per.AllowedServerRoles[i];
            File.WriteAllText(path, string.Concat($"{per.Name}:\n",
                    $"l{list_content}\n"));
        }
        public void Load()
        {
            if (!Directory.Exists(PermissionConfigsDirectory))
            {
                Log.Custom($"Permissions directory not found - creating: {PermissionConfigsDirectory}", "Warn", ConsoleColor.DarkYellow);
                Directory.CreateDirectory(PermissionConfigsDirectory);
            }
            var path = Path.Combine(PermissionConfigsDirectory, $"Permissions-{Server.Port}.yaml");
            if (!File.Exists(path))
            {
                File.Create(path);
                foreach (var per in CustomPermissions)
                {
                    Save(per);
                }
            }
        }
    }
}
