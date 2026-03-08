using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;

namespace DamageMeterRebuilt.Infrastructure;

internal sealed class NameResolver
{
    public string ResolveEncounterKey(ModelId? id)
    {
        return id?.Entry ?? "unknown";
    }

    public string ResolveCreatureName(Creature? creature)
    {
        if (creature?.IsPlayer == true)
        {
            return ResolvePlayerDisplayName(creature);
        }

        return creature?.Name ?? "Unknown";
    }

    public string ResolvePlayerKey(Creature? creature)
    {
        if (creature is null)
        {
            return "unknown-player";
        }

        return creature.Player?.NetId.ToString() ?? creature.Name ?? "unknown-player";
    }

    public string ResolveCharacterId(Creature? creature)
    {
        return creature?.Player?.Character?.Id?.Entry ?? string.Empty;
    }

    public Color ResolveCharacterColor(Creature? creature)
    {
        return creature?.Player?.Character?.NameColor ?? new Color(0.95f, 0.55f, 0.15f, 1f);
    }

    public string ResolvePlayerDisplayName(Creature? creature)
    {
        if (creature is null)
        {
            return "Unknown";
        }

        try
        {
            var player = creature.Player;
            var runManager = RunManager.Instance;
            var platform = runManager?.NetService?.Platform;
            if (player is not null && platform is not null && runManager is not null)
            {
                var playerId = runManager.IsSinglePlayerOrFakeMultiplayer
                    ? PlatformUtil.GetLocalPlayerId(platform.Value)
                    : player.NetId;
                var playerName = PlatformUtil.GetPlayerName(platform.Value, playerId);
                if (!string.IsNullOrWhiteSpace(playerName))
                {
                    return playerName;
                }
            }

            return creature.Player?.Character?.Id?.Entry ?? creature.Name ?? "Unknown";
        }
        catch
        {
            return creature.Player?.Character?.Id?.Entry ?? creature.Name ?? "Unknown";
        }
    }

    public string ResolveCardKey(CardModel? card)
    {
        return card?.Id?.Entry ?? "Other";
    }

    public string ResolveCardPlayKey(CardPlay? cardPlay)
    {
        return ResolveCardKey(cardPlay?.Card);
    }

    public string ResolvePotionKey(PotionModel? potion)
    {
        return potion?.Id?.Entry ?? "Unknown";
    }

    public string ResolvePowerKey(PowerModel? power)
    {
        return power?.Id?.Entry ?? "Unknown";
    }

    public string ResolveMonsterKey(Creature? creature)
    {
        return creature?.Monster?.Id?.Entry ?? "Unknown";
    }

    public string ResolveDamageSourceKey(Creature? creature)
    {
        return creature?.IsMonster == true
            ? ResolveMonsterKey(creature)
            : "Unknown";
    }

    public string ResolveCardName(CardModel? card)
    {
        return card?.Title.ToString() ?? card?.Id?.Entry ?? "Unknown Card";
    }

    public string ResolveCardPlayName(CardPlay? cardPlay)
    {
        return ResolveCardName(cardPlay?.Card);
    }

    public string ResolveCardSourceName(CardModel? card)
    {
        return ResolveCardName(card);
    }

    public string ResolvePotionName(PotionModel? potion)
    {
        return potion?.Title.ToString() ?? potion?.Id?.Entry ?? "Unknown Potion";
    }

    public string ResolvePowerName(PowerModel? power)
    {
        return power?.Title.ToString() ?? power?.Id?.Entry ?? "Unknown Power";
    }

    public string ResolveCardDisplayName(string key)
    {
        if (key == "Other")
        {
            return key;
        }

        return ResolveLocalizedModelName("cards", key);
    }

    public string ResolvePotionDisplayName(string key)
    {
        return ResolveLocalizedModelName("potions", key);
    }

    public string ResolvePowerDisplayName(string key)
    {
        return ResolveLocalizedModelName("powers", key);
    }

    public string ResolveMonsterDisplayName(string key)
    {
        if (key == "Unknown")
        {
            return key;
        }

        return ResolveLocalizedModelName("monsters", key);
    }

    private static string ResolveLocalizedModelName(string table, string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return key;
        }

        try
        {
            var localized = new LocString(table, key + ".title");
            if (localized.Exists())
            {
                return localized.GetFormattedText();
            }
        }
        catch
        {
        }

        return key;
    }
}
