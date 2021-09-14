using Dissonance.Audio.Capture;
using Dissonance.Integrations.MirrorIgnorance;
using Dissonance.Networking;
using NAudio.Wave;
using System;
using System.IO;
using System.Net;
using UnityEngine;
namespace Qurre.API
{
	[Obsolete("Сurrently unavailable")]
	public static class Audio
	{
		///<summary>
		///<para>Plays music from the stream.</para>
		///<para>format - WaveFormat(48000, 1) /*(.ogg)*/</para>
		///<para>Example:</para>
		/// <example>
		/// <code>
		/// Play(new MemoryStream(audio), 1);
		/// </code>
		/// </example>
		///</summary>
		public static void Play(Stream stream, float volume) => Play(stream, 999, volume);
		///<summary>
		///<para>Plays music from a file.</para>
		///<para>format - WaveFormat(48000, 1) /*(.ogg)*/</para>
		///<para>Example:</para>
		/// <example>
		/// <code>
		/// PlayFromFile($"{PluginManager.PluginsDirectory}/audio/OmegaWarhead.raw", 1);
		/// </code>
		/// </example>
		///</summary>
		public static void PlayFromFile(string path, float volume) => Play(new FileStream(path, FileMode.Open), volume);
		///<summary>
		///<para>Plays music from a link.</para>
		///<para>format - WaveFormat(48000, 1) /*(.ogg)*/</para>
		///<para>Example:</para>
		/// <example>
		/// <code>
		/// PlayFromUrl("https://cdn.scpsl.store/qurre/audio/OmegaWarhead.raw", 1);
		/// </code>
		/// </example>
		///</summary>
		public static void PlayFromUrl(string url, float volume)
		{
            using var wc = new WebClient();
            byte[] byteData = wc.DownloadData(url);
            Play(new MemoryStream(byteData), volume);
        }
		public static MirrorIgnoranceClient client;
		public static ClientInfo<MirrorConn> СlientInfo;
		private static void Play(Stream stream, ushort playerid, float volume)
		{
		}
		public class MicrophoneModule : MonoBehaviour, IMicrophoneCapture
		{
			public bool IsRecording { get; private set; }
			public TimeSpan Latency { get; private set; }
			public WaveFormat StartCapture(string name)
			{
				return new WaveFormat(48000, 1);
			}
			public void StopCapture()
			{
			}
			public void Subscribe(IMicrophoneSubscriber listener)
			{
			}
			public bool Unsubscribe(IMicrophoneSubscriber listener)
			{
				return false;
			}
			public bool UpdateSubscribers()
			{
				return false;
			}
			public Stream _file;
		}
	}
}