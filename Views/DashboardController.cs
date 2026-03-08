using DamageMeterRebuilt.Domain;
using DamageMeterRebuilt.Infrastructure;
using Godot;
using System.Text;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;

namespace DamageMeterRebuilt.Views;

internal sealed class DashboardController
{
    private const int CurrentSegmentId = -1;
    private const int OverallSegmentId = -2;

    private readonly SessionStore _sessions;
    private readonly SettingsStore _settings;
    private readonly NameResolver _names;
    private DashboardOverlay? _overlay;
    private DashboardModalOverlay? _dashboardOverlay;
    private DashboardInputHandler? _inputHandler;
    private bool _overlayVisible;
    private bool _dashboardVisible;
    private DashboardView _currentView;
    private DashboardScope _currentScope;
    private int _selectedSegmentId;
    private string? _detailKey;
    private readonly Dictionary<DashboardView, string> _dashboardDetailKeys = new();

    public DashboardController(SessionStore sessions, SettingsStore settings, NameResolver names)
    {
        _sessions = sessions;
        _settings = settings;
        _names = names;
        _overlayVisible = _settings.OverlayVisible;
        _currentView = _settings.SelectedView;
        _currentScope = _settings.SelectedScope;
        _selectedSegmentId = _currentScope == DashboardScope.Current ? CurrentSegmentId : OverallSegmentId;
        _sessions.Changed += OnStatsChanged;
    }

    public void Initialize()
    {
        EnsureOverlay();
        Refresh();
        LoggerAdapter.Info("Dashboard controller initialized.");
    }

    public void AttachToCurrentCombat()
    {
        _detailKey = null;
        EnsureOverlay();
        Refresh();
        LoggerAdapter.Info("Dashboard attached.");
    }

    public void Reset()
    {
        _detailKey = null;
        _dashboardDetailKeys.Clear();
        EnsureOverlay();
        Refresh();
        LoggerAdapter.Info("Dashboard reset.");
    }

    public void ToggleVisibility()
    {
        _overlayVisible = !_overlayVisible;
        _settings.UpdateOverlayVisible(_overlayVisible);
        Refresh();
        LoggerAdapter.Info($"Dashboard visibility toggled: {(_overlayVisible ? "shown" : "hidden")}");
    }

    public void CycleView(bool forward)
    {
        if (!forward && _detailKey is not null)
        {
            _detailKey = null;
            Refresh();
            LoggerAdapter.Info("Returned from detail view.");
            return;
        }

        var views = Enum.GetValues<DashboardView>();
        var currentIndex = Array.IndexOf(views, _currentView);
        if (currentIndex < 0)
        {
            currentIndex = 0;
        }

        currentIndex = forward
            ? (currentIndex + 1) % views.Length
            : (currentIndex - 1 + views.Length) % views.Length;

        _currentView = views[currentIndex];
        _detailKey = null;
        _settings.UpdateSelectedView(_currentView);
        Refresh();
        LoggerAdapter.Info($"Dashboard view changed to {_currentView}.");
    }

    public void ToggleScope()
    {
        _currentScope = _currentScope == DashboardScope.Current
            ? DashboardScope.Overall
            : DashboardScope.Current;
        _selectedSegmentId = _currentScope == DashboardScope.Current ? CurrentSegmentId : OverallSegmentId;
        _detailKey = null;
        _settings.UpdateSelectedScope(_currentScope);
        Refresh();
        LoggerAdapter.Info($"Dashboard scope changed to {_currentScope}.");
    }

    private void OnStatsChanged()
    {
        Refresh();
    }

    private void EnsureOverlay()
    {
        var tree = Godot.Engine.GetMainLoop() as SceneTree;
        if (tree?.Root is null)
        {
            return;
        }

        EnsureInputHandler(tree.Root);

        if (!IsOverlayAlive())
        {
            _overlay = new DashboardOverlay(_settings.PanelPosition, _settings.UseAnchoredPanelPosition);
            _overlay.CycleViewRequested += CycleView;
            _overlay.ResetRequested += OnResetRequested;
            _overlay.DashboardRequested += OnDashboardRequested;
            _overlay.TitleRequested += OnTitleRequested;
            _overlay.SegmentCycleRequested += OnSegmentCycleRequested;
            _overlay.BarActivated += OnBarActivated;
            _overlay.ViewSelected += OnViewSelected;
            _overlay.SegmentSelected += OnSegmentSelected;
            _overlay.ScaleSelected += OnScaleSelected;
            _overlay.OpacitySelected += OnOpacitySelected;
            _overlay.MaxBarsSelected += OnMaxBarsSelected;
            _overlay.AutoResetChanged += OnAutoResetChanged;
            _overlay.ResetPositionRequested += OnResetPositionRequested;
            _overlay.PositionChanged += position => _settings.UpdatePanelPosition(position);
            _overlay.TreeEntered += OnOverlayTreeEntered;
            _overlay.ApplyStyle(_settings.Scale, _settings.Opacity);
            _overlay.SetDashboardAvailable(true);
            _overlay.HideOverlay();
        }

        if (_overlay!.GetParent() is null)
        {
            tree.Root.CallDeferred(Node.MethodName.AddChild, _overlay);
            LoggerAdapter.Info("Dashboard overlay queued for scene attachment.");
        }

        if (!IsDashboardOverlayAlive())
        {
            _dashboardOverlay = new DashboardModalOverlay();
            _dashboardOverlay.CloseRequested += CloseDashboardOverlay;
            _dashboardOverlay.PanelTitleRequested += OnDashboardPanelTitleRequested;
            _dashboardOverlay.PanelBarActivated += OnDashboardPanelBarActivated;
            _dashboardOverlay.Hide();
        }
    }

    private void EnsureInputHandler(Window root)
    {
        if (!IsInputHandlerAlive())
        {
            _inputHandler = new DashboardInputHandler();
            _inputHandler.ToggleRequested += ToggleVisibility;
            _inputHandler.CycleViewRequested += CycleView;
            _inputHandler.ToggleScopeRequested += ToggleScope;
            _inputHandler.CloseDashboardRequested += OnDashboardCloseRequested;
        }

        if (_inputHandler!.GetParent() is null)
        {
            root.CallDeferred(Node.MethodName.AddChild, _inputHandler);
            LoggerAdapter.Info("Dashboard input handler queued for scene attachment.");
        }
    }

    private bool IsOverlayAlive()
    {
        return _overlay is not null && GodotObject.IsInstanceValid(_overlay);
    }

    private bool IsInputHandlerAlive()
    {
        return _inputHandler is not null && GodotObject.IsInstanceValid(_inputHandler);
    }

    private bool IsDashboardOverlayAlive()
    {
        return _dashboardOverlay is not null && GodotObject.IsInstanceValid(_dashboardOverlay);
    }

    private void UpdateDashboardOverlay()
    {
        if (!IsDashboardOverlayAlive())
        {
            return;
        }

        if (!_dashboardVisible)
        {
            _dashboardOverlay!.Hide();
            return;
        }

        var tree = Godot.Engine.GetMainLoop() as SceneTree;
        if (_dashboardOverlay!.GetParent() is null && tree?.Root is not null)
        {
            tree.Root.CallDeferred(Node.MethodName.AddChild, _dashboardOverlay);
        }

        if (_dashboardOverlay.GetParent() is null)
        {
            return;
        }

        _dashboardOverlay.Show();
        _dashboardOverlay.SetView(BuildDashboardModalView());
    }

