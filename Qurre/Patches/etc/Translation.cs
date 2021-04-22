using HarmonyLib;
namespace Qurre.Patches.etc
{
    [HarmonyPatch(typeof(TranslationReader), "LoadTranslation")]
    internal static class Translation
    {
        private static void Postfix(string translationPath)
        {
            TranslationReader.path = translationPath;
        }
    }
}