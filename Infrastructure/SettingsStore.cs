using DamageMeterRebuilt.Domain;
using DamageMeterRebuilt.Views;
using Godot;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DamageMeterRebuilt.Infrastructure;

internal sealed class SettingsStore
{
    private readonly string _settingsPath;
    private readonly string _legacySettingsPath;
    private DashboardSettings _settings;

    public SettingsStore()
    {
        var userDir = ProjectSettings.GlobalizePath("user://");
        _settingsPath = Path.Combine(userDir, "DamageMeterRebuilt_settings.json");
        _legacySettingsPath = Path.Combine(userDir, "DamageMeter_settings.json");
        _settings = Load();
    }

    public bool OverlayVisible => _settings.OverlayVisible;

    public Vector2 PanelPosition => new(_settings.PanelX, _settings.PanelY);

    public bool UseAnchoredPanelPosition => _settings.UseAnchoredPosition;

    public DashboardView SelectedView => Enum.IsDefined(typeof(DashboardView), _settings.SelectedView)
        ? (DashboardView)_settings.SelectedView
        : DashboardView.DamageDealt;

    public DashboardScope SelectedScope => Enum.IsDefined(typeof(DashboardScope), _settings.SelectedScope)
        ? (DashboardScope)_settings.SelectedScope
        : DashboardScope.Current;

    public float Scale => _settings.Scale;

    public float Opacity => _settings.Opacity;

    public int MaxBars => Math.Max(_settings.MaxBars, 1);

    public bool AutoResetOnNewRun => _settings.AutoReset;

    public PersonalRecords GetRecords()
    {
        return ToPersonalRecords(_settings.Records);
    }

    public void ResetPanelPosition()
    {
        if (_settings.UseAnchoredPosition)
        {
            return;
        }

        _settings = _settings with
        {
            UseAnchoredPosition = true,
            PanelX = DashboardSettings.Default.PanelX,
            PanelY = DashboardSettings.Default.PanelY
        };
        Save();
    }

    public void UpdateOverlayVisible(bool visible)
    {
        if (_settings.OverlayVisible == visible)
        {
            return;
        }

        _settings = _settings with { OverlayVisible = visible };
        Save();
    }

    public void UpdatePanelPosition(Vector2 position)
    {
        if (!_settings.UseAnchoredPosition
            && Math.Abs(_settings.PanelX - position.X) < 0.01f
            && Math.Abs(_settings.PanelY - position.Y) < 0.01f)
        {
            return;
        }

        _settings = _settings with
        {
            UseAnchoredPosition = false,
            PanelX = position.X,
            PanelY = position.Y
        };
        Save();
    }

    public void UpdateSelectedView(DashboardView view)
    {
        if (_settings.SelectedView == (int)view)
        {
            return;
        }

        _settings = _settings with { SelectedView = (int)view };
        Save();
    }

    public void UpdateSelectedScope(DashboardScope scope)
    {
        if (_settings.SelectedScope == (int)scope)
        {
            return;
        }

        _settings = _settings with { SelectedScope = (int)scope };
        Save();
    }

    public void UpdateScale(float scale)
    {
        var clamped = Mathf.Clamp(scale, 0.5f, 2f);
        if (Math.Abs(_settings.Scale - clamped) < 0.01f)
        {
            return;
        }

        _settings = _settings with { Scale = clamped };
        Save();
    }

    public void UpdateOpacity(float opacity)
    {
        var clamped = Mathf.Clamp(opacity, 0.3f, 1f);
        if (Math.Abs(_settings.Opacity - clamped) < 0.01f)
        {
            return;
        }

        _settings = _settings with { Opacity = clamped };
        Save();
    }

    public void UpdateMaxBars(int maxBars)
    {
        var normalized = Math.Clamp(maxBars, 3, 30);
        if (_settings.MaxBars == normalized)
        {
            return;
        }

        _settings = _settings with { MaxBars = normalized };
        Save();
    }

    public void UpdateAutoReset(bool autoReset)
    {
        if (_settings.AutoReset == autoReset)
        {
            return;
        }

        _settings = _settings with { AutoReset = autoReset };
        Save();
    }

    public void SaveRecords(PersonalRecords records)
    {
        var normalized = FromPersonalRecords(records);
        if (_settings.Records == normalized)
        {
            return;
        }

        _settings = _settings with { Records = normalized };
        Save();
    }

