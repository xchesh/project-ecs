using GameSdk.Core.Conditions;
using GameSdk.Core.Toolbox;
using GameSdk.Services.Authentication;
using GameSdk.Services.GraphicQuality;
using GameSdk.Services.InApp;
using GameSdk.Services.NetworkConnectivity;
using GameSdk.Services.PlayerState;
using GameSdk.Services.RemoteConfig;
using GameSdk.UnityContainer;
using UnityEngine;

namespace Project.Installers
{
    public class GameSdkServicesInstaller : IUnityInstaller
    {
        [SerializeField] private AuthenticationConfig _authenticationConfig;
        [SerializeField] private NetworkConnectivityConfig _networkConnectivityConfig;
        [SerializeField] private GraphicQualityConfig _graphicQualityConfig;

        public override void InstallBindings(IUnityContainer container)
        {
            // Authentication
            UnityEngine.Assertions.Assert.IsNotNull(_authenticationConfig, "AuthenticationConfig is not set");
            container.RegisterInstance(_authenticationConfig).As<AuthenticationConfig>();
            container.Register<AuthenticationService>().As<IAuthenticationService>();

            // Network Connectivity
            UnityEngine.Assertions.Assert.IsNotNull(_networkConnectivityConfig, "NetworkConnectivityConfig is not set");
            container.RegisterInstance(_networkConnectivityConfig).As<NetworkConnectivityConfig>();
            container.Register<NetworkConnectivityService>().As<INetworkConnectivityService>();

            // In-App
            container.Register<InAppService>().As<IInAppService>();

            // Remote Config
            container.Register<RemoteConfigContext>().As<IRemoteConfigContext>();
            container.Register<RemoteConfigService>().As<IRemoteConfigService>();

            // Graphic Quality
            UnityEngine.Assertions.Assert.IsNotNull(_graphicQualityConfig, "GraphicQualityConfig is not set");
            container.RegisterInstance(_graphicQualityConfig).As<IGraphicQualityConfig>();
            container.Register<GraphicQualityService>().As<IGraphicQualityService>();

            // Player State
            container.Register<PlayerStatesService>().As<IPlayerStatesService>();
            container.Register<PlayerPrefsPlayerStateProvider>().As<IPlayerStateProvider>().WithParameter(PlayerStateProviderType.LOCAL).WithParameter(true);

            // Graphic Quality Conditions
            container.Register<DeviceIdCondition>().As<ICondition>();
            container.Register<DeviceModelsCondition>().As<ICondition>();
            container.Register<GraphicsDeviceNameCondition>().As<ICondition>();
            container.Register<GraphicsDeviceTypeCondition>().As<ICondition>();
            container.Register<MemorySizeCondition>().As<ICondition>();
        }

        public override void Reset()
        {
            _authenticationConfig = EditorExtensions.GetAsset<AuthenticationConfig>();
            _networkConnectivityConfig = EditorExtensions.GetAsset<NetworkConnectivityConfig>();
            _graphicQualityConfig = EditorExtensions.GetAsset<GraphicQualityConfig>();
        }
    }
}
