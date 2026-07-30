using System.Threading;
using Cysharp.Threading.Tasks;
using Services.BridgeRelatedServices.Platform.Abstractions;

namespace Services.BridgeRelatedServices.Platform.Mock
{
    public class MockPlatformAuth : IPlatformAuth
    {
        public bool WasAuthorizedBefore => false;
        public bool ShouldAuthenticatePlayer => false;
        public bool IsAuthenticated => true;

        public UniTask AuthenticatePlayer(CancellationToken cancellationToken = default) => UniTask.CompletedTask;
    }
}
