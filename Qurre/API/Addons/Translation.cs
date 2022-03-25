using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.API.Addons
{
    public class Translation
    {
        public Translation(string text, TranslationType translationType = TranslationType.None, TranslationToolType toolType = TranslationToolType.None, string from = null, string to = null)
        {
            Text = text;
            Type = translationType;
            ToolType = toolType;
            fromLanguage = from;
            ToLanguage = to;
        }
        public string Return(Translation translation)
        {
            if (translation == null) return null;
            if (translation.Type == TranslationType.Word) return translation.Text.ToLower();
            if (translation.Type == TranslationType.Sentence) return translation.Text.ToLowerInvariant();
            if (translation.Type == TranslationType.None) return translation.Text;
            return null;
        }
        public string NetWorkReturn(Translation translation)
        {
            if (translation == null) return null;
            if (translation.ToolType == TranslationToolType.None) return null;
            return Translate(translation);
        }
        public string Translate(Translation translation)
        {
            WebRequest webRequest = null;
            WebResponse webResponse = null;
            string resJson = "";
            int textIndex = 0;
            int textLen = 0;
            if (translation.ToolType == TranslationToolType.None) return null;
            if (translation.ToolType == TranslationToolType.Google)
            {
                webRequest = WebRequest.Create($"http://ajax.googleapis.com/ajax/services/language/translate?v=1.0&langpair={translation.fromLanguage}|{translation.ToLanguage}&q={translation.Text} ");
                webResponse = webRequest.GetResponse();
                resJson = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                textIndex = resJson.IndexOf("translatedText") + 17;
                textLen = resJson.IndexOf("\"", textIndex) - textIndex;
                return resJson.Substring(textIndex, textLen);
            }
            return null;
        }
        public string Text { get; set; }
        public TranslationType Type { get; set; }
        public TranslationToolType ToolType { get; set; }
        public string fromLanguage { get; set; }
        public string ToLanguage { get; set; }
    }
    public enum TranslationType
    {
        None,
        Sentence,
        Word,
    }
    public enum TranslationToolType
    {
        None,
        Google,
    }
}
