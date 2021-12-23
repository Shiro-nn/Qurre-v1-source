using Qurre.API.Modules;
using System.Collections.Generic;
namespace Qurre.API.Addons
{
    public class ListConfigs
    {
        public IConfig this[int index] => Cache[index];
        private readonly List<IConfig> Cache = new();
        public bool Add(IConfig cfg)
        {
            if (Cache.Contains(cfg)) return false;
            Cache.Add(cfg);
            CustomConfigsManager.Load(cfg);
            return true;
        }
        public bool Remove(IConfig cfg)
        {
            if (!Cache.Contains(cfg)) return false;
            Cache.Remove(cfg);
            return true;
        }
        public bool Destroy(IConfig cfg)
        {
            if (!Cache.Contains(cfg)) return false;
            Cache.Remove(cfg);
            CustomConfigsManager.Destroy(cfg);
            return true;
        }
    }
}