    private DashboardSettings Load()
    {
        try
        {
            if (!File.Exists(_settingsPath))
            {
                var imported = TryImportLegacySettings();
                if (imported is not null)
                {
                    SaveImported(imported);
                    return imported;
                }

                return DashboardSettings.Default;
            }

            var json = File.ReadAllText(_settingsPath);
            var loaded = JsonSerializer.Deserialize<DashboardSettingsFile>(json);
            var normalized = Normalize(loaded);
            if (loaded is null || loaded.NeedsUpgrade())
            {
                SaveImported(normalized);
            }

            return normalized;
        }
        catch (Exception ex)
        {
            LoggerAdapter.Error("Failed to load settings. Using defaults.", ex);
            return DashboardSettings.Default;
        }
    }

    private DashboardSettings? TryImportLegacySettings()
    {
        try
        {
            if (!File.Exists(_legacySettingsPath))
            {
                return null;
            }

            var json = File.ReadAllText(_legacySettingsPath);
            var legacy = JsonSerializer.Deserialize<LegacyDamageMeterSettings>(json);
            if (legacy is null)
            {
                return null;
            }

            return DashboardSettings.Default with
            {
                PanelX = legacy.PanelX ?? DashboardSettings.Default.PanelX,
                PanelY = legacy.PanelY ?? DashboardSettings.Default.PanelY,
                UseAnchoredPosition = legacy.PanelX is null || legacy.PanelY is null,
                Scale = legacy.Scale,
                Opacity = legacy.Opacity,
                MaxBars = legacy.MaxBars,
                AutoReset = legacy.AutoReset,
                Records = legacy.Records is null ? DashboardRecords.Default : FromLegacyRecords(legacy.Records)
            };
        }
        catch (Exception ex)
        {
            LoggerAdapter.Error("Failed to import legacy DamageMeter settings.", ex);
            return null;
        }
    }

    private void SaveImported(DashboardSettings imported)
    {
        _settings = imported;
        Save();
    }

    private static DashboardSettings Normalize(DashboardSettingsFile? loaded)
    {
        var defaults = DashboardSettings.Default;
        if (loaded is null)
        {
            return defaults;
        }

        return defaults with
        {
            OverlayVisible = loaded.OverlayVisible ?? defaults.OverlayVisible,
            PanelX = loaded.PanelX ?? defaults.PanelX,
            PanelY = loaded.PanelY ?? defaults.PanelY,
            UseAnchoredPosition = loaded.UseAnchoredPosition
                ?? (loaded.PanelX is null || loaded.PanelY is null
                    ? defaults.UseAnchoredPosition
                    : false),
            SelectedView = loaded.SelectedView ?? defaults.SelectedView,
            SelectedScope = loaded.SelectedScope ?? defaults.SelectedScope,
            Scale = loaded.Scale is > 0f ? Mathf.Clamp(loaded.Scale.Value, 0.5f, 2f) : defaults.Scale,
            Opacity = loaded.Opacity is > 0f ? Mathf.Clamp(loaded.Opacity.Value, 0.3f, 1f) : defaults.Opacity,
            MaxBars = loaded.MaxBars is > 0 ? Math.Clamp(loaded.MaxBars.Value, 3, 30) : defaults.MaxBars,
            AutoReset = loaded.AutoReset ?? defaults.AutoReset,
            Records = loaded.Records is null ? defaults.Records : NormalizeRecords(loaded.Records)
        };
    }

    private static PersonalRecords ToPersonalRecords(DashboardRecords records)
    {
        return new PersonalRecords
        {
            HighestHit = records.HighestHit,
            HighestHitCard = records.HighestHitCard,
            MostFightDamage = records.MostFightDamage,
            BestTurnDamage = records.BestTurnDamage,
            MostCardsPlayed = records.MostCardsPlayed,
            MostBlockGained = records.MostBlockGained,
            TotalDamage = records.TotalDamage,
            TotalFights = records.TotalFights
        };
    }

    private static DashboardRecords FromPersonalRecords(PersonalRecords records)
    {
        return new DashboardRecords(
            records.HighestHit,
            records.HighestHitCard,
            records.MostFightDamage,
            records.BestTurnDamage,
            records.MostCardsPlayed,
            records.MostBlockGained,
            records.TotalDamage,
            records.TotalFights);
    }

