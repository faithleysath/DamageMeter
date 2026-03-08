using DamageMeterRebuilt.Domain;
using DamageMeterRebuilt.Infrastructure;
using DamageMeterRebuilt.Views;
using MegaCrit.Sts2.Core.Runs;

namespace DamageMeterRebuilt.Lifecycle;

internal sealed class RunLifecycleBridge
{
    private readonly SessionStore _sessions;
    private readonly DashboardController _dashboard;
    private readonly SettingsStore _settings;

    public RunLifecycleBridge(SessionStore sessions, DashboardController dashboard, SettingsStore settings)
    {
        _sessions = sessions;
        _dashboard = dashboard;
        _settings = settings;
    }

    public void Attach()
    {
        var runManager = RunManager.Instance;
        runManager.RunStarted += _ => OnRunStarted();
    }

    private void OnRunStarted()
    {
        try
        {
            if (!_settings.AutoResetOnNewRun)
            {
                LoggerAdapter.Info("RunStarted observed, auto-reset disabled.");
                return;
            }

            _sessions.ResetAll();
            _dashboard.Reset();
            LoggerAdapter.Info("Run statistics reset.");
        }
        catch (Exception ex)
        {
            LoggerAdapter.Error("RunStarted handling failed", ex);
        }
    }
}
