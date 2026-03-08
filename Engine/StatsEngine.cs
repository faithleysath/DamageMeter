using DamageMeterRebuilt.Domain;
using DamageMeterRebuilt.Infrastructure;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace DamageMeterRebuilt.Engine;

internal sealed class StatsEngine
{
    private readonly SessionStore _sessions;
    private readonly NameResolver _names;

    public StatsEngine(SessionStore sessions, NameResolver names)
    {
        _sessions = sessions;
        _names = names;
    }

    public void Apply(CombatHistoryEntry entry, EncounterSession session)
    {
        switch (entry)
        {
            case DamageReceivedEntry damage:
                ApplyDamageReceived(damage, session);
                break;
            case BlockGainedEntry block:
                ApplyBlockGained(block, session);
                break;
            case CardPlayFinishedEntry cardPlay:
                ApplyCardPlayFinished(cardPlay, session);
                break;
            case CardDrawnEntry cardDrawn:
                ApplyCardDrawn(cardDrawn, session);
                break;
            case CardDiscardedEntry discarded:
                ApplyCardDiscarded(discarded, session);
                break;
            case CardExhaustedEntry exhausted:
                ApplyCardExhausted(exhausted, session);
                break;
            case PotionUsedEntry potionUsed:
                ApplyPotionUsed(potionUsed, session);
                break;
            case PowerReceivedEntry powerReceived:
                ApplyPowerReceived(powerReceived, session);
                break;
            case MonsterPerformedMoveEntry move:
                ApplyMonsterMove(move, session);
                break;
            case OrbChanneledEntry orb:
                ApplyOrbChanneled(orb, session);
                break;
            case CardGeneratedEntry generated:
                ApplyCardGenerated(generated, session);
                break;
            case CardAfflictedEntry afflicted:
                ApplyCardAfflicted(afflicted, session);
                break;
            case EnergySpentEntry energy:
                ApplyEnergySpent(energy, session);
                break;
            case SummonedEntry summoned:
                ApplySummoned(summoned, session);
                break;
            case StarsModifiedEntry stars:
                ApplyStarsModified(stars, session);
                break;
        }
    }

    public void ApplyTurnStarted(CombatState state)
    {
        if (_sessions.CurrentEncounter is null)
        {
            return;
        }

        _sessions.CurrentEncounter.CurrentTurn = state.RoundNumber;
        _sessions.MarkDirty();
    }

    public void ApplyTurnEnded(CombatState state)
    {
        if (_sessions.CurrentEncounter is null)
        {
            return;
        }

        foreach (var player in state.Players)
        {
            var stats = GetOrCreatePlayer(player.Creature, out _);
            var wasted = Math.Max(player.PlayerCombatState?.Energy ?? 0, 0);
            stats.TotalEnergyWasted += wasted;
            stats.EnergyWastedPerTurn[state.RoundNumber] =
                stats.EnergyWastedPerTurn.GetValueOrDefault(state.RoundNumber) + wasted;
        }

        _sessions.MarkDirty();
    }