    private void Refresh()
    {
        if (!IsOverlayAlive() || _overlay!.GetParent() is null || !_overlay.IsInsideTree())
        {
            return;
        }

        UpdateDashboardOverlay();

        if (!_overlayVisible)
        {
            _overlay!.HideOverlay();
            return;
        }

        _overlay.ConfigureMenus(
            _currentView,
            BuildSegmentLabels(),
            GetSelectedSegmentMenuIndex(),
            _settings.Scale,
            _settings.Opacity,
            _settings.MaxBars,
            _settings.AutoResetOnNewRun);

        if (_selectedSegmentId == OverallSegmentId)
        {
            var overall = _sessions.GetOverallView();
            if (overall.OverallPlayers.Count == 0 && _sessions.ArchivedSegments.Count == 0)
            {
                _overlay!.HideOverlay();
                return;
            }

            _overlay!.ShowOverlay();
            _overlay.SetView(
                BuildOverallBarView(overall),
                BuildFooter($"{I18n.SegmentOverall} | {_sessions.ArchivedSegments.Count} {I18n.SegmentFight}"),
                _settings.MaxBars);
            return;
        }

        if (_selectedSegmentId >= 0)
        {
            var archived = GetSelectedArchivedSegment();
            if (archived is null)
            {
                _selectedSegmentId = CurrentSegmentId;
                _currentScope = DashboardScope.Current;
                _settings.UpdateSelectedScope(_currentScope);
                Refresh();
                return;
            }

            _overlay!.ShowOverlay();
            _overlay.SetView(
                BuildArchivedBarView(archived, _selectedSegmentId),
                BuildFooter($"{archived.EncounterKey} | T{archived.TurnCount} | {archived.Timeline.Count} {I18n.Events}"),
                _settings.MaxBars);
            return;
        }

        if (_sessions.CurrentEncounter is null)
        {
            var overall = _sessions.GetOverallView();
            if (overall.OverallPlayers.Count == 0 && _sessions.ArchivedSegments.Count == 0)
            {
                _overlay!.HideOverlay();
                return;
            }

            _overlay!.ShowOverlay();
            _overlay.SetView(
                new DashboardBarView(
                    BuildPanelTitle(),
                    TitleClickable: false,
                    SegmentLabel: GetSegmentLabel(),
                    Bars: new[]
                    {
                        new DashboardBarItem("empty", I18n.WaitingForCombat, 0, I18n.WaitingForCombat)
                    }),
                BuildFooter($"{I18n.WaitingForCombat} | {_sessions.ArchivedSegments.Count} {I18n.SegmentFight}"),
                _settings.MaxBars);
            return;
        }

        var encounter = _sessions.CurrentEncounter;
        _overlay!.ShowOverlay();
        _overlay.SetView(
            BuildEncounterBarView(encounter),
            BuildFooter($"{encounter.EncounterKey} | T{encounter.CurrentTurn} | {encounter.Timeline.Count} {I18n.Events}"),
            _settings.MaxBars);
    }

    private string BuildEncounterBody(EncounterSession encounter)
    {
        return _currentView switch
        {
            DashboardView.DamageDealt => BuildEncounterDamageDealtBody(encounter),
            DashboardView.DamageTaken => BuildEncounterDamageTakenBody(encounter),
            DashboardView.CardUsage => BuildEncounterCardUsageBody(encounter),
            DashboardView.Block => BuildEncounterBlockBody(encounter),
            DashboardView.Energy => BuildEncounterEnergyBody(encounter),
            DashboardView.Efficiency => BuildEncounterEfficiencyBody(encounter),
            DashboardView.Potions => BuildEncounterPotionsBody(encounter),
            DashboardView.Debuffs => BuildEncounterDebuffsBody(encounter),
            DashboardView.DeathLog => BuildEncounterDeathLogBody(encounter),
            DashboardView.CardFlow => BuildEncounterCardFlowBody(encounter),
            DashboardView.CombatLog => BuildEncounterCombatLogBody(encounter),
            DashboardView.Records => BuildRecordsBody(_sessions.GetOverallView().Records),
            DashboardView.Orbs => BuildEncounterOrbsBody(encounter),
            DashboardView.Threat => BuildEncounterThreatBody(encounter),
            DashboardView.Advanced => BuildEncounterAdvancedBody(encounter),
            _ => "Unsupported category."
        };
    }

    private string BuildOverallBody(RunStats overall)
    {
        return _currentView switch
        {
            DashboardView.DamageDealt => BuildOverallDamageDealtBody(overall),
            DashboardView.DamageTaken => BuildOverallDamageTakenBody(overall),
            DashboardView.CardUsage => BuildOverallCardUsageBody(overall),
            DashboardView.Block => BuildOverallBlockBody(overall),
            DashboardView.Energy => BuildOverallEnergyBody(overall),
            DashboardView.Efficiency => BuildOverallEfficiencyBody(overall),
            DashboardView.Potions => BuildOverallPotionsBody(overall),
            DashboardView.Debuffs => BuildOverallDebuffsBody(overall),
            DashboardView.DeathLog => BuildArchivedDeathLogBody(),
            DashboardView.CardFlow => BuildOverallCardFlowBody(overall),
            DashboardView.CombatLog => BuildArchivedTimelineBody(),
            DashboardView.Records => BuildRecordsBody(overall.Records),
            DashboardView.Orbs => BuildOverallOrbsBody(overall),
            DashboardView.Threat => BuildOverallThreatBody(overall),
            DashboardView.Advanced => BuildOverallAdvancedBody(overall),
            _ => "Unsupported category."
        };
    }

    private void OnOverlayTreeEntered()
    {
        LoggerAdapter.Info("Dashboard overlay attached to scene tree.");
        Refresh();
    }

    private void OnResetRequested()
    {
        _detailKey = null;
        _dashboardDetailKeys.Clear();
        _sessions.ResetAll();
        if (_sessions.IsTracking)
        {
            _currentScope = DashboardScope.Current;
            _selectedSegmentId = CurrentSegmentId;
            _settings.UpdateSelectedScope(_currentScope);
        }

        Refresh();
        LoggerAdapter.Info("Dashboard reset requested.");
    }

    private void OnDashboardRequested()
    {
        _dashboardVisible = !_dashboardVisible;
        if (!_dashboardVisible)
        {
            CloseDashboardOverlay();
            return;
        }

        EnsureOverlay();
        Refresh();
        LoggerAdapter.Info("Dashboard overlay opened.");
    }

    private void OnDashboardCloseRequested()
    {
        if (!_dashboardVisible)
        {
            return;
        }

        CloseDashboardOverlay();
    }

    private void OnViewSelected(DashboardView view)
    {
        if (_currentView == view)
        {
            return;
        }

        _currentView = view;
        _detailKey = null;
        _settings.UpdateSelectedView(_currentView);
        Refresh();
        LoggerAdapter.Info($"Dashboard view selected: {_currentView}.");
    }

    private void OnTitleRequested()
    {
        if (_detailKey is null)
        {
            return;
        }

        _detailKey = null;
        Refresh();
        LoggerAdapter.Info("Returned from detail view.");
    }

    private void OnSegmentCycleRequested(bool forward)
    {
        var labels = BuildSegmentLabels();
        if (labels.Count == 0)
        {
            return;
        }

        var currentIndex = GetSelectedSegmentMenuIndex();
        var nextIndex = forward
            ? (currentIndex + 1) % labels.Count
            : (currentIndex - 1 + labels.Count) % labels.Count;
        SelectSegmentByMenuIndex(nextIndex);
    }

    private void OnSegmentSelected(int menuIndex)
    {
        SelectSegmentByMenuIndex(menuIndex);
    }

    private void OnBarActivated(string key)
    {
        if (!CurrentCategoryHasDetail())
        {
            return;
        }

        _detailKey = key;
        Refresh();
        LoggerAdapter.Info($"Opened detail for {key}.");
    }

    private void OnScaleSelected(float scale)
    {
        _settings.UpdateScale(scale);
        _overlay?.ApplyStyle(_settings.Scale, _settings.Opacity);
        Refresh();
        LoggerAdapter.Info($"Dashboard scale updated to {_settings.Scale:0.00}.");
    }

    private void OnOpacitySelected(float opacity)
    {
        _settings.UpdateOpacity(opacity);
        _overlay?.ApplyStyle(_settings.Scale, _settings.Opacity);
        Refresh();
        LoggerAdapter.Info($"Dashboard opacity updated to {_settings.Opacity:0.00}.");
    }

    private void OnMaxBarsSelected(int maxBars)
    {
        _settings.UpdateMaxBars(maxBars);
        Refresh();
        LoggerAdapter.Info($"Dashboard max bars updated to {_settings.MaxBars}.");
    }

    private void OnAutoResetChanged(bool autoReset)
    {
        _settings.UpdateAutoReset(autoReset);
        Refresh();
        LoggerAdapter.Info($"Dashboard auto-reset updated: {autoReset}.");
    }

    private void OnResetPositionRequested()
    {
        _settings.ResetPanelPosition();
        if (IsOverlayAlive())
        {
            _overlay!.QueueFree();
            _overlay = null;
        }

        EnsureOverlay();
        Refresh();
        LoggerAdapter.Info("Dashboard position reset.");
    }

    private void CloseDashboardOverlay()
    {
        _dashboardVisible = false;
        _dashboardDetailKeys.Clear();
        if (IsDashboardOverlayAlive())
        {
            _dashboardOverlay!.Hide();
        }

        LoggerAdapter.Info("Dashboard overlay closed.");
    }

    private void OnDashboardPanelTitleRequested(DashboardView view)
    {
        if (_dashboardDetailKeys.Remove(view))
        {
            UpdateDashboardOverlay();
            LoggerAdapter.Info($"Dashboard detail closed for {view}.");
        }
    }

    private void OnDashboardPanelBarActivated(DashboardView view, string key)
    {
        _dashboardDetailKeys[view] = key;
        UpdateDashboardOverlay();
        LoggerAdapter.Info($"Dashboard detail opened for {view}: {key}.");
    }

    private DashboardBarView BuildEncounterBarView(EncounterSession encounter)
    {
        var bars = BuildBars(encounter.Players, encounter.Timeline, encounter.DeathSnapshots, overall: null);
        return new DashboardBarView(
            BuildPanelTitle(),
            _detailKey is not null,
            GetSegmentLabel(),
            bars);
    }

