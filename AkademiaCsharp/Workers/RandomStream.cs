using System;
using System.IO;

namespace AkademiaCsharp.Workers
{
    public class RandomStream : Stream
    {
        private readonly int _seed;
        private Random _rng;
        private long _length;        
        private long _position;
        private bool _disposed = false;

        public RandomStream(int length, int seed)
        {
            _length = length;
            _seed = seed;
            Reset();
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length { get => _length; }

        public override long Position { get => _position; set => throw new NotSupportedException(); }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public void Reset()
        {
            _position = 0;
            _rng = new Random(_seed);
        }
        private long BytesLeft => Length - Position;

        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIfDisposed();

            if (BytesLeft == 0)
            {
                return 0;
            }

            checked
            {
                count = (int)Math.Min(count, BytesLeft);
            }

            _rng.NextBytes(buffer.AsSpan(offset, count));
            _position += count;

            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            _length = value;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("RandomStream");
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _disposed = true;
        }
    }
}
