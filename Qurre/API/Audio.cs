using Dissonance;
using Dissonance.Audio.Capture;
using Dissonance.Audio.Codecs;
using Dissonance.Integrations.MirrorIgnorance;
using Dissonance.Networking;
using Mirror;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using System.Linq;
namespace Qurre.API
{
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
			using (var wc = new WebClient())
			{
				byte[] byteData = wc.DownloadData(url);
				Play(new MemoryStream(byteData), volume);
			}
		}
		public static MirrorIgnoranceClient client;
		public static ClientInfo<MirrorConn> СlientInfo;
		private static void Play(Stream stream, ushort playerid, float volume)
		{
			MirrorIgnoranceCommsNetwork mirrorIgnoranceCommsNetwork = UnityEngine.Object.FindObjectOfType<MirrorIgnoranceCommsNetwork>();
			DissonanceComms dissonanceComms = UnityEngine.Object.FindObjectOfType<DissonanceComms>();
			if (mirrorIgnoranceCommsNetwork.Client == null) mirrorIgnoranceCommsNetwork.StartClient(Unit.None);
			client = mirrorIgnoranceCommsNetwork.Client;
			if (dissonanceComms.TryGetComponent<IMicrophoneCapture>(out IMicrophoneCapture microphoneCapture))
			{
				bool isRecording = microphoneCapture.IsRecording;
				if (isRecording) microphoneCapture.StopCapture();
				UnityEngine.Object.Destroy((Component)microphoneCapture);
			}
			mirrorIgnoranceCommsNetwork.Mode = NetworkMode.Host;
			MicrophoneModule floatArrayCapture = dissonanceComms.gameObject.AddComponent<MicrophoneModule>();
			floatArrayCapture._file = stream;
			dissonanceComms._capture.Start(mirrorIgnoranceCommsNetwork, floatArrayCapture);
			dissonanceComms._capture.MicrophoneName = "StreamedMic";
			СlientInfo = mirrorIgnoranceCommsNetwork.Server._clients.GetOrCreateClientInfo(playerid, "MusicBot", new CodecSettings(Codec.Opus, 48000U, 960), new MirrorConn(NetworkServer.localConnection));
			ClientInfo<MirrorConn> clientInfo = СlientInfo;
			dissonanceComms.IsMuted = false;
			KeyValuePair<ushort, RoomChannel>[] array = dissonanceComms.RoomChannels._openChannelsBySubId.ToArray();
			foreach (KeyValuePair<ushort, RoomChannel> keyValuePair in array)
			{
				dissonanceComms.RoomChannels.Close(keyValuePair.Value);
			}
			mirrorIgnoranceCommsNetwork.Server._clients.LeaveRoom("Null", clientInfo);
			mirrorIgnoranceCommsNetwork.Server._clients.LeaveRoom("Intercom", clientInfo);
			mirrorIgnoranceCommsNetwork.Server._clients.JoinRoom("Null", clientInfo);
			mirrorIgnoranceCommsNetwork.Server._clients.JoinRoom("Intercom", clientInfo);
			dissonanceComms.RoomChannels.Open("Null", false, ChannelPriority.High, volume);
			dissonanceComms.RoomChannels.Open("Intercom", false, ChannelPriority.High, volume);
		}
		public class MicrophoneModule : MonoBehaviour, IMicrophoneCapture
		{
			public bool IsRecording { get; private set; }
			public TimeSpan Latency { get; private set; }
			public WaveFormat StartCapture(string name)
			{
				WaveFormat format;
				if (_file == null || !_file.CanRead)
				{
					if (!_stopped && Log.debug)
					{
						Log.Error($"[Audio] _file==null: {_file == null}");
						if (_file != null) Log.Error($"[Audio] _file.CanRead=={_file.CanRead}");
					}
					IsRecording = false;
					Latency = TimeSpan.FromMilliseconds(0.0);
					format = _format;
				}
				else
				{
					_stopped = false;
					IsRecording = true;
					Latency = TimeSpan.FromMilliseconds(0.0);
					Log.Debug($"[Audio] Enabled: {name}");
					format = _format;
				}
				return format;
			}
			public void StopCapture()
			{
				IsRecording = false;
				Log.Debug("[Audio] Disabled");
				bool flag = _file != null;
				if (flag)
				{
					_file.Dispose();
					_file.Close();
				}
				_stopped = true;
				_file = null;
			}
			public void Subscribe(IMicrophoneSubscriber listener)
			{
				_subscribers.Add(listener);
			}
			public bool Unsubscribe(IMicrophoneSubscriber listener)
			{
				return _subscribers.Remove(listener);
			}
			public bool UpdateSubscribers()
			{
				bool flag = _file == null;
				bool result;
				if (flag)
				{
					result = true;
				}
				else
				{
					_elapsedTime += Time.unscaledDeltaTime;
					while (_elapsedTime > 0.02f)
					{
						_elapsedTime -= 0.02f;
						int num = _file.Read(_frameBytes, 0, _frameBytes.Length);
						_readOffset += num;
						Array.Clear(_frame, 0, _frame.Length);
						Buffer.BlockCopy(_frameBytes, 0, _frame, 0, num);
						foreach (IMicrophoneSubscriber microphoneSubscriber in _subscribers)
						{
							microphoneSubscriber.ReceiveMicrophoneData(new ArraySegment<float>(_frame), _format);
						}
					}
					result = false;
				}
				return result;
			}
			private readonly List<IMicrophoneSubscriber> _subscribers = new List<IMicrophoneSubscriber>();
			private readonly WaveFormat _format = new WaveFormat(48000, 1);
			private readonly float[] _frame = new float[960];
			private readonly byte[] _frameBytes = new byte[3840];
			private float _elapsedTime;
			public Stream _file;
			private int _readOffset;
			private bool _stopped = false;
		}
	}
}