    private DashboardBarView BuildArchivedBarView(EncounterSegment segment, int archivedIndex)
    {
        var bars = BuildBars(segment.Players, segment.Timeline, segment.DeathSnapshots, overall: null);
        return new DashboardBarView(
            BuildPanelTitle(),
            _detailKey is not null,
            BuildArchivedSegmentLabel(archivedIndex, segment.EncounterKey),
            bars);
    }

    private DashboardBarView BuildOverallBarView(RunStats overall)
    {
        var bars = BuildBars(overall.OverallPlayers, timeline: null, deathSnapshots: null, overall);
        return new DashboardBarView(
            BuildPanelTitle(),
            _detailKey is not null,
            GetSegmentLabel(),
            bars);
    }

    private DashboardModalView BuildDashboardModalView()
    {
        var panels = new List<DashboardMiniPanelView>();
        var segmentLabel = GetSegmentLabel();
        var context = ResolveCurrentContext();

        foreach (var view in Enum.GetValues<DashboardView>())
        {
            var detailKey = _dashboardDetailKeys.GetValueOrDefault(view);
            var bars = BuildBarsForView(view, detailKey, context.Players, context.Timeline, context.DeathSnapshots, context.Overall)
                .Take(8)
                .ToList();

            panels.Add(new DashboardMiniPanelView(
                view,
                BuildDashboardPanelTitle(view, detailKey, context.Players),
                detailKey is not null,
                CategoryHasDetail(view) && detailKey is null,
                bars));
        }

        return new DashboardModalView($"{I18n.Dashboard}  —  {segmentLabel}", panels);
    }

    private IReadOnlyList<DashboardBarItem> BuildBars(
        Dictionary<string, PlayerStats> players,
        IReadOnlyList<TimelineEvent>? timeline,
        IReadOnlyDictionary<string, List<TimelineEvent>>? deathSnapshots,
        RunStats? overall)
    {
        return _currentView switch
        {
            DashboardView.DamageDealt => BuildDamageDealtBars(players),
            DashboardView.DamageTaken => BuildDamageTakenBars(players),
            DashboardView.Dpt => BuildDptBars(players),
            DashboardView.CardUsage => BuildCardUsageBars(players),
            DashboardView.Block => BuildBlockBars(players),
            DashboardView.Energy => BuildEnergyBars(players),
            DashboardView.Efficiency => BuildEfficiencyBars(players),
            DashboardView.Overkill => BuildOverkillBars(players),
            DashboardView.Potions => BuildPotionBars(players),
            DashboardView.Debuffs => BuildDebuffBars(players),
            DashboardView.DeathLog => BuildDeathLogBars(players, deathSnapshots),
            DashboardView.CardFlow => BuildCardFlowBars(players),
            DashboardView.CombatLog => BuildCombatLogBars(players, timeline),
            DashboardView.Records => BuildRecordBars(overall ?? _sessions.GetOverallView()),
            DashboardView.Orbs => BuildOrbBars(players),
            DashboardView.Threat => BuildThreatBars(players),
            DashboardView.Advanced => BuildAdvancedBars(players),
            _ => Array.Empty<DashboardBarItem>()
        };
    }

    private IReadOnlyList<DashboardBarItem> BuildBarsForView(
        DashboardView view,
        string? detailKey,
        Dictionary<string, PlayerStats> players,
        IReadOnlyList<TimelineEvent>? timeline,
        IReadOnlyDictionary<string, List<TimelineEvent>>? deathSnapshots,
        RunStats? overall)
    {
        var previousView = _currentView;
        var previousDetailKey = _detailKey;
        try
        {
            _currentView = view;
            _detailKey = detailKey;
            return BuildBars(players, timeline, deathSnapshots, overall).ToList();
        }
        finally
        {
            _currentView = previousView;
            _detailKey = previousDetailKey;
        }
    }

    private DashboardContext ResolveCurrentContext()
    {
        if (_selectedSegmentId == OverallSegmentId)
        {
            var overall = _sessions.GetOverallView();
            return new DashboardContext(overall.OverallPlayers, null, null, overall);
        }

        if (_selectedSegmentId >= 0)
        {
            var archived = GetSelectedArchivedSegment();
            if (archived is not null)
            {
                return new DashboardContext(archived.Players, archived.Timeline, archived.DeathSnapshots, null);
            }
        }

        if (_sessions.CurrentEncounter is not null)
        {
            return new DashboardContext(
                _sessions.CurrentEncounter.Players,
                _sessions.CurrentEncounter.Timeline,
                _sessions.CurrentEncounter.DeathSnapshots,
                null);
        }

        var fallback = _sessions.GetOverallView();
        return new DashboardContext(fallback.OverallPlayers, null, null, fallback);
    }

    private IReadOnlyList<string> BuildSegmentLabels()
    {
        var labels = new List<string> { I18n.SegmentCurrent, I18n.SegmentOverall };
        for (var index = 0; index < _sessions.ArchivedSegments.Count; index++)
        {
            labels.Add(BuildArchivedSegmentLabel(index, _sessions.ArchivedSegments[index].EncounterKey));
        }

        return labels;
    }

    private int GetSelectedSegmentMenuIndex()
    {
        return _selectedSegmentId switch
        {
            CurrentSegmentId => 0,
            OverallSegmentId => 1,
            >= 0 => Math.Min(_selectedSegmentId + 2, _sessions.ArchivedSegments.Count + 1),
            _ => 0
        };
    }

    private void SelectSegmentByMenuIndex(int menuIndex)
    {
        if (menuIndex <= 0)
        {
            _selectedSegmentId = CurrentSegmentId;
            _currentScope = DashboardScope.Current;
            _settings.UpdateSelectedScope(_currentScope);
        }
        else if (menuIndex == 1)
        {
            _selectedSegmentId = OverallSegmentId;
            _currentScope = DashboardScope.Overall;
            _settings.UpdateSelectedScope(_currentScope);
        }
        else
        {
            var archivedIndex = menuIndex - 2;
            if (archivedIndex < 0 || archivedIndex >= _sessions.ArchivedSegments.Count)
            {
                return;
            }

            _selectedSegmentId = archivedIndex;
        }

        _detailKey = null;
        Refresh();
        LoggerAdapter.Info($"Dashboard segment selected: {GetSegmentLabel()}.");
    }

    private EncounterSegment? GetSelectedArchivedSegment()
    {
        return _selectedSegmentId >= 0 && _selectedSegmentId < _sessions.ArchivedSegments.Count
            ? _sessions.ArchivedSegments[_selectedSegmentId]
            : null;
    }

    private IReadOnlyList<DashboardBarItem> BuildDamageDealtBars(Dictionary<string, PlayerStats> players)
    {
        if (_detailKey is null)
        {
            return players.Values
                .OrderByDescending(player => player.DamageDealt)
                .Select((player, index) => CreatePlayerSummaryBar(player, index, player.DamageDealt))
                .ToList();
        }

        return TryGetPlayer(players, _detailKey, out var playerStats)
            ? playerStats.DamageByCard
                .OrderByDescending(entry => entry.Value)
                .Select(entry => new DashboardBarItem(entry.Key, _names.ResolveCardDisplayName(entry.Key), entry.Value))
                .ToList()
            : Array.Empty<DashboardBarItem>();
    }

    private IReadOnlyList<DashboardBarItem> BuildDamageTakenBars(Dictionary<string, PlayerStats> players)
    {
        if (_detailKey is null)
        {
            return players.Values
                .OrderByDescending(player => player.DamageTaken)
                .Select((player, index) => CreatePlayerSummaryBar(player, index, player.DamageTaken))
                .ToList();
        }

        return TryGetPlayer(players, _detailKey, out var playerStats)
            ? playerStats.DamageBySource
                .OrderByDescending(entry => entry.Value)
                .Select(entry => new DashboardBarItem(entry.Key, _names.ResolveMonsterDisplayName(entry.Key), entry.Value))
                .ToList()
            : Array.Empty<DashboardBarItem>();
    }

    private IReadOnlyList<DashboardBarItem> BuildDptBars(Dictionary<string, PlayerStats> players)
    {
        var turnCount = GetDptTurnCount();
        if (_detailKey is null)
        {
            return players.Values
                .OrderByDescending(player => turnCount > 0 ? player.DamageDealt / turnCount : 0)
                .Select((player, index) => CreatePlayerSummaryBar(
                    player,
                    index,
                    turnCount > 0 ? player.DamageDealt / turnCount : 0))
                .ToList();
        }

        return TryGetPlayer(players, _detailKey, out var playerStats)
            ? playerStats.DamagePerTurn
                .OrderBy(entry => entry.Key)
                .Select(entry => new DashboardBarItem($"turn_{entry.Key}", $"{I18n.Turn} {entry.Key}", entry.Value))
                .ToList()
            : Array.Empty<DashboardBarItem>();
    }

