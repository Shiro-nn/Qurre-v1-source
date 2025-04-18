﻿using System;
using System.IO;
using NAudio.Wave;
namespace Qurre.API.Addons.Audio.Extensions
{
    public struct AudioStream : IAudioStream, IDisposable
    {
        private Stream _stream;
        private readonly string uid = Guid.NewGuid().ToString("N");

        public int Read(byte[] buffer, int offset, int count) => _stream.Read(buffer, offset, count);
        public bool CheckEnd() => _stream.Position == _stream.Length;

        public long Length => _stream.Length;
        public long Position
        {
            get => _stream.Position;
            set => _stream.Position = value;
        }
        public TimeSpan Duration => TimeSpan.FromSeconds(_stream.Length / 1920 * 4 * 0.04f);
        public TimeSpan Progression => TimeSpan.FromSeconds(_stream.Position / 1920 * 4 * 0.04f);
        public WaveFormat Format => _format;
        public int Rate => _rate;

        public bool Destroyed() => _destroyed;

        public AudioStream(Stream stream, int rate = 48000)
        {
            if (stream is null) throw new ArgumentNullException("Qurre Audio: Stream is null");
            if (!stream.CanRead)
            {
                Log.Error("Audio: Stream cannot be read stream");
                throw new ArgumentException("Qurre Audio: Stream cannot be read stream");
            }
            _stream = stream;
            _rate = rate;
            _format = new(rate, 1);
        }

        private readonly WaveFormat _format;
        private readonly int _rate;
        private bool _destroyed = false;

        public void Dispose()
        {
            if (_destroyed) return;

            if (_stream is not null)
            {
                _stream.Dispose();
                _stream = null;
            }

            _destroyed = true;

            GC.SuppressFinalize(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is not AudioStream other)
                return false;

            return this == other;
        }
        public static bool operator ==(AudioStream a, AudioStream b) => a.uid == b.uid;
        public static bool operator !=(AudioStream a, AudioStream b) => !(a == b);
        public override int GetHashCode() => Tuple.Create(uid).GetHashCode();
        public override string ToString()
            => $"Audio Stream: Length: \"{Length}\"; Rate: {Rate}; Format: {Format}";
    }
}