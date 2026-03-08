using DamageMeterRebuilt.Domain;
using DamageMeterRebuilt.Infrastructure;
using DamageMeterRebuilt.Views;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Rooms;

namespace DamageMeterRebuilt.Lifecycle;

internal sealed class CombatLifecycleBridge
{
    private readonly SessionStore _sessions;
    private readonly HistoryTailer _historyTailer;
    private readonly TurnTracker _turnTracker;
    private readonly DashboardController _dashboard;

    public CombatLifecycleBridge(
        SessionStore sessions,
        HistoryTailer historyTailer,
        TurnTracker turnTracker,
        DashboardController dashboard)
    {
        _sessions = sessions;
        _historyTailer = historyTailer;
        _turnTracker = turnTracker;
        _dashboard = dashboard;
    }

    public void Attach()
    {
        var combat = CombatManager.Instance;
        combat.CombatSetUp += OnCombatSetUp;
        combat.CombatEnded += OnCombatEnded;
    }

    private void OnCombatSetUp(CombatState state)
    {
        try
        {
            var encounterKey = state.Encounter?.Id?.Entry ?? "unknown";
            _sessions.StartEncounter(encounterKey);

            var combat = CombatManager.Instance;
            _turnTracker.Attach(combat);
            _historyTailer.Attach(combat.History);

            _dashboard.AttachToCurrentCombat();
            LoggerAdapter.Info($"Encounter started: {encounterKey}");
        }
        catch (Exception ex)
        {
            LoggerAdapter.Error("CombatSetUp handling failed", ex);
        }
    }

    private void OnCombatEnded(CombatRoom _)
    {
        try
        {
            _historyTailer.Detach();
            _turnTracker.Detach();
            _sessions.ArchiveCurrentEncounterIfMeaningful();
            LoggerAdapter.Info("Encounter archived.");
        }
        catch (Exception ex)
        {
            LoggerAdapter.Error("CombatEnded handling failed", ex);
        }
    }
}
