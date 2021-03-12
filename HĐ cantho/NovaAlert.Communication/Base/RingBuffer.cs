using System;
using System.Threading;

namespace NovaAlert.Communication.Base
{
    /// <summary>
    /// Provides a thread-safe fixed-size read-write data buffer. 
    /// </summary>
    /// <remarks>
    ///   Reading from an empty buffer will wait until data is written. 
    ///   Writing to a full buffer will wait until data is read.
    /// </remarks>
    public sealed class RingBuffer : Disposable
    {
        /// <summary>
        /// The default buffer size.
        /// </summary>
        private const int DEFAULT_SIZE = 1024;

        /// <summary>
        /// Storage for the ring buffer contents.
        /// </summary>
        private readonly byte[] _Data;
        /// <summary>
        /// An instance lock object.
        /// </summary>
        private readonly object _ModificationKey;

        /// <summary>
        /// The readers gate.
        /// </summary>
        private readonly ManualResetEvent _HasData;

        /// <summary>
        /// The writers gate.
        /// </summary>
        private readonly ManualResetEvent _HasCapacity;
        /// <summary>
        /// Index for the head of the buffer.
        /// </summary>
        /// <remarks>
        ///   The index of the next byte to be <see cref="Write">written</see>.
        /// </remarks>
        private int _HeadIndex;

        /// <summary>
        /// Index for the tail of the buffer.
        /// </summary>
        /// <remarks>
        ///   The index of the next byte to be <see cref="Read">read</see>.
        /// </remarks>
        private int _TailIndex;

        /// <summary>
        /// The number of unread bytes in the buffer.
        /// </summary>
        private int _Count;
        /// <summary>
        /// Create a new RingBuffer with the default size.
        /// </summary>
        public RingBuffer() : this(DEFAULT_SIZE) { }

        /// <summary>
        /// Create a new RingBuffer with a specified size.
        /// </summary>
        /// <param name="size">The size of the ring buffer to create.</param>
        public RingBuffer(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException("size");
            }

            _Data = new byte[size];
            _ModificationKey = new object();

            // readers block until something is written
            _HasData = new ManualResetEvent(false);

            // writers may enter immediately
            _HasCapacity = new ManualResetEvent(true);
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~RingBuffer()
        {
            Dispose(false);
        }
        /// <summary>
        /// Gets the number of unread bytes in the buffer.
        /// </summary>
        public int Count
        {
            get { return _Count; }
        }

        /// <summary>
        /// Gets the number of unwritten bytes in the buffer.
        /// </summary>
        public int FreeCount
        {
            get { return _Data.Length - Count; }
        }

        /// <summary>
        /// Gets the total number of bytes in the buffer.
        /// </summary>
        public int Capacity
        {
            get { return _Data.Length; }
        }

        /// <summary>
        /// Gets a value indicating whether the buffer is empty or not.
        /// </summary>
        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the buffer is full or not.
        /// </summary>
        public bool IsFull
        {
            get { return (Count == _Data.Length); }
        }

