using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text;
namespace Qurre.API.Modules
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



		internal bool GetBool(string key, bool def = false, string comment = "")
		{
			string text = GetRawString(key);
			if (text == "default")
			{
				WriteCfg(key, def, comment);
				return def;
			}
			if (bool.TryParse(text, out bool result)) return result;
			Log.Warn(key + " has invalid value, " + text + " is not a valid bool!");
			CommentInvalid(key, "BOOL");
			WriteCfg(key, def, comment);
			return def;
		}
		internal byte GetByte(string key, byte def = 0, string comment = "")
		{
			string text = GetRawString(key);
			if (text == "default")
			{
				WriteCfg(key, def, comment);
				return def;
			}
			if (byte.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out byte result)) return result;
			Log.Warn(key + " has an invalid value, " + text + " is not a valid byte!");
			CommentInvalid(key, "BYTE");
			WriteCfg(key, def, comment);
			return def;
		}
		internal sbyte GetSByte(string key, sbyte def = 0, string comment = "")
		{
			string text = GetRawString(key);
			if (text == "default")
			{
				WriteCfg(key, def, comment);
				return def;
			}
			if (sbyte.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out sbyte result)) return result;
			Log.Warn(key + " has an invalid value, " + text + " is not a valid signed byte!");
			CommentInvalid(key, "SBYTE");
			WriteCfg(key, def, comment);
			return def;
		}
		internal char GetChar(string key, char def = ' ', string comment = "")
		{
			string text = GetRawString(key);
			if (text == "default")
			{
				WriteCfg(key, def, comment);
				return def;
			}
			if (char.TryParse(text, out char result)) return result;
			Log.Warn(key + " has an invalid value, " + text + " is not a valid char!");
			CommentInvalid(key, "CHAR");
			WriteCfg(key, def, comment);
			return def;
		}
		internal decimal GetDecimal(string key, decimal def = 0m, string comment = "")
		{
			string text = GetRawString(key);
			if (text == "default")
			{
				WriteCfg(key, def, comment);
				return def;
			}
			if (decimal.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result)) return result;
			Log.Warn(key + " has invalid value, " + text + " is not a valid decimal!");
			CommentInvalid(key, "DECIMAL");
			WriteCfg(key, def, comment);
			return def;
		}
		internal double GetDouble(string key, double def = 0.0, string comment = "")
		{
			string text = GetRawString(key);
			if (text == "default")
			{
				WriteCfg(key, def, comment);
				return def;
			}
			if (double.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double result)) return result;
			Log.Warn(key + " has invalid value, " + text + " is not a valid double!");
			CommentInvalid(key, "DOUBLE");
			WriteCfg(key, def, comment);
			return def;
		}
		internal float GetFloat(string key, float def = 0f, string comment = "")
		{
			string text = GetRawString(key);
			if (text == "default")
			{
				WriteCfg(key, def, comment);
				return def;
			}
			if (float.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out float result)) return result;
			Log.Warn(key + " has invalid value, " + text + " is not a valid float!");
			CommentInvalid(key, "FLOAT");
			WriteCfg(key, def, comment);
			return def;
		}
		internal int GetInt(string key, int def = 0, string comment = "")
		{
			string text = GetRawString(key);
			if (text == "default")
			{
				WriteCfg(key, def, comment);
				return def;
			}
			if (int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int result)) return result;
			Log.Warn(key + " has an invalid value, " + text + " is not a valid integer!");
			CommentInvalid(key, "INT");
			WriteCfg(key, def, comment);
			return def;
		}
		internal uint GetUInt(string key, uint def = 0U, string comment = "")
		{
			string text = GetRawString(key);
			if (text == "default")
			{
				WriteCfg(key, def, comment);
				return def;
			}
			if (uint.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out uint result)) return result;
			Log.Warn(key + " has an invalid value, " + text + " is not a valid unsigned integer!");
			CommentInvalid(key, "UINT");
			WriteCfg(key, def, comment);
			return def;
		}
		internal long GetLong(string key, long def = 0L, string comment = "")
		{
			string text = GetRawString(key);
			if (text == "default")
			{
				WriteCfg(key, def, comment);
				return def;
			}
			if (long.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out long result)) return result;
			Log.Warn(key + " has an invalid value, " + text + " is not a valid long!");
			CommentInvalid(key, "LONG");
			WriteCfg(key, def, comment);
			return def;
		}
		internal ulong GetULong(string key, ulong def = 0UL, string comment = "")
		{
			string text = GetRawString(key);
			if (text == "default")
			{
				WriteCfg(key, def, comment);
				return def;
			}
			if (ulong.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out ulong result)) return result;
			Log.Warn(key + " has an invalid value, " + text + " is not a valid unsigned long!");
			CommentInvalid(key, "ULONG");
			WriteCfg(key, def, comment);
			return def;
		}
		internal short GetShort(string key, short def = 0, string comment = "")
		{
			string text = GetRawString(key);
			if (text == "default")
			{
				WriteCfg(key, def, comment);
				return def;
			}
			if (short.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out short result)) return result;
			Log.Warn(key + " has an invalid value, " + text + " is not a valid short!");
			CommentInvalid(key, "SHORT");
			WriteCfg(key, def, comment);
			return def;
		}
		internal ushort GetUShort(string key, ushort def = 0, string comment = "")
		{
			string text = GetRawString(key);
			if (text == "default")
			{
				WriteCfg(key, def, comment);
				return def;
			}
			if (ushort.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out ushort result)) return result;
			Log.Warn(key + " has an invalid value, " + text + " is not a valid unsigned short!");
			CommentInvalid(key, "USHORT");
			WriteCfg(key, def, comment);
			return def;
		}
		internal string GetString(string key, string def = "", string comment = "")
		{
			string rawString = GetRawString(key);
			if (rawString == "default")
			{
				WriteCfg(key, def, comment);
				return def;
			}
			if (key.ToLower() == "qurre_database") return def;
			return rawString.Replace("\\n", "\n");
		}
		internal string GetDataBase(string key)
		{
			string rawString = GetRawString(key);
			if (rawString == "default") return "";
			return rawString.Replace("\\n", "\n");
		}
		private void WriteCfg(string key, object def, string comment = "")
		{
			string _com = comment.Trim().Replace("\n", "\n#");
			string _def = def.ToString().Replace("\n", "\\n");
			using StreamWriter sw = new StreamWriter(PluginManager.ConfigsPath, true, Encoding.Default);
			if (_def == "True") _def = "true";
			if (_def == "False") _def = "false";
			if (_com == "") sw.WriteLine($"{key}: {_def}");
			else sw.WriteLine($"#{_com}\n{key}: {_def}");
			RawData.Add($"{key}: {_def}");
			sw.Close();
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
		private bool TryGetString(string key, out string value)
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
			var _key = key.ToLower();
			if (!TryGetString(_key, out string result))
			{
				return "default";
			}
			return result;
		}
	}
}