    private IReadOnlyList<DashboardBarItem> BuildCardUsageBars(Dictionary<string, PlayerStats> players)
    {
        if (_detailKey is null)
        {
            return players.Values
                .OrderByDescending(player => player.CardsPlayed)
                .Select((player, index) => CreatePlayerSummaryBar(player, index, player.CardsPlayed))
                .ToList();
        }

        return TryGetPlayer(players, _detailKey, out var playerStats)
            ? playerStats.CardPlayCount
                .OrderByDescending(entry => entry.Value)
                .Select(entry => new DashboardBarItem(entry.Key, _names.ResolveCardDisplayName(entry.Key), entry.Value))
                .ToList()
            : Array.Empty<DashboardBarItem>();
    }

    private IReadOnlyList<DashboardBarItem> BuildBlockBars(Dictionary<string, PlayerStats> players)
    {
        if (_detailKey is null)
        {
            return players.Values
                .OrderByDescending(player => player.TotalBlockGained)
                .Select((player, index) => CreatePlayerSummaryBar(player, index, player.TotalBlockGained))
                .ToList();
        }

        return TryGetPlayer(players, _detailKey, out var playerStats)
            ? playerStats.BlockByCard
                .OrderByDescending(entry => entry.Value)
                .Select(entry => new DashboardBarItem(entry.Key, _names.ResolveCardDisplayName(entry.Key), entry.Value))
                .ToList()
            : Array.Empty<DashboardBarItem>();
    }

    private IReadOnlyList<DashboardBarItem> BuildEnergyBars(Dictionary<string, PlayerStats> players)
    {
        if (_detailKey is null)
        {
            return players.Values
                .Where(player => player.TotalEnergySpent > 0)
                .OrderByDescending(player => player.TotalEnergySpent)
                .Select((player, index) => CreatePlayerSummaryBar(player, index, player.TotalEnergySpent))
                .ToList();
        }

        if (!TryGetPlayer(players, _detailKey, out var playerStats))
        {
            return Array.Empty<DashboardBarItem>();
        }

        var bars = playerStats.EnergyWastedPerTurn
            .OrderBy(entry => entry.Key)
            .Where(entry => entry.Value > 0)
            .Select(entry => new DashboardBarItem($"turn_{entry.Key}", $"{I18n.Turn} {entry.Key}", entry.Value, $"{entry.Value} {I18n.Wasted}"))
            .ToList();

        if (bars.Count == 0 && playerStats.TotalEnergyWasted > 0)
        {
            bars.Add(new DashboardBarItem("total_wasted", I18n.TotalWasted, playerStats.TotalEnergyWasted));
        }

        return bars;
    }

    private IReadOnlyList<DashboardBarItem> BuildEfficiencyBars(Dictionary<string, PlayerStats> players)
    {
        if (_detailKey is null)
        {
            return players.Values
                .Where(player => player.TotalEnergySpent > 0)
                .OrderByDescending(ComputeEfficiency)
                .Select((player, index) =>
                {
                    var efficiency = ComputeEfficiency(player);
                    return new DashboardBarItem(
                        player.Key,
                        $"{index + 1}. {player.DisplayName}",
                        Math.Max(1, (int)(efficiency * 10)),
                        $"{efficiency:F1} DMG/E",
                        Clickable: true,
                        ColorOverride: PlayerVisuals.GetPlayerColor(player.Key, player.CharacterColor),
                        Icon: PlayerVisuals.LoadCharacterIcon(player.CharacterId));
                })
                .ToList();
        }

        if (!TryGetPlayer(players, _detailKey, out var playerStats))
        {
            return Array.Empty<DashboardBarItem>();
        }

        var bars = new List<DashboardBarItem>();
        foreach (var entry in playerStats.DamageByCard.OrderByDescending(entry => entry.Value))
        {
            var energy = playerStats.EnergySpentByCard.GetValueOrDefault(entry.Key);
            if (energy <= 0)
            {
                continue;
            }

            var efficiency = (double)entry.Value / energy;
            bars.Add(new DashboardBarItem(
                entry.Key,
                _names.ResolveCardDisplayName(entry.Key),
                Math.Max(1, (int)(efficiency * 10)),
                $"{efficiency:F1} ({entry.Value}/{energy}E)"));
        }

        return bars;
    }

    private IReadOnlyList<DashboardBarItem> BuildOverkillBars(Dictionary<string, PlayerStats> players)
    {
        if (_detailKey is null)
        {
            return players.Values
                .Where(player => player.OverkillDealt > 0)
                .OrderByDescending(player => player.OverkillDealt)
                .Select((player, index) => CreatePlayerSummaryBar(player, index, player.OverkillDealt))
                .ToList();
        }

        if (!TryGetPlayer(players, _detailKey, out var playerStats))
        {
            return Array.Empty<DashboardBarItem>();
        }

        var bars = new List<DashboardBarItem>();
        if (playerStats.DamageDealt > 0)
        {
            bars.Add(new DashboardBarItem("effective", I18n.CatDamageDealt, playerStats.DamageDealt));
        }
        if (playerStats.OverkillDealt > 0)
        {
            bars.Add(new DashboardBarItem("overkill", I18n.CatOverkill, playerStats.OverkillDealt));
        }
        if (playerStats.BlockedByTarget > 0)
        {
            bars.Add(new DashboardBarItem("blocked", I18n.Blocked, playerStats.BlockedByTarget));
        }

        return bars;
    }

    private IReadOnlyList<DashboardBarItem> BuildPotionBars(Dictionary<string, PlayerStats> players)
    {
        if (_detailKey is null)
        {
            return players.Values
                .Where(player => player.PotionsUsed > 0)
                .OrderByDescending(player => player.PotionsUsed)
                .Select((player, index) => CreatePlayerSummaryBar(player, index, player.PotionsUsed))
                .ToList();
        }

        return TryGetPlayer(players, _detailKey, out var playerStats)
            ? playerStats.PotionUseCount
                .OrderByDescending(entry => entry.Value)
                .Select(entry => new DashboardBarItem(entry.Key, _names.ResolvePotionDisplayName(entry.Key), entry.Value))
                .ToList()
            : Array.Empty<DashboardBarItem>();
    }

    private IReadOnlyList<DashboardBarItem> BuildDebuffBars(Dictionary<string, PlayerStats> players)
    {
        if (_detailKey is null)
        {
            return players.Values
                .Where(player => player.DebuffsApplied > 0)
                .OrderByDescending(player => player.DebuffsApplied)
                .Select((player, index) => CreatePlayerSummaryBar(player, index, player.DebuffsApplied))
                .ToList();
        }

        return TryGetPlayer(players, _detailKey, out var playerStats)
            ? playerStats.PowerDebuffCount
                .OrderByDescending(entry => entry.Value)
                .Select(entry => new DashboardBarItem(entry.Key, _names.ResolvePowerDisplayName(entry.Key), entry.Value))
                .ToList()
            : Array.Empty<DashboardBarItem>();
    }

    private IReadOnlyList<DashboardBarItem> BuildDeathLogBars(
        Dictionary<string, PlayerStats> players,
        IReadOnlyDictionary<string, List<TimelineEvent>>? deathSnapshots)
    {
        if (_detailKey is null)
        {
            return players.Values
                .Select(player =>
                {
                    var dead = deathSnapshots?.ContainsKey(player.Key) == true;
                    return CreatePlayerSummaryBar(player, null, dead ? 1 : 0, dead ? I18n.Dead : I18n.Alive);
                })
                .ToList();
        }

        if (deathSnapshots is null || !deathSnapshots.TryGetValue(_detailKey, out var events))
        {
            return Array.Empty<DashboardBarItem>();
        }

        return events
            .Select((item, index) => new DashboardBarItem(
                $"evt_{index}",
                $"{FormatLegacyEventPrefix(item.Type, item.Turn)} {item.Label}",
                Math.Max(1, item.Value),
                item.Value > 0 ? item.Value.ToString() : string.Empty))
            .ToList();
    }