        /// <summary>
        /// Gets a flag indicating if the buffer is closed.
        /// </summary>
        public bool IsClosed { get; private set; }
        /// <summary>
        /// Gets the element at the specified <paramref name="offset"/> from the tail index.
        /// </summary>
        /// <param name="offset">The zero-based index of the element to retrieve.</param>
        /// <returns>The requested element.</returns>
        /// <remarks>this[0] will return the same as ReadByte().</remarks>
        public byte this[int offset]
        {
            get
            {
                ThrowIfDisposed();
                if ((offset < 0) || (offset >= _Count))
                {
                    throw new ArgumentOutOfRangeException("offset");
                }
                return _Data[(_TailIndex + offset) % _Data.Length];
            }
        }
        /// <summary>
        /// Writes a byte to the head of the RingBuffer.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void WriteByte(byte value)
        {
            ThrowIfDisposed();

            if (IsClosed)
            {
                throw new ApplicationException("Buffer is closed.");
            }

            _HasCapacity.WaitOne();

            bool signalReaders;

            lock (_ModificationKey)
            {
                _Data[_HeadIndex] = value;
                _HeadIndex = (_HeadIndex + 1) % _Data.Length;

                signalReaders = IsEmpty;
                _Count++;
            }

            if (IsFull)
            {
                _HasCapacity.Reset();
            }

            if (signalReaders)
            {
                _HasData.Set();
            }
        }
        /// <summary>
        /// Reads a byte from the buffer.
        /// </summary>
        /// <returns>The next available byte, or zero (0) if it's empty.</returns>
        public byte ReadByte()
        {
            ThrowIfDisposed();

            byte value = 0;

            _HasData.WaitOne();

            if (!IsEmpty)
            {
                bool signalWriters;

                lock (_ModificationKey)
                {
                    value = _Data[_TailIndex];
                    _TailIndex = (_TailIndex + 1) % _Data.Length;

                    signalWriters = IsFull;
                    _Count--;
                }

                if (!IsClosed && (Count == 0))
                {
                    _HasData.Reset();
                }

                if (signalWriters)
                {
                    _HasCapacity.Set();
                }
            }

            return value;
        }
        /// <summary>
        /// Writes the specified data to the head of the RingBuffer.
        /// </summary>
        /// <param name="buffer">The input buffer.</param>
        /// <param name="index">The index of the input data.</param>
        /// <param name="count">The number of bytes to copy.</param>
        public void Write(byte[] buffer, int index, int count)
        {
            ThrowIfDisposed();

            if (IsClosed)
            {
                throw new ApplicationException("Buffer is closed");
            }

            while (count != 0)
            {
                _HasCapacity.WaitOne();

                bool signalReaders;

                // Gauranteed to not be full at this point, 
                //      however readers may still read from the buffer first.
                lock (_ModificationKey)
                {
                    int bytesToWrite = Math.Min(Capacity - Count, count);

                    signalReaders = (Count == 0);

                    while (0 != bytesToWrite--)
                    {
                        _Data[_HeadIndex] = buffer[index];
                        index++;

                        _HeadIndex = (_HeadIndex + 1) % _Data.Length;

                        count--;
                        _Count++;
                    }
                }

                if (IsFull)
                {
                    _HasCapacity.Reset();
                }

                if (signalReaders)
                {
                    _HasData.Set();
                }
            }
        }
        /// <summary>
        /// Reads data from the tail of the RingBuffer.
        /// </summary>
        /// <param name="buffer">The output buffer.</param>
        /// <param name="index">The index of the output data.</param>
        /// <param name="count">The number of bytes to copy.</param>
        /// <returns>The number of bytes read.</returns>
        public int Read(byte[] buffer, int index, int count)
        {
            ThrowIfDisposed();

            int result = 0;

            while (count != 0)
            {
                _HasData.WaitOne();

                if (IsEmpty)
                {
                    count = 0;

                }
                else
                {
                    lock (_ModificationKey)
                    {
                        int toRead = Math.Min(Count, count);

                        result += toRead;

                        bool signalWriters = IsFull;

                        while (0 != toRead--)
                        {
                            buffer[index] = _Data[_TailIndex];
                            index++;

                            _TailIndex = (_TailIndex + 1) % _Data.Length;

                            count--;
                            _Count--;
                        }

                        if (!IsClosed && (Count == 0))
                        {
                            _HasData.Reset();
                        }

                        if (signalWriters)
                        {
                            _HasCapacity.Set();
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Close the buffer for writing, and flush any waiting readers.
        /// </summary>
        /// <remarks>
        ///   A Read when the buffer is closed and there is no data will return -1.
        /// </remarks>
        public void Flush()
        {
            ThrowIfDisposed();
            IsClosed = true;
            _HasData.Set();
        }

        /// <summary>
        /// Clear the buffer contents, and reset the <see cref="IsClosed"/> status.
        /// </summary>
        public void Reset()
        {
            ThrowIfDisposed();

            _TailIndex = 0;
            _HeadIndex = 0;
            _Count = 0;

            Array.Clear(_Data, 0, _Data.Length);

            _HasData.Reset();
            _HasCapacity.Set();

            IsClosed = false;
        }

        /// <summary>
        /// This method disposes all <see cref="IDisposable"/> fields.
        /// </summary>
        protected override void DisposeManagedFields()
        {
            base.DisposeManagedFields();

            _HasCapacity.Close();
            ((IDisposable)_HasCapacity).Dispose();

            _HasData.Close();
            ((IDisposable)_HasData).Dispose();
        }

        public int IndexOf(byte value, int index = 0)
        {
            int i = index;
            while (i < this.Count)
            {
                if (this[i] == value)
                    return i;
                else
                    i++;
            }

            return -1;
        }
    }
}
