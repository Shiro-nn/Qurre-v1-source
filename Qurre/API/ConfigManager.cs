using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
namespace Qurre.API
{
    internal class ConfigManager
    {
        internal ConfigManager() => LoadConfigFile();
        internal List<string> RawData;
        private static List<string> Filter(IEnumerable<string> lines)
        {
            return (from line in lines
                    where !string.IsNullOrEmpty(line) && !line.StartsWith("#") && (line.StartsWith(" - ") || line.Contains(':'))
                    select line).ToList();
        }
		internal void Reload() => LoadConfigFile();
		internal void LoadConfigFile()
        {
            RemoveInvalid();
            RawData = Filter(FileManager.ReadAllLines(PluginManager.ConfigsPath));
        }
        private static void RemoveInvalid()
        {
            string[] array = FileManager.ReadAllLines(PluginManager.ConfigsPath);
            bool flag = false;
            for (int i = 0; i < array.Length; i++)
            {
                if (!array[i].StartsWith("#") && !array[i].StartsWith(" -") && !array[i].Contains(":") && !string.IsNullOrEmpty(array[i].Replace(" ", "")))
                {
                    flag = true;
                    array[i] = "#INVALID - " + array[i];
                }
            }
            if (flag) FileManager.WriteToFile(array, PluginManager.ConfigsPath, false);
        }



