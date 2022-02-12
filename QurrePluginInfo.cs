using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qurre
{
    public class QurrePluginInfo : Attribute
    {
        public string Name = "";
        public string Author = "";
        public string Description = "";
        public string Version = "";
        public string NeededVersion = "";
    }
}
