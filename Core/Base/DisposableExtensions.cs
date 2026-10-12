namespace Core.Base
{
    public static class DisposableExtensions
    {
        public static void HandledDispose(IDisposeProvider provider)
        {
            provider.ChildDisposables.Dispose();
        }
    }
}
