using System;
using System.Threading;

namespace NovaAlert.Communication.Base
{
    /// <summary>
    /// Provides a base implementation of <see cref="IDisposable"/>.
    /// </summary>
    /// <remarks>
    /// 
    /// Derived classes that need finalizers should implement the following
    ///  AND be sealed.
    /// 
    ///    /// <summary>
    ///    /// Finalizer.
    ///    /// </summary>
    ///    ~MyClass() {
    ///       Dispose(false);
    ///    }
    /// 
    /// Derived classes that need to protect against access of disposed state
    ///  can use the following example:
    /// 
    ///    /// <summary>
    ///    /// Member accessing potentially disposed state.
    ///    /// </summary>
    ///    public void MyMethod() {
    ///       ThrowIfDisposed();
    ///       // ... 
    ///    }
    /// 
    /// </remarks>
    public abstract class Disposable : IDisposable
    {
        /// <summary>
        /// "Not Disposed" state.
        /// </summary>
        private const int NOT_DISPOSED = 0;

        /// <summary>
        /// "Disposing" state.
        /// </summary>
        private const int DISPOSING = 1;

        /// <summary>
        /// "Disposed" state.
        /// </summary>
        private const int DISPOSED = 2;

        /// <summary>
        /// A flag to indicate state of disposal.
        /// </summary>
        private int _State = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected Disposable() { }

        /// <summary>
        /// <c>true</c> if the instance has been disposed; otherwise, <c>false</c>.
        /// </summary>
        public bool IsDisposed
        {
            get { return Thread.VolatileRead(ref _State) != NOT_DISPOSED; }
        }

        /// <summary>
        /// Occurs when the instance is being disposed.
        /// </summary>
        public event EventHandler Disposing;

        /// <summary>
        /// Occurs when the instance has been disposed.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Notifies listeners that the instance is being disposed.
        /// </summary>
        protected virtual void OnDisposing()
        {
            if (Disposing != null)
            {
                Disposing.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Notifies listeners that the instance has been disposed.
        /// </summary>
        protected virtual void OnDisposed()
        {
            if (Disposed != null)
            {
                Disposed.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        ///  releasing, or resetting resources held by the current instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes the instance.
        /// </summary>
        /// <param name="disposing"><c>true</c> to indicate explicit
        ///  cleanup (i.e. <see cref="IDisposable.Dispose"/>()), otherwise
        ///  <c>false</c> (i.e. implicit cleanup from finalizer)</param>
        protected void Dispose(bool disposing)
        {
            // if( _State == NOT_DISPOSED ) {
            if (Interlocked.CompareExchange(ref _State,
                                                DISPOSING,
                                                NOT_DISPOSED) == NOT_DISPOSED)
            {
                // _State = DISPOSING;

                if (disposing)
                {
                    try
                    {
                        OnDisposing();
                    }
                    catch
                    {
                        // intentional suppression
                    }
                    try
                    {
                        DisposeManagedFields();
                    }
                    catch
                    {
                        // intentional suppression
                    }
                    try
                    {
                        RemoveDelegates();
                    }
                    catch
                    {
                        // intentional suppression
                    }
                }

                try
                {
                    ReleaseUnmanagedResources();
                }
                catch
                {
                    // intentional suppression
                }

                Interlocked.Increment(ref _State); // _State = DISPOSED;

                if (disposing)
                {
                    try
                    {
                        GC.SuppressFinalize(this);
                    }
                    catch
                    {
                        // intentional suppression
                    }
                    try
                    {
                        OnDisposed();
                    }
                    catch
                    {
                        // intentional suppression
                    }
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, this method should dispose
        ///  all <see cref="IDisposable"/> fields.
        /// </summary>
        protected virtual void DisposeManagedFields() { }

        /// <summary>
        /// When overridden in a derived class, this method should remove
        ///  all delegate invocations and references.
        /// </summary>
        protected virtual void RemoveDelegates() { }

        /// <summary>
        /// When overridden in a derived class, this method should clean up
        ///  any unmanaged resources.
        /// </summary>
        protected virtual void ReleaseUnmanagedResources() { }

        /// <summary>
        /// Derived classes may use this method to prevent member access
        ///  on a disposed instance.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName,
                   @"Access to a disposed object is not allowed.");
            }
        }
    }
}
