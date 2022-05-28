﻿using Qurre.API.Addons.Audio;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Qurre.API
{
	public class Audio
	{
		public static List<Audio> Audios { get; private set; } = new();
		///<summary>
		///<para>Plays music from a file.</para>
		///<para>Example:</para>
		/// <example>
		/// <code>
		/// new Audio($"{PluginManager.PluginsDirectory}/Audio/OmegaWarhead.raw", 100, true, frameSize: 1920, sampleRate: 48000);
		/// </code>
		/// </example>
		///</summary>
		public Audio(string path, byte volume, bool instant = false, int frameSize = 1920, int sampleRate = 48000) :
			this(new FileStream(path, FileMode.Open), volume, instant, frameSize, sampleRate)
		{ }
		///<summary>
		///<para>Plays music from the stream.</para>
		///<para>Example:</para>
		/// <example>
		/// <code>
		/// new Audio(new MemoryStream(audio), 100, false, frameSize: 1920, sampleRate: 48000);
		/// </code>
		/// </example>
		///</summary>
		public Audio(Stream stream, byte volume, bool instant = false, int frameSize = 1920, int sampleRate = 48000)
		{
			Server.Host.ReferenceHub.nicknameSync.Network_displayName = "Qurre Audio";
			foreach (var channel in Radio.comms.PlayerChannels._openChannelsBySubId.Values.ToList())
			{
				Radio.comms.PlayerChannels.Close(channel);
				Radio.comms.PlayerChannels.Open(channel.TargetId, false, Dissonance.ChannelPriority.Default, 1);
			}
			Microphone = Radio.comms.gameObject.AddComponent<Microphone>().Create(stream, volume, frameSize, sampleRate, this);
			Radio.comms.IsMuted = false;
			if (instant && Audios.Count > 0)
			{
				Audios[0].Microphone.StopCapture();
				Audios[0] = this;
			}
			else
			{
				Audios.Add(this);
				if (Audios.FirstOrDefault() == this) Microphone.ResetMicrophone(Microphone.Name, true);
			}
		}
		public readonly IMicrophone Microphone;
		/*public static void PlayFromUrl(string url, float volume)
		{
			using WebClient _web = new();
			byte[] byteData = _web.DownloadData(url);
			Play(new MemoryStream(byteData), volume);
		}*/
	}
}