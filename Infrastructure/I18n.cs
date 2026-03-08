using System.Reflection;
using System.Text.Json;
using Godot;
using MegaCrit.Sts2.Core.Localization;

namespace DamageMeterRebuilt.Infrastructure;

internal static class I18n
{
    private const string DefaultLanguage = "en";
    private const string ResourcePrefix = "DamageMeterRebuilt.localization.";

    private static Dictionary<string, string> _translations = new(StringComparer.OrdinalIgnoreCase);
    private static string? _loadedLanguage;

    public static string CatDamageDealt => Get("cat_damage_dealt", "Damage Dealt");
    public static string CatDamageTaken => Get("cat_damage_taken", "Damage Taken");
    public static string CatDpt => Get("cat_dpt", "DPT");
    public static string CatCardUsage => Get("cat_card_usage", "Cards Played");
    public static string CatBlock => Get("cat_block", "Block");
    public static string CatEnergy => Get("cat_energy", "Energy");
    public static string CatEfficiency => Get("cat_efficiency", "DMG/Energy");
    public static string CatOverkill => Get("cat_overkill", "Overkill");
    public static string CatPotions => Get("cat_potions", "Potions");
    public static string CatDebuffs => Get("cat_debuffs", "Debuffs Applied");
    public static string CatDeathLog => Get("cat_death_log", "Death Log");
    public static string CatCardFlow => Get("cat_card_flow", "Card Flow");
    public static string CatCombatLog => Get("cat_combat_log", "Combat Log");
    public static string CatRecords => Get("cat_records", "Records");
    public static string CatOrbs => Get("cat_orbs", "Orbs");
    public static string CatThreat => Get("cat_threat", "Threat");
    public static string CatAdvanced => Get("cat_advanced", "Advanced");
    public static string Turn => Get("turn", "Turn");
    public static string Wasted => Get("wasted", "wasted");
    public static string TotalWasted => Get("total_wasted", "Total Wasted");
    public static string Blocked => Get("blocked", "Blocked");
    public static string Generated => Get("generated", "Generated");
    public static string Afflictions => Get("afflictions", "Afflictions");
    public static string Summoned => Get("summoned", "Summoned");
    public static string Stars => Get("stars", "Stars");
    public static string Targeted => Get("targeted", "Targeted");
    public static string WaitingForCombat => Get("waiting", "Waiting for combat...");
    public static string SegmentCurrent => Get("segment_current", "Current");
    public static string SegmentOverall => Get("segment_overall", "Overall");
    public static string SegmentFight => Get("segment_fight", "Fight");
    public static string DeathLog => Get("death_log", "Death Log");
    public static string Dead => Get("dead", "DEAD");
    public static string Alive => Get("alive", "Alive");
    public static string Drawn => Get("drawn", "Drawn");
    public static string Discarded => Get("discarded", "Discarded");
    public static string Exhausted => Get("exhausted", "Exhausted");
    public static string Events => Get("events", "events");
    public static string RecHighestHit => Get("rec_highest_hit", "Highest Hit");
    public static string RecMostFightDmg => Get("rec_most_fight_dmg", "Most Fight DMG");
    public static string RecBestTurnDmg => Get("rec_best_turn_dmg", "Best Turn DMG");
    public static string RecMostCards => Get("rec_most_cards", "Most Cards Played");
    public static string RecMostBlock => Get("rec_most_block", "Most Block Gained");
    public static string RecTotalDamage => Get("rec_total_damage", "Total Damage");
    public static string RecTotalFights => Get("rec_total_fights", "Fights Completed");
    public static string RecNoRecords => Get("rec_no_records", "No records yet");
    public static string Settings => Get("settings", "Settings");
    public static string SettingsScale => Get("settings_scale", "Scale");
    public static string SettingsOpacity => Get("settings_opacity", "Opacity");
    public static string SettingsMaxBars => Get("settings_max_bars", "Max Bars");
    public static string SettingsResetPos => Get("settings_reset_pos", "Reset Position");
    public static string ResetData => Get("reset_data", "Reset Data");
    public static string SettingsAutoReset => Get("settings_auto_reset", "Auto-reset on new run");
    public static string Dashboard => Get("dashboard", "Dashboard");

    public static void Initialize()
    {
        _loadedLanguage = null;
        EnsureLoaded();
    }

    public static string Get(string key, string fallback)
    {
        EnsureLoaded();
        return _translations.GetValueOrDefault(key) ?? fallback;
    }

    private static void EnsureLoaded()
    {
        var language = ResolveLanguage();
        if (string.Equals(language, _loadedLanguage, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _translations = LoadTranslations(language);
        _loadedLanguage = language;
    }

    private static Dictionary<string, string> LoadTranslations(string language)
    {
        foreach (var candidate in GetLanguageCandidates(language))
        {
            var loaded = TryLoadEmbedded(candidate);
            if (loaded is { Count: > 0 })
            {
                return loaded;
            }
        }

        return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    private static Dictionary<string, string>? TryLoadEmbedded(string language)
    {
        var resourceName = $"{ResourcePrefix}{language}.json";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(stream);
        }
        catch (JsonException ex)
        {
            LoggerAdapter.Error($"Failed to parse localization resource '{resourceName}'.", ex);
            return null;
        }
    }

    private static string ResolveLanguage()
    {
        string? language = null;
        try
        {
            language = LocManager.Instance?.Language;
        }
        catch
        {
        }

        if (string.IsNullOrWhiteSpace(language))
        {
            try
            {
                language = TranslationServer.GetLocale();
            }
            catch
            {
            }
        }

        return NormalizeLanguageCode(language);
    }

    private static string NormalizeLanguageCode(string? language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return DefaultLanguage;
        }

        return language
            .Trim()
            .Replace('-', '_')
            .ToLowerInvariant();
    }

    private static IEnumerable<string> GetLanguageCandidates(string language)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (seen.Add(language))
        {
            yield return language;
        }

        var separator = language.IndexOf('_');
        if (separator > 0)
        {
            var prefix = language[..separator];
            if (seen.Add(prefix))
            {
                yield return prefix;
            }
        }

        if (language.StartsWith("zh", StringComparison.OrdinalIgnoreCase) && seen.Add("zhs"))
        {
            yield return "zhs";
        }

        if (language.StartsWith("en", StringComparison.OrdinalIgnoreCase) && seen.Add("en"))
        {
            yield return "en";
        }

        if (seen.Add(DefaultLanguage))
        {
            yield return DefaultLanguage;
        }
    }
}
