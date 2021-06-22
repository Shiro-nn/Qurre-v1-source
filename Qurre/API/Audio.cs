using System.IO;
using System.Net;
namespace Qurre.API
{
    public static class Audio
    {
        ///<summary>
        ///<para>Plays music from the stream.</para>
        ///<para>format - WaveFormat(48000, 1) /*(.ogg)*/</para>
        ///<para>Example:</para>
        ///<para>Play(new MemoryStream(audio), 1);</para>
        ///</summary>
        public static void Play(Stream stream, float volume) => QurreModLoader.Audio.Play(stream, volume);
        ///<summary>
        ///<para>Plays music from a file.</para>
        ///<para>format - WaveFormat(48000, 1) /*(.ogg)*/</para>
        ///<para>Example:</para>
        ///<para>PlayFromFile($"{PluginManager.PluginsDirectory}/audio/OmegaWarhead.raw", 1);</para>
        ///</summary>
        public static void PlayFromFile(string path, float volume) => QurreModLoader.Audio.Play(new FileStream(path, FileMode.Open), volume);
        ///<summary>
        ///<para>Plays music from a link.</para>
        ///<para>format - WaveFormat(48000, 1) /*(.ogg)*/</para>
        ///<para>Example:</para>
        ///<para>PlayFromUrl("https://cdn.scpsl.store/qurre/audio/OmegaWarhead.raw", 1);</para>
        ///</summary>
        public static void PlayFromUrl(string url, float volume)
        {
            using (var wc = new WebClient())
            {
                byte[] byteData = wc.DownloadData(url);
                QurreModLoader.Audio.Play(new MemoryStream(byteData), volume);
            }
        }
    }
}