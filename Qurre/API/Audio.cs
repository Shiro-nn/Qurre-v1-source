﻿using Qurre.API.Addons.Audio;
using Qurre.API.Addons.Audio.Extensions;
using System.IO;
namespace Qurre.API
{
	public static class Audio
	{
		///<summary>
		///<para>Plays music from a file.</para>
		///<para>Example:</para>
		/// <example>
		/// <code>
		/// Audio.PlayFromFile($"{PluginManager.PluginsDirectory}/Audio/OmegaWarhead.raw", 100, instant: true, loop: false, rate: 48000);
		/// </code>
		/// </example>
		///</summary>
		public static AudioTask PlayFromFile(string path, byte volume, bool instant = false, bool loop = false, int rate = 48000,
			string playerName = "Qurre Audio") => Play(new AudioStream(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read),
				rate), volume, instant, loop, playerName);
		///<summary>
		///<para>Plays music from a url.</para>
		///<para>Example:</para>
		/// <example>
		/// <code>
		/// Audio.PlayFromUrl("https://cdn.scpsl.store/qurre/audio/OmegaWarhead.raw", 100, instant: true, loop: false, rate: 48000);
		/// </code>
		/// </example>
		///</summary>
		public static AudioTask PlayFromUrl(string url, byte volume, bool instant = false, bool loop = false, int rate = 48000,
			string playerName = "Qurre Audio")
		{
			using System.Net.WebClient _web = new();
			byte[] byteData = _web.DownloadData(url);
			return Play(new AudioStream(new MemoryStream(byteData), rate), volume, instant, loop, playerName);
		}
		///<summary>
		///<para>Plays music from the stream.</para>
		///<para>Example:</para>
		/// <example>
		/// <code>
		/// Audio.PlayFromStream(new MemoryStream(audio), 100, instant: true, loop: false, rate: 48000);
		/// </code>
		/// </example>
		///</summary>
		public static AudioTask PlayFromStream(Stream stream, byte volume, bool instant = false, bool loop = false, int rate = 48000,
			string playerName = "Qurre Audio") => Play(new AudioStream(stream, rate), volume, instant, loop, playerName);

		///<summary>
		///<para>Plays music from the Audio Stream.</para>
		///<para>Example:</para>
		/// <example>
		/// <code>
		/// Audio.Play(new AudioStream(stream), 100, instant: true, loop: false);
		/// </code>
		/// </example>
		///</summary>
		public static AudioTask Play(IAudioStream stream, byte volume, bool instant = false, bool loop = false, string playerName = "Qurre Audio")
			=> Play(new(stream, volume, loop, playerName), instant);


		///<summary>
		///<para>Plays music from the Audio Task.</para>
		///<para>Example:</para>
		/// <example>
		/// <code>
		/// Audio.Play(new AudioTask(...), instant: true);
		/// </code>
		/// </example>
		///</summary>
		public static AudioTask Play(AudioTask task, bool instant = false)
		{
			if (_micro is null) _micro = Radio.comms.gameObject.AddComponent<Microphone>();
			if (instant && _micro._tasks.Count > 0)
			{
				_micro._tasks.Insert(1, task);
				_micro.StopPlay();
			}
			else
			{
				_micro._tasks.Add(task);
				if (_micro._tasks.Count == 1) _micro.ResetMicrophone();
			}
			return task;
		}
		internal static Microphone _micro;
		public static IMicrophone Microphone => _micro;
	}
}