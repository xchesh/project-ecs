using GameSdk.Services.RemoteConfig;
using GameSdk.UnityContainer;
using Project.Services.RemoteConfig;

namespace Project.Installers
{
    public class ProjectInstaller : IUnityInstaller
    {
        public override void InstallBindings(IUnityContainer container)
        {
            container.Register<Bootstrap>().As<IBootstrap>();
            container.Register<ProjectRemoteConfigAttribution>().As<IRemoteConfigAttribution>();
        }
    }
}
