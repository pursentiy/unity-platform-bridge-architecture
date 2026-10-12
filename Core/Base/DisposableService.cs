using Core.Logging;


namespace Core.Base
{
    public abstract class DisposableService : IDisposableService
    {
        public DisposableCollection ChildDisposables { get; } = new DisposableCollection();

        public bool IsDisposed => _disposed;

        private bool _disposed;

        public void InitializeService()
        {
            _disposed = false;
            OnInitialize();
        }

        protected abstract void OnInitialize();

        public void DisposeService()
        {
            if (_disposed)
            {
                LoggerService.LogWarning($"A call to dispose service {GetType().Name} which has already been disposed");
                return;
            }

            OnDisposing();
            DisposableExtensions.HandledDispose(this);
            _disposed = true;
        }

        protected abstract void OnDisposing();
    }
}