    private IReadOnlyList<DashboardBarItem> BuildCardFlowBars(Dictionary<string, PlayerStats> players)
    {
        if (_detailKey is null)
        {
            return players.Values
                .Select(player => new
                {
                    Player = player,
                    Total = player.CardsDrawn + player.CardsDiscarded + player.CardsExhausted
                })
                .Where(entry => entry.Total > 0)
                .OrderByDescending(entry => entry.Total)
                .Select((entry, index) => CreatePlayerSummaryBar(
                    entry.Player,
                    index,
                    entry.Total,
                    $"{entry.Player.CardsDrawn}D {entry.Player.CardsDiscarded}d {entry.Player.CardsExhausted}E"))
                .ToList();
        }

        if (!TryGetPlayer(players, _detailKey, out var playerStats))
        {
            return Array.Empty<DashboardBarItem>();
        }

        var bars = new List<DashboardBarItem>();
        if (playerStats.CardsDrawn > 0)
        {
            bars.Add(new DashboardBarItem("drawn", I18n.Drawn, playerStats.CardsDrawn));
        }
        if (playerStats.CardsDiscarded > 0)
        {
            bars.Add(new DashboardBarItem("discarded", I18n.Discarded, playerStats.CardsDiscarded));
        }
        if (playerStats.CardsExhausted > 0)
        {
            bars.Add(new DashboardBarItem("exhausted", I18n.Exhausted, playerStats.CardsExhausted));
        }

        bars.AddRange(playerStats.ExhaustCount
            .OrderByDescending(entry => entry.Value)
            .Take(5)
            .Select(entry => new DashboardBarItem(
                $"exhaust_{entry.Key}",
                "  " + _names.ResolveCardDisplayName(entry.Key),
                entry.Value,
                $"{entry.Value}x {I18n.Exhausted}")));

        return bars;
    }

    private IReadOnlyList<DashboardBarItem> BuildCombatLogBars(
        Dictionary<string, PlayerStats> players,
        IReadOnlyList<TimelineEvent>? timeline)
    {
        if (_detailKey is null)
        {
            if (timeline is null)
            {
                return Array.Empty<DashboardBarItem>();
            }

            return players.Values
                .Select(player =>
                {
                    var count = timeline.Count(item => item.ActorKey == player.Key && IsLegacyCombatLogEvent(item.Type));
                    return CreatePlayerSummaryBar(player, null, count, $"{count} {I18n.Events}");
                })
                .OrderByDescending(item => item.Value)
                .ToList();
        }

        if (timeline is null)
        {
            return Array.Empty<DashboardBarItem>();
        }

        return timeline
            .Where(item => item.ActorKey == _detailKey && IsLegacyCombatLogEvent(item.Type))
            .TakeLast(20)
            .Reverse()
            .Select((item, index) => new DashboardBarItem(
                $"evt_{index}",
                $"{FormatLegacyEventPrefix(item.Type, item.Turn)} {item.Label}",
                Math.Max(1, item.Value),
                item.Value > 0 ? item.Value.ToString() : string.Empty))
            .ToList();
    }

    private IReadOnlyList<DashboardBarItem> BuildRecordBars(RunStats overall)
    {
        var records = overall.Records;
        var bars = new List<DashboardBarItem>();
        if (records.HighestHit > 0)
        {
            var display = records.HighestHitCard.Length == 0
                ? records.HighestHit.ToString()
                : $"{records.HighestHit} ({_names.ResolveCardDisplayName(records.HighestHitCard)})";
            bars.Add(new DashboardBarItem("highest_hit", I18n.RecHighestHit, records.HighestHit, display));
        }
        if (records.MostFightDamage > 0)
        {
            bars.Add(new DashboardBarItem("most_fight", I18n.RecMostFightDmg, records.MostFightDamage));
        }
        if (records.BestTurnDamage > 0)
        {
            bars.Add(new DashboardBarItem("best_turn", I18n.RecBestTurnDmg, records.BestTurnDamage));
        }
        if (records.MostCardsPlayed > 0)
        {
            bars.Add(new DashboardBarItem("most_cards", I18n.RecMostCards, records.MostCardsPlayed));
        }
        if (records.MostBlockGained > 0)
        {
            bars.Add(new DashboardBarItem("most_block", I18n.RecMostBlock, records.MostBlockGained));
        }
        if (records.TotalDamage > 0)
        {
            bars.Add(new DashboardBarItem(
                "total_damage",
                I18n.RecTotalDamage,
                (int)Math.Min(records.TotalDamage, int.MaxValue),
                FormatCompactNumber(records.TotalDamage)));
        }
        if (records.TotalFights > 0)
        {
            bars.Add(new DashboardBarItem("total_fights", I18n.RecTotalFights, records.TotalFights));
        }
        if (bars.Count == 0)
        {
            bars.Add(new DashboardBarItem("none", I18n.RecNoRecords, 0, string.Empty));
        }

        return bars;
    }

    private IReadOnlyList<DashboardBarItem> BuildOrbBars(Dictionary<string, PlayerStats> players)
    {
        if (_detailKey is null)
        {
            return players.Values
                .Where(player => player.OrbsChanneled > 0)
                .OrderByDescending(player => player.OrbsChanneled)
                .Select((player, index) => CreatePlayerSummaryBar(player, index, player.OrbsChanneled))
                .ToList();
        }

        return TryGetPlayer(players, _detailKey, out var playerStats)
            ? playerStats.OrbCount
                .OrderByDescending(entry => entry.Value)
                .Select(entry => new DashboardBarItem(entry.Key, entry.Key, entry.Value))
                .ToList()
            : Array.Empty<DashboardBarItem>();
    }

    private IReadOnlyList<DashboardBarItem> BuildThreatBars(Dictionary<string, PlayerStats> players)
    {
        if (_detailKey is null)
        {
            return players.Values
                .Select(player => new
                {
                    Player = player,
                    Total = player.MonsterMoveTargets.Values.Sum()
                })
                .Where(entry => entry.Total > 0)
                .OrderByDescending(entry => entry.Total)
                .Select((entry, index) => CreatePlayerSummaryBar(
                    entry.Player,
                    index,
                    entry.Total,
                    $"{entry.Total} {I18n.Targeted}"))
                .ToList();
        }

        return TryGetPlayer(players, _detailKey, out var playerStats)
            ? playerStats.MonsterMoveTargets
                .OrderByDescending(entry => entry.Value)
                .Select(entry => new DashboardBarItem(entry.Key, entry.Key, entry.Value))
                .ToList()
            : Array.Empty<DashboardBarItem>();
    }

    private IReadOnlyList<DashboardBarItem> BuildAdvancedBars(Dictionary<string, PlayerStats> players)
    {
        if (_detailKey is null)
        {
            return players.Values
                .Select(player => new
                {
                    Player = player,
                    Total = ComputeAdvancedTotal(player)
                })
                .Where(entry => entry.Total > 0)
                .OrderByDescending(entry => entry.Total)
                .Select((entry, index) => CreatePlayerSummaryBar(entry.Player, index, entry.Total))
                .ToList();
        }

        if (!TryGetPlayer(players, _detailKey, out var playerStats))
        {
            return Array.Empty<DashboardBarItem>();
        }

        var bars = new List<DashboardBarItem>();
        if (playerStats.CardsGenerated > 0)
        {
            bars.Add(new DashboardBarItem("generated_total", I18n.Generated, playerStats.CardsGenerated));
        }
        if (playerStats.AfflictionsApplied > 0)
        {
            bars.Add(new DashboardBarItem("afflictions_total", I18n.Afflictions, playerStats.AfflictionsApplied));
        }
        if (playerStats.SummonsCreated > 0)
        {
            bars.Add(new DashboardBarItem("summoned_total", I18n.Summoned, playerStats.SummonsCreated));
        }
        if (playerStats.StarsModified != 0)
        {
            bars.Add(new DashboardBarItem(
                "stars_total",
                I18n.Stars,
                Math.Abs(playerStats.StarsModified),
                playerStats.StarsModified > 0 ? $"+{playerStats.StarsModified}" : playerStats.StarsModified.ToString()));
        }

        bars.AddRange(playerStats.GeneratedCardCount
            .OrderByDescending(entry => entry.Value)
            .Take(5)
            .Select(entry => new DashboardBarItem(
                $"generated_{entry.Key}",
                "  " + entry.Key,
                entry.Value,
                $"{entry.Value}x {I18n.Generated}")));

        bars.AddRange(playerStats.AfflictionCount
            .OrderByDescending(entry => entry.Value)
            .Take(5)
            .Select(entry => new DashboardBarItem(
                $"affliction_{entry.Key}",
                "  " + entry.Key,
                entry.Value,
                $"{entry.Value}x {I18n.Afflictions}")));

        return bars;
    }

    private string BuildDashboardPanelTitle(
        DashboardView view,
        string? detailKey,
        Dictionary<string, PlayerStats> players)
    {
        if (detailKey is null)
        {
            return GetCategoryLabel(view);
        }

        return $"◀ {BuildDetailTitle(view, detailKey, players, GetCategoryLabel(view))}";
    }

    private bool CurrentCategoryHasDetail()
    {
        return CategoryHasDetail(_currentView);
    }

    private static bool CategoryHasDetail(DashboardView view)
    {
        return view is not DashboardView.Records;
    }

    private bool TryGetPlayer(Dictionary<string, PlayerStats> players, string key, out PlayerStats stats)
    {
        return players.TryGetValue(key, out stats!);
    }

    private string BuildPanelTitle()
    {
        var label = GetCategoryLabel(_currentView);
        return _detailKey is not null
            ? $"◀ {GetDetailTitle(label)}"
            : label;
    }

