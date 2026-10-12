using System.Threading;
using Cysharp.Threading.Tasks;
using Platform.Abstractions;

namespace Platform.Mocks
{
    public class MockPlatformSocial : IPlatformSocial
    {
        public bool IsShareSupported => true;
        public bool IsInviteSupported => true;

        public UniTask<bool> ShareResult(string shareMessage, CancellationToken cancellationToken) => UniTask.FromResult(false);
        public UniTask<bool> InviteFriends(CancellationToken cancellationToken) => UniTask.FromResult(false);
    }
}
