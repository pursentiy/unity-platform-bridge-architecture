using System.Threading;
using Cysharp.Threading.Tasks;

namespace Services.BridgeRelatedServices.Platform.Abstractions
{
    public interface IPlatformAuth
    {
        bool WasAuthorizedBefore { get; }
        bool ShouldAuthenticatePlayer { get; }
        bool IsAuthenticated { get; }

        UniTask AuthenticatePlayer(CancellationToken cancellationToken = default);
    }
}