    private string GetDetailTitle(string fallback)
    {
        if (_detailKey is null)
        {
            return fallback;
        }

        return BuildDetailTitle(_currentView, _detailKey, ResolveCurrentContext().Players, fallback);
    }

    private string GetSegmentLabel()
    {
        return _selectedSegmentId switch
        {
            CurrentSegmentId => I18n.SegmentCurrent,
            OverallSegmentId => I18n.SegmentOverall,
            >= 0 when _selectedSegmentId < _sessions.ArchivedSegments.Count =>
                BuildArchivedSegmentLabel(_selectedSegmentId, _sessions.ArchivedSegments[_selectedSegmentId].EncounterKey),
            >= 0 => $"{I18n.SegmentFight} {_selectedSegmentId + 1}",
            _ => I18n.SegmentCurrent
        };
    }

    private static string BuildDetailTitle(
        DashboardView view,
        string detailKey,
        IReadOnlyDictionary<string, PlayerStats> players,
        string fallback)
    {
        if (!players.TryGetValue(detailKey, out var player))
        {
            return fallback;
        }

        return view switch
        {
            DashboardView.DeathLog => $"{player.DisplayName} — {I18n.DeathLog}",
            DashboardView.CombatLog => $"{player.DisplayName} — {I18n.CatCombatLog}",
            DashboardView.Energy => $"{player.DisplayName} — {I18n.Wasted}: {player.TotalEnergyWasted}",
            _ => player.DisplayName
        };
    }

    private static string BuildArchivedSegmentLabel(int archivedIndex, string encounterKey)
    {
        var encounterName = ResolveEncounterName(encounterKey);
        var fightLabel = $"{I18n.SegmentFight} {archivedIndex + 1}";
        return string.IsNullOrWhiteSpace(encounterName)
            ? fightLabel
            : $"{fightLabel}: {encounterName}";
    }

    private static string ResolveEncounterName(string encounterKey)
    {
        if (string.IsNullOrWhiteSpace(encounterKey))
        {
            return string.Empty;
        }

        try
        {
            var localized = new LocString("encounters", encounterKey + ".title");
            if (localized.Exists())
            {
                return localized.GetFormattedText();
            }
        }
        catch
        {
            // Fall back to the raw encounter key when localization is unavailable.
        }

        return encounterKey;
    }

    private string BuildEncounterDamageDealtBody(EncounterSession encounter)
    {
        if (encounter.Players.Count == 0)
        {
            return "Collecting combat history...";
        }

        var builder = new StringBuilder();
        foreach (var player in encounter.Players.Values.OrderByDescending(player => player.DamageDealt).Take(_settings.MaxBars))
        {
            builder.AppendLine($"{player.DisplayName}: {player.DamageDealt} dmg | hits {player.HitCount} | max {player.MaxSingleHit}");
            builder.AppendLine($"  by card: {FormatTopMap(player.DamageByCard, _settings.MaxBars)}");
        }

        return builder.ToString().TrimEnd();
    }

    private string BuildEncounterDamageTakenBody(EncounterSession encounter)
    {
        if (encounter.Players.Count == 0)
        {
            return "No incoming damage yet.";
        }

        var builder = new StringBuilder();
        foreach (var player in encounter.Players.Values.OrderByDescending(player => player.DamageTaken).Take(_settings.MaxBars))
        {
            builder.AppendLine($"{player.DisplayName}: taken {player.DamageTaken} | blocked by target {player.BlockedByTarget}");
            builder.AppendLine($"  incoming: {FormatTopMap(player.DamageBySource, _settings.MaxBars)}");
        }

        return builder.ToString().TrimEnd();
    }

    private string BuildEncounterCardUsageBody(EncounterSession encounter)
    {
        if (encounter.Players.Count == 0)
        {
            return "No card usage data yet.";
        }

        var builder = new StringBuilder();
        foreach (var player in encounter.Players.Values.OrderByDescending(player => player.CardsPlayed).Take(_settings.MaxBars))
        {
            builder.AppendLine($"{player.DisplayName}: played {player.CardsPlayed}");
            builder.AppendLine($"  top played: {FormatTopMap(player.CardPlayCount, _settings.MaxBars)}");
            builder.AppendLine($"  types: {FormatCardTypes(player.CardTypeCount)}");
        }

        return builder.ToString().TrimEnd();
    }

    private string BuildEncounterBlockBody(EncounterSession encounter)
    {
        if (encounter.Players.Count == 0)
        {
            return "No block data yet.";
        }

        var builder = new StringBuilder();
        foreach (var player in encounter.Players.Values.OrderByDescending(player => player.TotalBlockGained).Take(_settings.MaxBars))
        {
            builder.AppendLine($"{player.DisplayName}: block {player.TotalBlockGained}");
            builder.AppendLine($"  by card: {FormatTopMap(player.BlockByCard, _settings.MaxBars)}");
        }

        return builder.ToString().TrimEnd();
    }

    private string BuildEncounterEnergyBody(EncounterSession encounter)
    {
        if (encounter.Players.Count == 0)
        {
            return "No energy data yet.";
        }

        var builder = new StringBuilder();
        foreach (var player in encounter.Players.Values.OrderByDescending(player => player.TotalEnergySpent).Take(_settings.MaxBars))
        {
            builder.AppendLine($"{player.DisplayName}: spent {player.TotalEnergySpent} | wasted {player.TotalEnergyWasted}");
            builder.AppendLine($"  by card: {FormatTopMap(player.EnergySpentByCard, _settings.MaxBars)}");
        }

        return builder.ToString().TrimEnd();
    }

    private string BuildEncounterEfficiencyBody(EncounterSession encounter)
    {
        if (encounter.Players.Count == 0)
        {
            return "No efficiency data yet.";
        }

        var builder = new StringBuilder();
        foreach (var player in encounter.Players.Values
                     .OrderByDescending(player => ComputeEfficiency(player))
                     .Take(_settings.MaxBars))
        {
            builder.AppendLine($"{player.DisplayName}: dmg/energy {ComputeEfficiency(player):0.00}");
            builder.AppendLine($"  damage {player.DamageDealt} | energy {player.TotalEnergySpent} | top damage {FormatTopMap(player.DamageByCard, 3)}");
        }

        return builder.ToString().TrimEnd();
    }

    private string BuildEncounterPotionsBody(EncounterSession encounter)
    {
        if (encounter.Players.Count == 0)
        {
            return "No potion data yet.";
        }

        var builder = new StringBuilder();
        foreach (var player in encounter.Players.Values.OrderByDescending(player => player.PotionsUsed).Take(_settings.MaxBars))
        {
            builder.AppendLine($"{player.DisplayName}: potions {player.PotionsUsed}");
            builder.AppendLine($"  by potion: {FormatTopMap(player.PotionUseCount, _settings.MaxBars)}");
        }

        return builder.ToString().TrimEnd();
    }

    private string BuildEncounterDebuffsBody(EncounterSession encounter)
    {
        if (encounter.Players.Count == 0)
        {
            return "No debuff data yet.";
        }

        var builder = new StringBuilder();
        foreach (var player in encounter.Players.Values.OrderByDescending(player => player.DebuffsApplied).Take(_settings.MaxBars))
        {
            builder.AppendLine($"{player.DisplayName}: debuffs {player.DebuffsApplied}");
            builder.AppendLine($"  by power: {FormatTopMap(player.PowerDebuffCount, _settings.MaxBars)}");
        }

        return builder.ToString().TrimEnd();
    }

    private string BuildEncounterDeathLogBody(EncounterSession encounter)
    {
        if (encounter.DeathSnapshots.Count == 0)
        {
            return "No deaths recorded in this combat.";
        }

        var builder = new StringBuilder();
        foreach (var (playerKey, events) in encounter.DeathSnapshots.Take(_settings.MaxBars))
        {
            builder.AppendLine($"{playerKey}:");
            foreach (var item in events.TakeLast(6))
            {
                builder.AppendLine($"  T{item.Turn} {FormatTimelineType(item.Type)} {item.Label} {item.Value}");
            }
        }

        return builder.ToString().TrimEnd();
    }

    private string BuildEncounterCardFlowBody(EncounterSession encounter)
    {
        if (encounter.Players.Count == 0)
        {
            return "No card flow data yet.";
        }

        var builder = new StringBuilder();
        foreach (var player in encounter.Players.Values.OrderByDescending(player => player.CardsDrawn + player.CardsDiscarded + player.CardsExhausted).Take(_settings.MaxBars))
        {
            builder.AppendLine($"{player.DisplayName}: drawn {player.CardsDrawn} | discarded {player.CardsDiscarded} | exhausted {player.CardsExhausted}");
            builder.AppendLine($"  draw: {FormatTopMap(player.DrawCount, _settings.MaxBars)}");
            builder.AppendLine($"  discard: {FormatTopMap(player.DiscardCount, _settings.MaxBars)}");
            builder.AppendLine($"  exhaust: {FormatTopMap(player.ExhaustCount, _settings.MaxBars)}");
        }

        return builder.ToString().TrimEnd();
    }

