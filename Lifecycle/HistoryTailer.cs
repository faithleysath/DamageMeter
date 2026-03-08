using DamageMeterRebuilt.Domain;
using DamageMeterRebuilt.Engine;
using MegaCrit.Sts2.Core.Combat.History;
using System.Linq;

namespace DamageMeterRebuilt.Lifecycle;

internal sealed class HistoryTailer
{
    private readonly StatsEngine _engine;
    private readonly SessionStore _sessions;
    private CombatHistory? _history;

    public HistoryTailer(StatsEngine engine, SessionStore sessions)
    {
        _engine = engine;
        _sessions = sessions;
    }

    public void Attach(CombatHistory history)
    {
        Detach();
        _history = history;
        _history.Changed += OnHistoryChanged;
        Drain();
    }

    public void Detach()
    {
        if (_history is not null)
        {
            _history.Changed -= OnHistoryChanged;
        }

        _history = null;
    }

    private void OnHistoryChanged()
    {
        Drain();
    }

    private void Drain()
    {
        if (_history is null || _sessions.CurrentEncounter is null)
        {
            return;
        }

        var encounter = _sessions.CurrentEncounter;
        var entries = _history.Entries.ToList();

        if (entries.Count < encounter.LastSeenHistoryIndex)
        {
            encounter.LastSeenHistoryIndex = 0;
        }

        for (var index = encounter.LastSeenHistoryIndex; index < entries.Count; index++)
        {
            _engine.Apply(entries[index], encounter);
        }

        encounter.LastSeenHistoryIndex = entries.Count;
        _sessions.MarkDirty();
    }
}