    private void ApplyDamageReceived(DamageReceivedEntry entry, EncounterSession session)
    {
        var result = entry.Result;
        var receiver = entry.Receiver;
        var dealer = entry.Dealer;
        var cardSource = entry.CardSource;

        if (dealer is not null && dealer.IsPlayer && receiver?.IsEnemy == true)
        {
            var dealerStats = GetOrCreatePlayer(dealer, out var dealerKey);

            dealerStats.DamageDealt += result.UnblockedDamage;
            dealerStats.OverkillDealt += result.OverkillDamage;
            dealerStats.BlockedByTarget += result.BlockedDamage;
            dealerStats.HitCount += 1;
            dealerStats.DamagePerTurn[session.CurrentTurn] =
                dealerStats.DamagePerTurn.GetValueOrDefault(session.CurrentTurn) + result.UnblockedDamage;

            var sourceKey = _names.ResolveCardKey(cardSource);
            dealerStats.DamageByCard[sourceKey] =
                dealerStats.DamageByCard.GetValueOrDefault(sourceKey) + result.UnblockedDamage;

            if (result.UnblockedDamage > dealerStats.MaxSingleHit)
            {
                dealerStats.MaxSingleHit = result.UnblockedDamage;
                dealerStats.MaxSingleHitCard = sourceKey;
            }

            if (result.UnblockedDamage > 0)
            {
                session.Timeline.Add(new TimelineEvent(
                    TimelineEventType.DamageDealt,
                    session.CurrentTurn,
                    dealerKey,
                    $"{_names.ResolveCardDisplayName(sourceKey)} -> {result.UnblockedDamage}",
                    result.UnblockedDamage));
            }
        }

        if (receiver is not null && receiver.IsPlayer)
        {
            var receiverStats = GetOrCreatePlayer(receiver, out var receiverKey);
            receiverStats.DamageTaken += result.UnblockedDamage;

            var sourceKey = _names.ResolveDamageSourceKey(dealer);
            receiverStats.DamageBySource[sourceKey] =
                receiverStats.DamageBySource.GetValueOrDefault(sourceKey) + result.UnblockedDamage;

            if (result.UnblockedDamage > 0)
            {
                session.Timeline.Add(new TimelineEvent(
                    TimelineEventType.DamageTaken,
                    session.CurrentTurn,
                    receiverKey,
                    $"{_names.ResolveMonsterDisplayName(sourceKey)} -> {result.UnblockedDamage}",
                    result.UnblockedDamage));
            }

            if (result.WasTargetKilled)
            {
                _sessions.CaptureDeathSnapshot(receiverKey, maxEvents: 8);
            }
        }
    }

    private void ApplyBlockGained(BlockGainedEntry entry, EncounterSession session)
    {
        var receiver = entry.Receiver;
        if (receiver is null || !receiver.IsPlayer)
        {
            return;
        }

        var stats = GetOrCreatePlayer(receiver, out var key);
        stats.TotalBlockGained += entry.Amount;

        var cardKey = _names.ResolveCardPlayKey(entry.CardPlay);
        stats.BlockByCard[cardKey] = stats.BlockByCard.GetValueOrDefault(cardKey) + entry.Amount;

        if (entry.Amount > 0)
        {
            session.Timeline.Add(new TimelineEvent(
                TimelineEventType.BlockGained,
                session.CurrentTurn,
                key,
                $"+{entry.Amount} ({_names.ResolveCardDisplayName(cardKey)})",
                entry.Amount));
        }
    }

    private void ApplyCardPlayFinished(CardPlayFinishedEntry entry, EncounterSession session)
    {
        var card = entry.CardPlay?.Card;
        var ownerCreature = card?.Owner?.Creature;
        if (ownerCreature is null || !ownerCreature.IsPlayer)
        {
            return;
        }

        var stats = GetOrCreatePlayer(ownerCreature, out var key);
        var cardKey = _names.ResolveCardKey(card);
        var cardName = _names.ResolveCardName(card);

        stats.CardsPlayed += 1;
        stats.CardPlayCount[cardKey] = stats.CardPlayCount.GetValueOrDefault(cardKey) + 1;
        stats.CardTypeCount[card!.Type] = stats.CardTypeCount.GetValueOrDefault(card.Type) + 1;

        var energySpent = entry.CardPlay is null ? 0 : entry.CardPlay.Resources.EnergySpent;
        stats.TotalEnergySpent += energySpent;
        if (energySpent > 0)
        {
            stats.EnergySpentByCard[cardKey] = stats.EnergySpentByCard.GetValueOrDefault(cardKey) + energySpent;
        }

        session.Timeline.Add(new TimelineEvent(
            TimelineEventType.CardPlayed,
            session.CurrentTurn,
            key,
            cardName,
            energySpent));
    }

    private void ApplyCardDrawn(CardDrawnEntry entry, EncounterSession session)
    {
        var ownerCreature = entry.Card?.Owner?.Creature;
        if (ownerCreature is null || !ownerCreature.IsPlayer)
        {
            return;
        }

        var stats = GetOrCreatePlayer(ownerCreature, out var key);
        var cardKey = _names.ResolveCardKey(entry.Card);
        var cardName = _names.ResolveCardName(entry.Card);

        stats.CardsDrawn += 1;
        stats.DrawCount[cardKey] = stats.DrawCount.GetValueOrDefault(cardKey) + 1;

        session.Timeline.Add(new TimelineEvent(
            TimelineEventType.CardDrawn,
            session.CurrentTurn,
            key,
            cardName,
            1));
    }