		public bool GetBool(string key, bool def = false)
		{
			string text = GetRawString(key).ToLower();
			if (text == "default") return def;
            if (bool.TryParse(text, out bool result)) return result;
            Log.Warn(key + " has invalid value, " + text + " is not a valid bool!");
			CommentInvalid(key, "BOOL");
			return def;
		}
		public byte GetByte(string key, byte def = 0)
		{
			string text = GetRawString(key).ToLower();
			if (text == "default") return def;
            if (byte.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out byte result)) return result;
            Log.Warn(key + " has an invalid value, " + text + " is not a valid byte!");
			CommentInvalid(key, "BYTE");
			return def;
		}
		public sbyte GetSByte(string key, sbyte def = 0)
		{
			string text = GetRawString(key).ToLower();
			if (text == "default") return def;
            if (sbyte.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out sbyte result)) return result;
            Log.Warn(key + " has an invalid value, " + text + " is not a valid signed byte!");
			CommentInvalid(key, "SBYTE");
			return def;
		}
		public char GetChar(string key, char def = ' ')
		{
			string rawString = GetRawString(key);
			if (rawString == "default") return def;
            if (char.TryParse(rawString, out char result)) return result;
            Log.Warn(key + " has an invalid value, " + rawString + " is not a valid char!");
			CommentInvalid(key, "CHAR");
			return def;
		}
		public decimal GetDecimal(string key, decimal def = 0m)
		{
			string text = GetRawString(key).ToLower();
			if (text == "default") return def;
            if (decimal.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result)) return result;
            Log.Warn(key + " has invalid value, " + text + " is not a valid decimal!");
			CommentInvalid(key, "DECIMAL");
			return def;
		}
		public double GetDouble(string key, double def = 0.0)
		{
			string text = GetRawString(key).ToLower();
			if (text == "default") return def;
            if (double.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double result)) return result;
            Log.Warn(key + " has invalid value, " + text + " is not a valid double!");
			CommentInvalid(key, "DOUBLE");
			return def;
		}
		public float GetFloat(string key, float def = 0f)
		{
			string text = GetRawString(key).ToLower();
			if (text == "default") return def;
            if (float.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out float result)) return result;
            Log.Warn(key + " has invalid value, " + text + " is not a valid float!");
			CommentInvalid(key, "FLOAT");
			return def;
		}
		public int GetInt(string key, int def = 0)
		{
			string text = GetRawString(key).ToLower();
			if (text == "default") return def;
            if (int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int result)) return result;
            Log.Warn(key + " has an invalid value, " + text + " is not a valid integer!");
			CommentInvalid(key, "INT");
			return def;
		}
		public uint GetUInt(string key, uint def = 0U)
		{
			string text = GetRawString(key).ToLower();
			if (text == "default") return def;
            if (uint.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out uint result)) return result;
            Log.Warn(key + " has an invalid value, " + text + " is not a valid unsigned integer!");
			CommentInvalid(key, "UINT");
			return def;
		}
		public long GetLong(string key, long def = 0L)
		{
			string text = GetRawString(key).ToLower();
			if (text == "default") return def;
            if (long.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out long result)) return result;
            Log.Warn(key + " has an invalid value, " + text + " is not a valid long!");
			CommentInvalid(key, "LONG");
			return def;
		}
		public ulong GetULong(string key, ulong def = 0UL)
		{
			string text = GetRawString(key).ToLower();
			if (text == "default") return def;
            if (ulong.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out ulong result)) return result;
            Log.Warn(key + " has an invalid value, " + text + " is not a valid unsigned long!");
			CommentInvalid(key, "ULONG");
			return def;
		}
		public short GetShort(string key, short def = 0)
		{
			string text = GetRawString(key).ToLower();
			if (text == "default") return def;
            if (short.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out short result)) return result;
            Log.Warn(key + " has an invalid value, " + text + " is not a valid short!");
			CommentInvalid(key, "SHORT");
			return def;
		}
		public ushort GetUShort(string key, ushort def = 0)
		{
			string text = GetRawString(key).ToLower();
			if (text == "default") return def;
            if (ushort.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out ushort result)) return result;
            Log.Warn(key + " has an invalid value, " + text + " is not a valid unsigned short!");
			CommentInvalid(key, "USHORT");
			return def;
		}
		public string GetString(string key, string def = "")
		{
			string rawString = GetRawString(key);
			if (rawString == "default") return def;
			return rawString.Replace("\\n", "\n");
		}
		public List<bool> GetBoolList(string key)
		{
			var DataBuffer = GetStringCollection(key);
			return DataBuffer.Select(new Func<string, bool>(bool.Parse)).ToList();
		}
		public List<byte> GetByteList(string key)
		{
			var DataBuffer = GetStringCollection(key);
			return DataBuffer.Select(new Func<string, byte>(byte.Parse)).ToList();
		}
		public List<sbyte> GetSByteList(string key)
		{
			var DataBuffer = GetStringCollection(key);
			return DataBuffer.Select(new Func<string, sbyte>(sbyte.Parse)).ToList();
		}
		public List<char> GetCharList(string key)
		{
			var DataBuffer = GetStringCollection(key);
			return DataBuffer.Select(new Func<string, char>(char.Parse)).ToList();
		}
		public List<decimal> GetDecimalList(string key)
		{
			var DataBuffer = GetStringCollection(key);
			return DataBuffer.Select(new Func<string, decimal>(decimal.Parse)).ToList();
		}
		public List<double> GetDoubleList(string key)
		{
			var DataBuffer = GetStringCollection(key);
			return DataBuffer.Select(new Func<string, double>(double.Parse)).ToList();
		}
		public List<float> GetFloatList(string key)
		{
			var DataBuffer = GetStringCollection(key);
			return DataBuffer.Select(new Func<string, float>(float.Parse)).ToList();
		}
		public List<int> GetIntList(string key)
		{
			var DataBuffer = GetStringCollection(key);
			return DataBuffer.Select(new Func<string, int>(int.Parse)).ToList();
		}
		public List<uint> GetUIntList(string key)
		{
			var DataBuffer = GetStringCollection(key);
			return DataBuffer.Select(new Func<string, uint>(uint.Parse)).ToList();
		}
		public List<long> GetLongList(string key)
		{
			var DataBuffer = GetStringCollection(key);
			return DataBuffer.Select(new Func<string, long>(long.Parse)).ToList();
		}
		public List<ulong> GetULongList(string key)
		{
			var DataBuffer = GetStringCollection(key);
			return DataBuffer.Select(new Func<string, ulong>(ulong.Parse)).ToList();
		}
		public List<short> GetShortList(string key)
		{
			var DataBuffer = GetStringCollection(key);
			return DataBuffer.Select(new Func<string, short>(short.Parse)).ToList();
		}
		public List<ushort> GetUShortList(string key)
		{
			var DataBuffer = GetStringCollection(key);
			return DataBuffer.Select(new Func<string, ushort>(ushort.Parse)).ToList();
		}
		public List<string> GetStringList(string key)
		{
			var DataBuffer = GetStringCollection(key);
			return DataBuffer;
		}
		public List<string> GetStringCollection(string _key)
		{
			string key = _key.ToLower();
			List<string> DataBuffer = new List<string>();
			foreach (string text in RawData)
			{
				if (text.ToLower().StartsWith(key) && text.TrimEnd(Array.Empty<char>()).EndsWith("[]")) break;
				if (text.ToLower().StartsWith(key + ":"))
				{
					string text2 = text.Substring(key.Length + 1).Trim();
					DataBuffer.Add(text2);
				}
				else
				{
					if (text.StartsWith(" - ")) DataBuffer.Add(text.Substring(3).TrimEnd(Array.Empty<char>()));
					else if (!text.StartsWith("#")) continue;
				}
			}
			return DataBuffer;
		}
		private void CommentInvalid(string key, string type)
		{
			for (int i = 0; i < RawData.Count; i++)
			{
				if (RawData[i].StartsWith(key + ": "))
				{
					RawData[i] = $"#INVALID {type} - {RawData[i]}";
				}
			}
			FileManager.WriteToFile(RawData, PluginManager.ConfigsPath, false);
		}
		public bool TryGetString(string key, out string value)
		{
			value = "";
			foreach (string text in RawData)
			{
				if (text.ToLower().StartsWith(key + ": "))
				{
					value = text.Substring(key.Length + 2);
					return true;
				}
			}
			return false;
		}
		private string GetRawString(string key)
		{
            if (!TryGetString(key, out string result))
            {
                return "default";
            }
            return result;
		}
	}
}