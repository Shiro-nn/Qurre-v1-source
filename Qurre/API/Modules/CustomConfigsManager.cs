﻿using Qurre.API.Addons;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.NodeDeserializers;
namespace Qurre.API.Modules
{
    internal static class CustomConfigsManager
    {
        internal static IDeserializer Deserializer { get; } = new DeserializerBuilder()
            .WithTypeConverter(new VectorConvert())
            .WithNamingConvention(NullNamingConvention.Instance)
            .WithNodeDeserializer(inner => new NodeDeserializer(inner), deserializer => deserializer.InsteadOf<ObjectNodeDeserializer>())
            .IgnoreFields()
            .IgnoreUnmatchedProperties()
            .Build();
        internal static ISerializer Serializer { get; } = new SerializerBuilder()
            .WithTypeConverter(new VectorConvert())
            .WithTypeInspector(inner => new TypeInspector(inner))
            .WithEmissionPhaseObjectGraphVisitor(args => new CommentsOGV(args.InnerVisitor))
            .WithNamingConvention(NullNamingConvention.Instance)
            .IgnoreFields()
            .Build();
        internal static void Destroy(IConfig cfg)
        {
            var path = Path.Combine(PluginManager.CustomConfigsDirectory, $"{cfg.Name}.yaml");
            if (File.Exists(path)) File.Delete(path);
        }
        internal static void Load(IConfig cfg)
        {
            if (!Directory.Exists(PluginManager.CustomConfigsDirectory))
            {
                Log.Custom($"Custom configs directory not found - creating: {PluginManager.CustomConfigsDirectory}", "Warn", System.ConsoleColor.DarkYellow);
                Directory.CreateDirectory(PluginManager.CustomConfigsDirectory);
            }
            var path = Path.Combine(PluginManager.CustomConfigsDirectory, $"{cfg.Name}-{Server.Port}.yaml");
            if (!File.Exists(path))
            {
                File.Create(path).Close();
                if (File.Exists(Path.Combine(PluginManager.CustomConfigsDirectory, $"{cfg.Name}.yaml")))
                {
                    string staticText = File.ReadAllText(path);
                    File.WriteAllText(path, staticText);
                }
            }
            string text = File.ReadAllText(path);
            var _ = (IConfig)Deserializer.Deserialize(text, cfg.GetType());
            if (_ != null) cfg.CopyProperties(_);
            Save(cfg);
        }
        internal static void Save(IConfig cfg)
        {
            string data = Serializer.Serialize(cfg);
            var path = Path.Combine(PluginManager.CustomConfigsDirectory, $"{cfg.Name}.yaml");
            if (!File.Exists(path)) return;
            File.WriteAllText(path, data ?? string.Empty);
        }
    }
}