    private string BuildEncounterCombatLogBody(EncounterSession encounter)
    {
        if (encounter.Timeline.Count == 0)
        {
            return "Combat log is empty.";
        }

        var builder = new StringBuilder();
        foreach (var item in encounter.Timeline.TakeLast(_settings.MaxBars))
        {
            builder.AppendLine($"T{item.Turn} {FormatTimelineType(item.Type)} {item.ActorKey} | {item.Label} | {item.Value}");
        }

        return builder.ToString().TrimEnd();
    }

    private string BuildEncounterOrbsBody(EncounterSession encounter)
    {
        if (encounter.Players.Count == 0)
        {
            return "No orb data yet.";
        }

        var builder = new StringBuilder();
        foreach (var player in encounter.Players.Values.OrderByDescending(player => player.OrbsChanneled).Take(_settings.MaxBars))
        {
            builder.AppendLine($"{player.DisplayName}: {player.OrbsChanneled} {I18n.CatOrbs.ToLowerInvariant()}");
            builder.AppendLine($"  by orb: {FormatTopMap(player.OrbCount, _settings.MaxBars)}");
        }

        return builder.ToString().TrimEnd();
    }

    private string BuildEncounterThreatBody(EncounterSession encounter)
    {
        if (encounter.Players.Count == 0)
        {
            return "No threat data yet.";
        }

        var builder = new StringBuilder();
        foreach (var player in encounter.Players.Values
                     .OrderByDescending(player => player.MonsterMoveTargets.Values.Sum())
                     .Take(_settings.MaxBars))
        {
            var total = player.MonsterMoveTargets.Values.Sum();
            builder.AppendLine($"{player.DisplayName}: {total} {I18n.Targeted}");
            builder.AppendLine($"  moves: {FormatTopMap(player.MonsterMoveTargets, _settings.MaxBars)}");
        }

        return builder.ToString().TrimEnd();
    }

    private string BuildEncounterAdvancedBody(EncounterSession encounter)
    {
        if (encounter.Players.Count == 0)
        {
            return "No advanced data yet.";
        }

        var builder = new StringBuilder();
        foreach (var player in encounter.Players.Values
                     .OrderByDescending(ComputeAdvancedTotal)
                     .Take(_settings.MaxBars))
        {
            builder.AppendLine(
                $"{player.DisplayName}: {I18n.Generated} {player.CardsGenerated} | {I18n.Afflictions} {player.AfflictionsApplied} | {I18n.Summoned} {player.SummonsCreated} | {I18n.Stars} {player.StarsModified}");
            builder.AppendLine($"  generated: {FormatTopMap(player.GeneratedCardCount, _settings.MaxBars)}");
            builder.AppendLine($"  afflictions: {FormatTopMap(player.AfflictionCount, _settings.MaxBars)}");
        }

        return builder.ToString().TrimEnd();
    }

    private string BuildOverallDamageDealtBody(RunStats overall)
    {
        return BuildPlayerRankingBody(overall.OverallPlayers.Values.OrderByDescending(player => player.DamageDealt),
            player => $"{player.DisplayName}: {player.DamageDealt} dmg | max {player.MaxSingleHit}",
            player => $"  by card: {FormatTopMap(player.DamageByCard, _settings.MaxBars)}");
    }

    private string BuildOverallDamageTakenBody(RunStats overall)
    {
        return BuildPlayerRankingBody(overall.OverallPlayers.Values.OrderByDescending(player => player.DamageTaken),
            player => $"{player.DisplayName}: taken {player.DamageTaken}",
            player => $"  incoming: {FormatTopMap(player.DamageBySource, _settings.MaxBars)}");
    }

    private string BuildOverallCardUsageBody(RunStats overall)
    {
        return BuildPlayerRankingBody(overall.OverallPlayers.Values.OrderByDescending(player => player.CardsPlayed),
            player => $"{player.DisplayName}: played {player.CardsPlayed}",
            player => $"  top played: {FormatTopMap(player.CardPlayCount, _settings.MaxBars)}");
    }

    private string BuildOverallBlockBody(RunStats overall)
    {
        return BuildPlayerRankingBody(overall.OverallPlayers.Values.OrderByDescending(player => player.TotalBlockGained),
            player => $"{player.DisplayName}: block {player.TotalBlockGained}",
            player => $"  by card: {FormatTopMap(player.BlockByCard, _settings.MaxBars)}");
    }

    private string BuildOverallEnergyBody(RunStats overall)
    {
        return BuildPlayerRankingBody(overall.OverallPlayers.Values.OrderByDescending(player => player.TotalEnergySpent),
            player => $"{player.DisplayName}: spent {player.TotalEnergySpent} | wasted {player.TotalEnergyWasted}",
            player => $"  by card: {FormatTopMap(player.EnergySpentByCard, _settings.MaxBars)}");
    }

    private string BuildOverallEfficiencyBody(RunStats overall)
    {
        return BuildPlayerRankingBody(overall.OverallPlayers.Values.OrderByDescending(player => ComputeEfficiency(player)),
            player => $"{player.DisplayName}: dmg/energy {ComputeEfficiency(player):0.00}",
            player => $"  damage {player.DamageDealt} | energy {player.TotalEnergySpent}");
    }

    private string BuildOverallPotionsBody(RunStats overall)
    {
        return BuildPlayerRankingBody(overall.OverallPlayers.Values.OrderByDescending(player => player.PotionsUsed),
            player => $"{player.DisplayName}: potions {player.PotionsUsed}",
            player => $"  by potion: {FormatTopMap(player.PotionUseCount, _settings.MaxBars)}");
    }

    private string BuildOverallDebuffsBody(RunStats overall)
    {
        return BuildPlayerRankingBody(overall.OverallPlayers.Values.OrderByDescending(player => player.DebuffsApplied),
            player => $"{player.DisplayName}: debuffs {player.DebuffsApplied}",
            player => $"  by power: {FormatTopMap(player.PowerDebuffCount, _settings.MaxBars)}");
    }

    private string BuildOverallCardFlowBody(RunStats overall)
    {
        return BuildPlayerRankingBody(overall.OverallPlayers.Values.OrderByDescending(player => player.CardsDrawn + player.CardsDiscarded + player.CardsExhausted),
            player => $"{player.DisplayName}: drawn {player.CardsDrawn} | discarded {player.CardsDiscarded} | exhausted {player.CardsExhausted}",
            player => $"  draw/discard/exhaust: {FormatTopMap(player.DrawCount, 2)} / {FormatTopMap(player.DiscardCount, 2)} / {FormatTopMap(player.ExhaustCount, 2)}");
    }

    private string BuildOverallOrbsBody(RunStats overall)
    {
        return BuildPlayerRankingBody(overall.OverallPlayers.Values.OrderByDescending(player => player.OrbsChanneled),
            player => $"{player.DisplayName}: {player.OrbsChanneled} {I18n.CatOrbs.ToLowerInvariant()}",
            player => $"  by orb: {FormatTopMap(player.OrbCount, _settings.MaxBars)}");
    }

    private string BuildOverallThreatBody(RunStats overall)
    {
        return BuildPlayerRankingBody(overall.OverallPlayers.Values.OrderByDescending(player => player.MonsterMoveTargets.Values.Sum()),
            player => $"{player.DisplayName}: {player.MonsterMoveTargets.Values.Sum()} {I18n.Targeted}",
            player => $"  moves: {FormatTopMap(player.MonsterMoveTargets, _settings.MaxBars)}");
    }

    private string BuildOverallAdvancedBody(RunStats overall)
    {
        return BuildPlayerRankingBody(overall.OverallPlayers.Values.OrderByDescending(ComputeAdvancedTotal),
            player => $"{player.DisplayName}: {I18n.Generated} {player.CardsGenerated} | {I18n.Afflictions} {player.AfflictionsApplied} | {I18n.Summoned} {player.SummonsCreated} | {I18n.Stars} {player.StarsModified}",
            player => $"  generated/afflictions: {FormatTopMap(player.GeneratedCardCount, 2)} / {FormatTopMap(player.AfflictionCount, 2)}");
    }

    private string BuildArchivedDeathLogBody()
    {
        if (_sessions.ArchivedSegments.Count == 0)
        {
            return "No archived deaths yet.";
        }

        var builder = new StringBuilder();
        foreach (var segment in _sessions.ArchivedSegments.Where(segment => segment.DeathSnapshots.Count > 0).TakeLast(_settings.MaxBars).Reverse())
        {
            builder.AppendLine(segment.EncounterKey);
            foreach (var (playerKey, events) in segment.DeathSnapshots)
            {
                builder.AppendLine($"  {playerKey}:");
                foreach (var item in events.TakeLast(3))
                {
                    builder.AppendLine($"    T{item.Turn} {FormatTimelineType(item.Type)} {item.Label} {item.Value}");
                }
            }
        }

        return builder.ToString().TrimEnd();
    }