    private static DashboardRecords NormalizeRecords(DashboardRecordsFile records)
    {
        return new DashboardRecords(
            records.HighestHit ?? 0,
            records.HighestHitCard ?? string.Empty,
            records.MostFightDamage ?? 0,
            records.BestTurnDamage ?? 0,
            records.MostCardsPlayed ?? 0,
            records.MostBlockGained ?? 0,
            records.TotalDamage ?? 0L,
            records.TotalFights ?? 0);
    }

    private static DashboardRecords FromLegacyRecords(LegacyDamageMeterRecords records)
    {
        return new DashboardRecords(
            records.HighestHit ?? 0,
            records.HighestHitCard ?? string.Empty,
            records.MostFightDamage ?? 0,
            records.BestTurnDamage ?? 0,
            records.MostCardsPlayed ?? 0,
            records.MostBlockGained ?? 0,
            records.TotalDamage ?? 0L,
            records.TotalFights ?? 0);
    }

    private void Save()
    {
        try
        {
            var directory = Path.GetDirectoryName(_settingsPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(_settings, JsonOptions);
            File.WriteAllText(_settingsPath, json);
        }
        catch (Exception ex)
        {
            LoggerAdapter.Error("Failed to save settings.", ex);
        }
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never
    };

    private sealed record DashboardSettings(
        bool OverlayVisible,
        float PanelX,
        float PanelY,
        bool UseAnchoredPosition,
        int SelectedView,
        int SelectedScope,
        float Scale,
        float Opacity,
        int MaxBars,
        bool AutoReset,
        DashboardRecords Records)
    {
        public static DashboardSettings Default { get; } = new(
            true,
            0f,
            0f,
            true,
            (int)DashboardView.DamageDealt,
            (int)DashboardScope.Current,
            1f,
            0.88f,
            10,
            true,
            DashboardRecords.Default);
    }

    private sealed record DashboardSettingsFile(
        bool? OverlayVisible,
        float? PanelX,
        float? PanelY,
        bool? UseAnchoredPosition,
        int? SelectedView,
        int? SelectedScope,
        float? Scale,
        float? Opacity,
        int? MaxBars,
        bool? AutoReset,
        DashboardRecordsFile? Records)
    {
        public bool NeedsUpgrade()
        {
            return OverlayVisible is null
                || SelectedView is null
                || SelectedScope is null
                || Scale is null
                || Opacity is null
                || MaxBars is null
                || AutoReset is null
                || Records is null;
        }
    }

    private sealed record LegacyDamageMeterSettings(
        [property: JsonPropertyName("panel_x")] float? PanelX,
        [property: JsonPropertyName("panel_y")] float? PanelY,
        [property: JsonPropertyName("scale")] float Scale,
        [property: JsonPropertyName("opacity")] float Opacity,
        [property: JsonPropertyName("max_bars")] int MaxBars,
        [property: JsonPropertyName("auto_reset")] bool AutoReset,
        [property: JsonPropertyName("records")] LegacyDamageMeterRecords? Records);

    private sealed record DashboardRecords(
        int HighestHit,
        string HighestHitCard,
        int MostFightDamage,
        int BestTurnDamage,
        int MostCardsPlayed,
        int MostBlockGained,
        long TotalDamage,
        int TotalFights)
    {
        public static DashboardRecords Default { get; } = new(0, string.Empty, 0, 0, 0, 0, 0L, 0);
    }

    private sealed record DashboardRecordsFile(
        int? HighestHit,
        string? HighestHitCard,
        int? MostFightDamage,
        int? BestTurnDamage,
        int? MostCardsPlayed,
        int? MostBlockGained,
        long? TotalDamage,
        int? TotalFights);

    private sealed record LegacyDamageMeterRecords(
        [property: JsonPropertyName("highest_hit")] int? HighestHit,
        [property: JsonPropertyName("highest_hit_card")] string? HighestHitCard,
        [property: JsonPropertyName("most_fight_damage")] int? MostFightDamage,
        [property: JsonPropertyName("best_turn_damage")] int? BestTurnDamage,
        [property: JsonPropertyName("most_cards_played")] int? MostCardsPlayed,
        [property: JsonPropertyName("most_block_gained")] int? MostBlockGained,
        [property: JsonPropertyName("total_damage")] long? TotalDamage,
        [property: JsonPropertyName("total_fights")] int? TotalFights);
}
