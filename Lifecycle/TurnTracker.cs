using DamageMeterRebuilt.Engine;
using DamageMeterRebuilt.Infrastructure;
using MegaCrit.Sts2.Core.Combat;

namespace DamageMeterRebuilt.Lifecycle;

internal sealed class TurnTracker
{
    private readonly StatsEngine _engine;
    private readonly Action<CombatState> _turnStartedHandler;
    private readonly Action<CombatState> _turnEndedHandler;
    private CombatManager? _combat;

    public TurnTracker(StatsEngine engine)
    {
        _engine = engine;
        _turnStartedHandler = OnTurnStarted;
        _turnEndedHandler = OnTurnEnded;
    }

    public void Attach(CombatManager combat)
    {
        Detach();
        _combat = combat;
        _combat.TurnStarted += _turnStartedHandler;
        _combat.TurnEnded += _turnEndedHandler;
    }

    public void Detach()
    {
        if (_combat is null)
        {
            return;
        }

        _combat.TurnStarted -= _turnStartedHandler;
        _combat.TurnEnded -= _turnEndedHandler;
        _combat = null;
    }

    private void OnTurnStarted(CombatState state)
    {
        try
        {
            _engine.ApplyTurnStarted(state);
        }
        catch (Exception ex)
        {
            LoggerAdapter.Error("TurnStarted handling failed", ex);
        }
    }

    private void OnTurnEnded(CombatState state)
    {
        try
        {
            _engine.ApplyTurnEnded(state);
        }
        catch (Exception ex)
        {
            LoggerAdapter.Error("TurnEnded handling failed", ex);
        }
    }
}
