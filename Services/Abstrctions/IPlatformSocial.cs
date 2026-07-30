using System.Threading;
using Cysharp.Threading.Tasks;

namespace Services.BridgeRelatedServices.Platform.Abstractions
{
    public interface IPlatformSocial
    {
        bool IsShareSupported { get; }
        bool IsInviteSupported { get; }

        UniTask<bool> ShareResult(string shareMessage, CancellationToken cancellationToken);
        UniTask<bool> InviteFriends(CancellationToken cancellationToken);
    }
}