    private string BuildArchivedTimelineBody()
    {
        if (_sessions.ArchivedSegments.Count == 0)
        {
            return "No archived encounters yet.";
        }

        var builder = new StringBuilder();
        foreach (var segment in _sessions.ArchivedSegments.TakeLast(_settings.MaxBars).Reverse())
        {
            var totalDamage = segment.Players.Values.Sum(player => player.DamageDealt);
            var totalTaken = segment.Players.Values.Sum(player => player.DamageTaken);
            builder.AppendLine($"{segment.EncounterKey}: turns {segment.TurnCount} | dmg {totalDamage} | taken {totalTaken}");
            foreach (var item in segment.Timeline.TakeLast(3))
            {
                builder.AppendLine($"  T{item.Turn} {FormatTimelineType(item.Type)} {item.Label} {item.Value}");
            }
        }

        return builder.ToString().TrimEnd();
    }

    private static string BuildRecordsBody(PersonalRecords records)
    {
        if (records.TotalFights == 0)
        {
            return "No records yet.";
        }

        var builder = new StringBuilder();
        builder.AppendLine($"Highest Hit: {records.HighestHit} via {records.HighestHitCard}");
        builder.AppendLine($"Most Fight Damage: {records.MostFightDamage}");
        builder.AppendLine($"Best Turn Damage: {records.BestTurnDamage}");
        builder.AppendLine($"Most Cards Played: {records.MostCardsPlayed}");
        builder.AppendLine($"Most Block Gained: {records.MostBlockGained}");
        builder.AppendLine($"Total Damage: {records.TotalDamage}");
        builder.AppendLine($"Total Fights: {records.TotalFights}");
        return builder.ToString().TrimEnd();
    }

    private string BuildPlayerRankingBody(
        IEnumerable<PlayerStats> players,
        Func<PlayerStats, string> headline,
        Func<PlayerStats, string> details)
    {
        var selected = players.Take(_settings.MaxBars).ToList();
        if (selected.Count == 0)
        {
            return "No archived combat data yet.";
        }

        var builder = new StringBuilder();
        foreach (var player in selected)
        {
            builder.AppendLine(headline(player));
            builder.AppendLine(details(player));
        }

        return builder.ToString().TrimEnd();
    }

    private static string FormatTopMap<TKey>(IEnumerable<KeyValuePair<TKey, int>> map, int take = 3)
    {
        var items = map
            .OrderByDescending(entry => entry.Value)
            .Take(take)
            .Select(entry => $"{entry.Key} {entry.Value}")
            .ToArray();

        return items.Length == 0 ? "-" : string.Join(", ", items);
    }

    private static string FormatCardTypes(IEnumerable<KeyValuePair<CardType, int>> map)
    {
        var items = map
            .Where(entry => entry.Value > 0)
            .OrderByDescending(entry => entry.Value)
            .Select(entry => $"{entry.Key} {entry.Value}")
            .ToArray();

        return items.Length == 0 ? "-" : string.Join(", ", items);
    }

    private static string FormatTimelineType(TimelineEventType type)
    {
        return type switch
        {
            TimelineEventType.DamageDealt => "DMG",
            TimelineEventType.DamageTaken => "TAKEN",
            TimelineEventType.BlockGained => "BLOCK",
            TimelineEventType.CardPlayed => "PLAY",
            TimelineEventType.CardDrawn => "DRAW",
            TimelineEventType.CardDiscarded => "DISC",
            TimelineEventType.CardExhausted => "EXH",
            TimelineEventType.PotionUsed => "POT",
            TimelineEventType.DebuffApplied => "DEBUFF",
            TimelineEventType.MonsterMove => "MOVE",
            TimelineEventType.OrbChanneled => "ORB",
            TimelineEventType.CardGenerated => "GEN",
            TimelineEventType.AfflictionApplied => "AFF",
            TimelineEventType.EnergySpent => "ENERGY",
            TimelineEventType.StarsModified => "STAR",
            TimelineEventType.Summoned => "SUMMON",
            _ => type.ToString()
        };
    }

    private static bool IsLegacyCombatLogEvent(TimelineEventType type)
    {
        return type is TimelineEventType.DamageDealt
            or TimelineEventType.DamageTaken
            or TimelineEventType.BlockGained
            or TimelineEventType.CardPlayed
            or TimelineEventType.PotionUsed
            or TimelineEventType.DebuffApplied;
    }

    private static string FormatLegacyEventPrefix(TimelineEventType type, int turn)
    {
        return type switch
        {
            TimelineEventType.DamageTaken => $"T{turn} [-]",
            TimelineEventType.DamageDealt => $"T{turn} [+]",
            TimelineEventType.BlockGained => $"T{turn} [B]",
            TimelineEventType.CardPlayed => $"T{turn} [C]",
            TimelineEventType.PotionUsed => $"T{turn} [P]",
            TimelineEventType.DebuffApplied => $"T{turn} [D]",
            _ => $"T{turn}"
        };
    }

    private string BuildFooter(string status)
    {
        return string.Empty;
    }

    private static double ComputeEfficiency(PlayerStats player)
    {
        return player.TotalEnergySpent <= 0
            ? player.DamageDealt
            : (double)player.DamageDealt / player.TotalEnergySpent;
    }

    private static int ComputeAdvancedTotal(PlayerStats player)
    {
        return player.CardsGenerated
            + player.AfflictionsApplied
            + player.SummonsCreated
            + Math.Abs(player.StarsModified);
    }

    private string BuildTitle(string? encounterKey = null)
    {
        var scopeLabel = _currentScope == DashboardScope.Current ? "Current" : "Overall";
        var categoryLabel = GetCategoryLabel(_currentView);
        return encounterKey is null
            ? $"Skada: Spire Edition [{scopeLabel}] {categoryLabel}"
            : $"Skada: Spire Edition [{scopeLabel}] {categoryLabel} | {encounterKey}";
    }

    private string GetCategoryLabel(DashboardView view)
    {
        return view switch
        {
            DashboardView.DamageDealt => I18n.CatDamageDealt,
            DashboardView.DamageTaken => I18n.CatDamageTaken,
            DashboardView.Dpt => I18n.CatDpt,
            DashboardView.CardUsage => I18n.CatCardUsage,
            DashboardView.Block => I18n.CatBlock,
            DashboardView.Energy => I18n.CatEnergy,
            DashboardView.Efficiency => I18n.CatEfficiency,
            DashboardView.Overkill => I18n.CatOverkill,
            DashboardView.Potions => I18n.CatPotions,
            DashboardView.Debuffs => I18n.CatDebuffs,
            DashboardView.DeathLog => I18n.CatDeathLog,
            DashboardView.CardFlow => I18n.CatCardFlow,
            DashboardView.CombatLog => I18n.CatCombatLog,
            DashboardView.Records => I18n.CatRecords,
            DashboardView.Orbs => I18n.CatOrbs,
            DashboardView.Threat => I18n.CatThreat,
            DashboardView.Advanced => I18n.CatAdvanced,
            _ => view.ToString()
        };
    }

    private static DashboardBarItem CreatePlayerSummaryBar(
        PlayerStats player,
        int? index,
        int value,
        string? displayText = null,
        bool clickable = true)
    {
        var label = index.HasValue
            ? $"{index.Value + 1}. {player.DisplayName}"
            : player.DisplayName;
        return new DashboardBarItem(
            player.Key,
            label,
            value,
            displayText,
            clickable,
            PlayerVisuals.GetPlayerColor(player.Key, player.CharacterColor),
            PlayerVisuals.LoadCharacterIcon(player.CharacterId));
    }

    private int GetDptTurnCount()
    {
        if (_selectedSegmentId >= 0 && _selectedSegmentId < _sessions.ArchivedSegments.Count)
        {
            return Math.Max(_sessions.ArchivedSegments[_selectedSegmentId].TurnCount, 1);
        }

        if (_selectedSegmentId == OverallSegmentId)
        {
            var currentTurns = _sessions.CurrentEncounter?.CurrentTurn ?? 0;
            var archivedTurns = _sessions.ArchivedSegments.Sum(segment => segment.TurnCount);
            return Math.Max(currentTurns + archivedTurns, 1);
        }

        return Math.Max(_sessions.CurrentEncounter?.CurrentTurn ?? 0, 1);
    }

    private static string FormatCompactNumber(long value)
    {
        return value switch
        {
            >= 1_000_000 => $"{value / 1_000_000d:0.0}M",
            >= 10_000 => $"{value / 1_000d:0.0}K",
            _ => value.ToString()
        };
    }

    private readonly record struct DashboardContext(
        Dictionary<string, PlayerStats> Players,
        IReadOnlyList<TimelineEvent>? Timeline,
        IReadOnlyDictionary<string, List<TimelineEvent>>? DeathSnapshots,
        RunStats? Overall);
}
