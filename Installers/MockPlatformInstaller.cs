
using Platform.Abstractions;
using Platform.Mocks;
using VContainer;
using VContainer.Unity;

namespace Installers
{
    public class MockPlatformInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<MockPlatformInitialization>(Lifetime.Singleton).As<IPlatformInitialization>();
            builder.Register<MockPlatformAuth>(Lifetime.Singleton).As<IPlatformAuth>();
            builder.Register<MockPlatformLifecycle>(Lifetime.Singleton).As<IPlatformLifecycle>();
            builder.Register<MockPlatformAds>(Lifetime.Singleton).As<IPlatformAds>();
            builder.Register<MockPlatformSocial>(Lifetime.Singleton).As<IPlatformSocial>();
            builder.Register<MockPlatformStorage>(Lifetime.Singleton).As<IPlatformStorage>();
            builder.Register<MockPlatformEnvironment>(Lifetime.Singleton).As<IPlatformEnvironment>();
        }
    }
}
