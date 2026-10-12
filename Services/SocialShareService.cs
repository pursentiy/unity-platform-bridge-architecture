using Core.Events;
using System.Threading;
using Cysharp.Threading.Tasks;
using Platform.Abstractions;
using Core.Base;
using Services.Overlay;

namespace Services
{
    public class SocialShareService : DisposableService
    {
        private const float ShareTimeoutDuration = 3f;
        private const float InviteFriendsHardTimeoutDuration = 600f;
        private const float ShareResultHardTimeoutDuration = 300f;

        private readonly IPlatformSocial _platformSocial;
        private readonly OverlayTaskRunner _overlayRunner;

        public SocialShareService(IPlatformSocial platformSocial, GlobalEventsService globalEventsService)
        {
            _platformSocial = platformSocial;
            _overlayRunner = new OverlayTaskRunner(globalEventsService, GetType().Name);
        }

        public bool IsShareSupported()
        {
            return _platformSocial.IsShareSupported;
        }

        public bool IsInviteSupported()
        {
            return _platformSocial.IsInviteSupported;
        }

        public UniTask<bool> ShareResult(string shareMessage, CancellationToken cancellationToken)
        {
            if (!IsShareSupported())
                return UniTask.FromResult(false);

            return _overlayRunner.Run(
                () => _platformSocial.ShareResult(shareMessage, cancellationToken),
                ShareTimeoutDuration,
                ShareResultHardTimeoutDuration);
        }

        public UniTask<bool> InviteFriends(CancellationToken cancellationToken)
        {
            if (!IsInviteSupported())
                return UniTask.FromResult(false);

            return _overlayRunner.Run(
                () => _platformSocial.InviteFriends(cancellationToken),
                ShareTimeoutDuration,
                InviteFriendsHardTimeoutDuration);
        }

        protected override void OnInitialize() { }

        protected override void OnDisposing()
        {
            _overlayRunner.Cancel();
        }
    }
}
