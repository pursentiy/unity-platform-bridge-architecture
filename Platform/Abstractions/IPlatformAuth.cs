using System.Threading;
using Cysharp.Threading.Tasks;

namespace Platform.Abstractions
{
    public interface IPlatformAuth
    {
        bool WasAuthorizedBefore { get; }
        bool ShouldAuthenticatePlayer { get; }
        bool IsAuthenticated { get; }

        UniTask AuthenticatePlayer(CancellationToken cancellationToken = default);
    }
}
