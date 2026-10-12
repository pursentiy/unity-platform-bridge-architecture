
namespace Core.Base
{
    public interface IDisposableService : IDisposeProvider
    {
        void InitializeService();
        void DisposeService();
    }
}