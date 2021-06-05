using System.IO;
using System.Net;

namespace Qurre.API
{
    public static class Audio
    {
        public static void Play(Stream stream, float volume) => QurreModLoader.Audio.Play(stream, volume);
        public static void PlayFromFile(string path, float volume)
        {
            FileStream stream = File.OpenRead(path);
            QurreModLoader.Audio.Play(stream, volume);
        }
        public static void PlayFromUrl(string url, float volume)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            QurreModLoader.Audio.Play(stream, volume);
        }
    }
}