using DamageMeterRebuilt.Domain;
using DamageMeterRebuilt.Engine;
using DamageMeterRebuilt.Infrastructure;
using DamageMeterRebuilt.Lifecycle;
using DamageMeterRebuilt.Views;
using MegaCrit.Sts2.Core.Modding;

namespace DamageMeterRebuilt;

[ModInitializer("Initialize")]
public static class ModEntry
{
    private static bool _initialized;

    public static void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        try
        {
            I18n.Initialize();
            var settings = new SettingsStore();
            var names = new NameResolver();
            var sessions = new SessionStore(settings.GetRecords(), settings.SaveRecords);
            var engine = new StatsEngine(sessions, names);
            var dashboard = new DashboardController(sessions, settings, names);
            var historyTailer = new HistoryTailer(engine, sessions);
            var turnTracker = new TurnTracker(engine);
            var combatBridge = new CombatLifecycleBridge(sessions, historyTailer, turnTracker, dashboard);
            var runBridge = new RunLifecycleBridge(sessions, dashboard, settings);

            dashboard.Initialize();
            runBridge.Attach();
            combatBridge.Attach();

            _initialized = true;
            LoggerAdapter.Info("Mod initialized.");
        }
        catch (Exception ex)
        {
            LoggerAdapter.Error("Mod initialization failed", ex);
            throw;
        }
    }
}
