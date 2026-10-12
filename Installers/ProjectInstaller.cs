using Core.Events;
using Gameplay;
using Services;
using VContainer;
using VContainer.Unity;

namespace Installers
{
    // Excerpt: only the bridge-related part of the real ProjectInstaller.
    public class ProjectInstaller : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
#if UNITY_EDITOR || USE_MOCK_PLATFORM
            new MockPlatformInstaller().Install(builder);
#elif WEBGL_CRAZYGAMES
            new CrazyGamesPlatformInstaller().Install(builder);
#elif WEBGL_PLAYGAMA
            new PlaygamaPlatformInstaller().Install(builder);
#else
            #error No platform define set
#endif

            // ... other game services ...

            builder.Register<AdsService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<SocialShareService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<GlobalEventsService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<OnGamePauseManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }
    }
}
