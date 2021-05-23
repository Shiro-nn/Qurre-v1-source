using Qurre.API.Objects;
using System.IO;
using System.Text;
namespace Qurre.API
{
    public class Config
    {
        internal ConfigManager ConfigManager { get; private set; }
        internal Config() => ConfigManager = new ConfigManager();
        public void Reload() => ConfigManager.Reload();
        public bool GetBool(string key, bool default_value, string comment = "") => (bool)Get(ConfigObjects.Bool, key, default_value, comment);
        public byte GetByte(string key, byte default_value, string comment = "") => (byte)Get(ConfigObjects.Byte, key, default_value, comment);
        public char GetChar(string key, char default_value, string comment = "") => (char)Get(ConfigObjects.Char, key, default_value, comment);
        public decimal GetDecimal(string key, decimal default_value, string comment = "") => (decimal)Get(ConfigObjects.Decimal, key, default_value, comment);
        public double GetDouble(string key, double default_value, string comment = "") => (double)Get(ConfigObjects.Double, key, default_value, comment);
        public float GetFloat(string key, float default_value, string comment = "") => (float)Get(ConfigObjects.Float, key, default_value, comment);
        public int GetInt(string key, int default_value, string comment = "") => (int)Get(ConfigObjects.Int, key, default_value, comment);
        public long GetLong(string key, long default_value, string comment = "") => (long)Get(ConfigObjects.Long, key, default_value, comment);
        public sbyte GetSByte(string key, sbyte default_value, string comment = "") => (sbyte)Get(ConfigObjects.Sbyte, key, default_value, comment);
        public short GetShort(string key, short default_value, string comment = "") => (short)Get(ConfigObjects.Short, key, default_value, comment);
        public string GetString(string key, string default_value, string comment = "") => (string)Get(ConfigObjects.String, key, default_value, comment);
        public uint GetUInt(string key, uint default_value, string comment = "") => (uint)Get(ConfigObjects.Uint, key, default_value, comment);
        public ulong GetULong(string key, ulong default_value, string comment = "") => (ulong)Get(ConfigObjects.Ulong, key, default_value, comment);
        public ushort GetUShort(string key, ushort default_value, string comment = "") => (ushort)Get(ConfigObjects.Ushort, key, default_value, comment);
        public object Get(ConfigObjects obj, string key, object def, string comment = "")
        {
            string _key = key;
            if (obj == ConfigObjects.Bool)
            {
                var _list = ConfigManager.GetBoolList(_key);
                if (_list == null || _list.Count < 1) WriteCfg(_key, def, comment);
                return ConfigManager.GetBool(_key, (bool)def);
            }
            else if (obj == ConfigObjects.Byte)
            {
                var _list = ConfigManager.GetByteList(_key);
                if (_list == null || _list.Count < 1) WriteCfg(_key, def, comment);
                return ConfigManager.GetByte(_key, (byte)def);
            }
            else if (obj == ConfigObjects.Char)
            {
                var _list = ConfigManager.GetCharList(_key);
                if (_list == null || _list.Count < 1) WriteCfg(_key, def, comment);
                return ConfigManager.GetChar(_key, (char)def);
            }
            else if (obj == ConfigObjects.Decimal)
            {
                var _list = ConfigManager.GetDecimalList(_key);
                if (_list == null || _list.Count < 1) WriteCfg(_key, def, comment);
                return ConfigManager.GetDecimal(_key, (decimal)def);
            }
            else if (obj == ConfigObjects.Double)
            {
                var _list = ConfigManager.GetDoubleList(_key);
                if (_list == null || _list.Count < 1) WriteCfg(_key, def, comment);
                return ConfigManager.GetDouble(_key, (double)def);
            }
            else if (obj == ConfigObjects.Float)
            {
                var _list = ConfigManager.GetFloatList(_key);
                if (_list == null || _list.Count < 1) WriteCfg(_key, def, comment);
                return ConfigManager.GetFloat(_key, (float)def);
            }
            else if (obj == ConfigObjects.Int)
            {
                var _list = ConfigManager.GetIntList(_key);
                if (_list == null || _list.Count < 1) WriteCfg(_key, def, comment);
                return ConfigManager.GetInt(_key, (int)def);
            }
            else if (obj == ConfigObjects.Long)
            {
                var _list = ConfigManager.GetLongList(_key);
                if (_list == null || _list.Count < 1) WriteCfg(_key, def, comment);
                return ConfigManager.GetLong(_key, (long)def);
            }
            else if (obj == ConfigObjects.Sbyte)
            {
                var _list = ConfigManager.GetSByteList(_key);
                if (_list == null || _list.Count < 1) WriteCfg(_key, def, comment);
                return ConfigManager.GetSByte(_key, (sbyte)def);
            }
            else if (obj == ConfigObjects.Short)
            {
                var _list = ConfigManager.GetShortList(_key);
                if (_list == null || _list.Count < 1) WriteCfg(_key, def, comment);
                return ConfigManager.GetShort(_key, (short)def);
            }
            else if (obj == ConfigObjects.String)
            {
                var _list = ConfigManager.GetStringList(_key);
                if (_list == null || _list.Count < 1) WriteCfg(_key, def, comment);
                if (_key == "qurre_database") return "";
                return ConfigManager.GetString(_key, (string)def);
            }
            else if (obj == ConfigObjects.Uint)
            {
                var _list = ConfigManager.GetUIntList(_key);
                if (_list == null || _list.Count < 1) WriteCfg(_key, def, comment);
                return ConfigManager.GetUInt(_key, (uint)def);
            }
            else if (obj == ConfigObjects.Ulong)
            {
                var _list = ConfigManager.GetULongList(_key);
                if (_list == null || _list.Count < 1) WriteCfg(_key, def, comment);
                return ConfigManager.GetULong(_key, (ulong)def);
            }
            else if (obj == ConfigObjects.Ushort)
            {
                var _list = ConfigManager.GetUShortList(_key);
                if (_list == null || _list.Count < 1) WriteCfg(_key, def, comment);
                return ConfigManager.GetUShort(_key, (ushort)def);
            }
            else return null;
        }
        private void WriteCfg(string key, object def, string comment = "")
        {
            string _com = comment.Trim();
            using (StreamWriter sw = new StreamWriter(PluginManager.ConfigsPath, true, Encoding.Default))
            {
                if (def.ToString() == "True") def = "true";
                if (def.ToString() == "False") def = "false";
                if (_com == "") sw.WriteLine($"{key}: {def}");
                else sw.WriteLine($"#{_com}\n{key}: {def}");
                ConfigManager.RawData.Add($"{key}: {def}");
            }
        }
    }
}