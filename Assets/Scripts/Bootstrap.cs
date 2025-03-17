using Cysharp.Threading.Tasks;
using GameSdk.Core.Loggers;
using GameSdk.Services.Authentication;
using GameSdk.Services.GraphicQuality;
using GameSdk.Services.InApp;
using GameSdk.Services.NetworkConnectivity;
using GameSdk.Services.PlayerState;
using GameSdk.Services.RemoteConfig;
using GameSdk.UnityContainer;
using UnityEngine;

[JetBrains.Annotations.UsedImplicitly]
public class Bootstrap : IBootstrap
{
    private readonly ISystemLogger _systemLogger;
    private readonly IAuthenticationService _authenticationService;
    private readonly IInAppService _inAppService;
    private readonly IPlayerStatesService _playerStatesService;
    private readonly INetworkConnectivityService _networkConnectivityService;
    private readonly IGraphicQualityService _graphicQualityService;
    private readonly IRemoteConfigService _remoteConfigService;

    public Bootstrap(
        ISystemLogger systemLogger,
        IAuthenticationService authenticationService,
        IInAppService inAppService,
        IPlayerStatesService playerStatesService,
        INetworkConnectivityService networkConnectivityService,
        IGraphicQualityService graphicQualityService,
        IRemoteConfigService remoteConfigService)
    {
        _systemLogger = systemLogger;
        _authenticationService = authenticationService;
        _inAppService = inAppService;
        _playerStatesService = playerStatesService;
        _networkConnectivityService = networkConnectivityService;
        _graphicQualityService = graphicQualityService;
        _remoteConfigService = remoteConfigService;
    }

    public void Boot()
    {
        Initialize().Forget();
    }

    private async UniTaskVoid Initialize()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        _systemLogger.Log(LogType.Log, "Bootstrap", "Start initialization...");

        _networkConnectivityService.Initialize();

        await _authenticationService.Initialize();
        await _remoteConfigService.Initialize();
        await _playerStatesService.Initialize();

        _inAppService.Initialize();
        _graphicQualityService.Initialize();

        sw.Stop();
        _systemLogger.Log(LogType.Log, "Bootstrap", $"Initialization completed in {sw.ElapsedMilliseconds} ms");
    }
}