    private void ApplyCardDiscarded(CardDiscardedEntry entry, EncounterSession session)
    {
        var ownerCreature = entry.Card?.Owner?.Creature;
        if (ownerCreature is null || !ownerCreature.IsPlayer)
        {
            return;
        }

        var stats = GetOrCreatePlayer(ownerCreature, out var key);
        var cardKey = _names.ResolveCardKey(entry.Card);
        var cardName = _names.ResolveCardName(entry.Card);

        stats.CardsDiscarded += 1;
        stats.DiscardCount[cardKey] = stats.DiscardCount.GetValueOrDefault(cardKey) + 1;

        session.Timeline.Add(new TimelineEvent(
            TimelineEventType.CardDiscarded,
            session.CurrentTurn,
            key,
            cardName,
            1));
    }

    private void ApplyCardExhausted(CardExhaustedEntry entry, EncounterSession session)
    {
        var ownerCreature = entry.Card?.Owner?.Creature;
        if (ownerCreature is null || !ownerCreature.IsPlayer)
        {
            return;
        }

        var stats = GetOrCreatePlayer(ownerCreature, out var key);
        var cardKey = _names.ResolveCardKey(entry.Card);
        var cardName = _names.ResolveCardName(entry.Card);

        stats.CardsExhausted += 1;
        stats.ExhaustCount[cardKey] = stats.ExhaustCount.GetValueOrDefault(cardKey) + 1;

        session.Timeline.Add(new TimelineEvent(
            TimelineEventType.CardExhausted,
            session.CurrentTurn,
            key,
            cardName,
            1));
    }

    private void ApplyPotionUsed(PotionUsedEntry entry, EncounterSession session)
    {
        var ownerCreature = entry.Potion?.Owner?.Creature;
        if (ownerCreature is null || !ownerCreature.IsPlayer)
        {
            return;
        }

        var stats = GetOrCreatePlayer(ownerCreature, out var key);
        var potionKey = _names.ResolvePotionKey(entry.Potion);
        var potionName = _names.ResolvePotionName(entry.Potion);

        stats.PotionsUsed += 1;
        stats.PotionUseCount[potionKey] = stats.PotionUseCount.GetValueOrDefault(potionKey) + 1;

        session.Timeline.Add(new TimelineEvent(
            TimelineEventType.PotionUsed,
            session.CurrentTurn,
            key,
            potionName,
            1));
    }

    private void ApplyPowerReceived(PowerReceivedEntry entry, EncounterSession session)
    {
        if (entry.Power?.Type != PowerType.Debuff || entry.Power.Owner?.IsEnemy != true)
        {
            return;
        }

        var applierCreature = entry.Applier;
        if (applierCreature is null || !applierCreature.IsPlayer)
        {
            return;
        }

        var stats = GetOrCreatePlayer(applierCreature, out var key);
        var powerKey = _names.ResolvePowerKey(entry.Power);
        var powerName = _names.ResolvePowerName(entry.Power);
        var amount = Convert.ToInt32(decimal.Max(entry.Amount, 1m));

        stats.DebuffsApplied += amount;
        stats.PowerDebuffCount[powerKey] = stats.PowerDebuffCount.GetValueOrDefault(powerKey) + amount;

        session.Timeline.Add(new TimelineEvent(
            TimelineEventType.DebuffApplied,
            session.CurrentTurn,
            key,
            $"{powerName} x{amount}",
            amount));
    }

    private void ApplyMonsterMove(MonsterPerformedMoveEntry entry, EncounterSession session)
    {
        var monsterLabel = entry.Monster?.Title.ToString() ?? entry.Monster?.Id?.Entry ?? "Unknown Monster";
        var moveLabel = entry.Move.Id ?? "unknown-move";
        var threatLabel = $"{monsterLabel}: {moveLabel}";

        if (entry.Targets is not null)
        {
            foreach (var target in entry.Targets)
            {
                if (target is null || !target.IsPlayer)
                {
                    continue;
                }

                var stats = GetOrCreatePlayer(target, out _);
                stats.MonsterMoveTargets[threatLabel] =
                    stats.MonsterMoveTargets.GetValueOrDefault(threatLabel) + 1;
            }
        }

        session.Timeline.Add(new TimelineEvent(
            TimelineEventType.MonsterMove,
            session.CurrentTurn,
            monsterLabel,
            moveLabel,
            1));
    }

    private void ApplyOrbChanneled(OrbChanneledEntry entry, EncounterSession session)
    {
        var ownerCreature = entry.Orb?.Owner?.Creature;
        if (ownerCreature is null || !ownerCreature.IsPlayer)
        {
            return;
        }

        var stats = GetOrCreatePlayer(ownerCreature, out var key);
        var orbName = entry.Orb?.Title.ToString() ?? "Unknown Orb";

        stats.OrbsChanneled += 1;
        stats.OrbCount[orbName] = stats.OrbCount.GetValueOrDefault(orbName) + 1;

        session.Timeline.Add(new TimelineEvent(
            TimelineEventType.OrbChanneled,
            session.CurrentTurn,
            key,
            orbName,
            1));
    }

    private void ApplyCardGenerated(CardGeneratedEntry entry, EncounterSession session)
    {
        var ownerCreature = entry.Card?.Owner?.Creature;
        if (ownerCreature is null || !ownerCreature.IsPlayer)
        {
            return;
        }

        var stats = GetOrCreatePlayer(ownerCreature, out var key);
        var cardName = _names.ResolveCardName(entry.Card);

        stats.CardsGenerated += 1;
        stats.GeneratedCardCount[cardName] = stats.GeneratedCardCount.GetValueOrDefault(cardName) + 1;

        session.Timeline.Add(new TimelineEvent(
            TimelineEventType.CardGenerated,
            session.CurrentTurn,
            key,
            cardName,
            1));
    }

    private void ApplyCardAfflicted(CardAfflictedEntry entry, EncounterSession session)
    {
        var ownerCreature = entry.Card?.Owner?.Creature;
        if (ownerCreature is null || !ownerCreature.IsPlayer)
        {
            return;
        }

        var stats = GetOrCreatePlayer(ownerCreature, out var key);
        var afflictionName = entry.Affliction?.Title.ToString() ?? "Unknown Affliction";

        stats.AfflictionsApplied += 1;
        stats.AfflictionCount[afflictionName] = stats.AfflictionCount.GetValueOrDefault(afflictionName) + 1;

        session.Timeline.Add(new TimelineEvent(
            TimelineEventType.AfflictionApplied,
            session.CurrentTurn,
            key,
            afflictionName,
            1));
    }

    private void ApplyEnergySpent(EnergySpentEntry entry, EncounterSession session)
    {
        var creature = entry.Actor;
        if (creature is null || !creature.IsPlayer)
        {
            return;
        }

        var key = _names.ResolvePlayerKey(creature);
        var label = "Raw Energy Spend";
        session.Timeline.Add(new TimelineEvent(
            TimelineEventType.EnergySpent,
            session.CurrentTurn,
            key,
            label,
            Convert.ToInt32(entry.Amount)));
    }

    private void ApplySummoned(SummonedEntry entry, EncounterSession session)
    {
        var creature = entry.Actor;
        if (creature is null || !creature.IsPlayer)
        {
            return;
        }

        var stats = GetOrCreatePlayer(creature, out var key);
        var amount = Convert.ToInt32(entry.Amount);
        stats.SummonsCreated += amount;

        session.Timeline.Add(new TimelineEvent(
            TimelineEventType.Summoned,
            session.CurrentTurn,
            key,
            "Summoned",
            amount));
    }

    private void ApplyStarsModified(StarsModifiedEntry entry, EncounterSession session)
    {
        var creature = entry.Actor;
        if (creature is null || !creature.IsPlayer)
        {
            return;
        }

        var stats = GetOrCreatePlayer(creature, out var key);
        var amount = Convert.ToInt32(entry.Amount);
        stats.StarsModified += amount;

        session.Timeline.Add(new TimelineEvent(
            TimelineEventType.StarsModified,
            session.CurrentTurn,
            key,
            "Stars Modified",
            amount));
    }

    private PlayerStats GetOrCreatePlayer(Creature creature, out string key)
    {
        key = _names.ResolvePlayerKey(creature);
        var stats = _sessions.GetOrCreatePlayer(key, _names.ResolveCreatureName(creature));
        stats.CharacterId = _names.ResolveCharacterId(creature);
        stats.CharacterColor = _names.ResolveCharacterColor(creature);
        return stats;
    }
}
