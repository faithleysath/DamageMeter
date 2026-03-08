# API Reference

## Preferred Public Entry Points

### Run lifecycle

Use:

- `RunManager.Instance`
- `RunManager.RunStarted`

Current assumption:

- new run reset belongs here

### Combat lifecycle

Use:

- `CombatManager.Instance`
- `CombatManager.CombatSetUp`
- `CombatManager.CombatEnded`
- `CombatManager.TurnStarted`
- `CombatManager.TurnEnded`
- `CombatManager.History`
- `CombatManager.StateTracker`

Confirmed delegate shapes used by the current rebuild:

- `CombatManager.CombatSetUp += Action<CombatState>`
- `CombatManager.CombatEnded += Action<CombatRoom>`
- `CombatManager.TurnStarted += Action<CombatState>`
- `CombatManager.TurnEnded += Action<CombatState>`
- `RunManager.RunStarted += Action<...>` and the rebuild only depends on the notification itself

## `CombatHistory`

### Events

- `Changed`

### Collection

- `Entries`

This is currently exposed as `IEnumerable<CombatHistoryEntry>`, so the rebuild materializes it with `ToList()` before indexed incremental draining.

### Methods

- `Clear()`
- `CardPlayStarted(...)`
- `CardPlayFinished(...)`
- `CardAfflicted(...)`
- `CardDiscarded(...)`
- `CardDrawn(...)`
- `CardExhausted(...)`
- `CardGenerated(...)`
- `CreatureAttacked(...)`
- `DamageReceived(...)`
- `BlockGained(...)`
- `EnergySpent(...)`
- `MonsterPerformedMove(...)`
- `OrbChanneled(...)`
- `PotionUsed(...)`
- `PowerReceived(...)`
- `StarsModified(...)`
- `Summoned(...)`

## Important Entry Shapes

### `DamageReceivedEntry`

Getters confirmed:

- `Result`
- `Dealer`
- `CardSource`
- `Receiver`

Usage:

- damage
- overkill
- blocked damage
- source attribution
- death snapshots

### `BlockGainedEntry`

Getters confirmed:

- `Amount`
- `Receiver`
- `Props`
- `CardPlay`

Usage:

- block totals
- block by card

### `CardPlayFinishedEntry`

Getters confirmed:

- `CardPlay`
- `WasEthereal`

Usage:

- cards played
- card type counts
- energy spent by card

### `CardDrawnEntry`

Getters confirmed:

- `Card`
- `FromHandDraw`

### `PotionUsedEntry`

Getters confirmed:

- `Potion`
- `Target`

### `PowerReceivedEntry`

Getters confirmed:

- `Power`
- `Amount`
- `Applier`

Current implementation assumption:

- `Applier` is a `Creature`

### `MonsterPerformedMoveEntry`

Getters confirmed:

- `Monster`
- `Move`
- `Targets`

Current implementation notes:

- `Monster` is a `MonsterModel`
- `Move.Id` is currently treated as a string identifier
- no confirmed public `Move.Title`, so UI should not assume it exists

### `OrbChanneledEntry`

Getters confirmed:

- `Orb`

Current implementation note:

- `Orb.Title` is a `LocString`

### `EnergySpentEntry`

Getters confirmed:

- `Amount`

Current implementation assumption:

- `Actor` inherited from `CombatHistoryEntry` is a `Creature`

### `SummonedEntry`

Getters confirmed:

- `Amount`

Current implementation assumption:

- `Actor` inherited from `CombatHistoryEntry` is a `Creature`

### `StarsModifiedEntry`

Getters confirmed:

- `Amount`

Current implementation assumption:

- `Actor` inherited from `CombatHistoryEntry` is a `Creature`

## Game Type Notes

### `Creature`

Confirmed getters:

- `IsPlayer`
- `IsEnemy`
- `IsMonster`
- `Player`
- `Monster`
- `Name`

### `DamageResult`

Confirmed getters:

- `UnblockedDamage`
- `BlockedDamage`
- `OverkillDamage`
- `WasTargetKilled`

### `CardPlay`

Confirmed getters:

- `Card`
- `Target`
- `ResultPile`
- `Resources`
- `IsAutoPlay`
- `PlayIndex`
- `PlayCount`
- `IsFirstInSeries`
- `IsLastInSeries`

### `ResourceInfo`

Confirmed getters:

- `EnergySpent`
- `EnergyValue`
- `StarsSpent`
- `StarValue`

### `CardModel`

Confirmed getters:

- `Id`
- `Title`
- `Type`
- `Owner`

### `PotionModel`

Confirmed getters:

- `Id`
- `Title`
- `Owner`

### `PowerModel`

Confirmed getters:

- `Id`
- `Title`
- `Type`
- `Owner`

### `PowerType`

Confirmed enum values:

- `None`
- `Buff`
- `Debuff`

### `CardType`

Confirmed enum values:

- `None`
- `Attack`
- `Skill`
- `Power`
- `Status`
- `Curse`
- `Quest`

## Display Conversion Notes

Several game-facing titles are not plain strings.

Use:

- `LocString.ToString()` for user-facing labels
- `ModelId.Entry` as fallback
- raw move ids only when no title-like string exists

## Rebuild Consumption Rules

The current rebuild uses this split:

- `CombatHistory.Changed` for accounting
- `CombatManager.TurnStarted` and `CombatManager.TurnEnded` for per-turn state
- `CombatManager.CombatSetUp` and `CombatManager.CombatEnded` for encounter lifecycle
- `RunManager.RunStarted` for full-run reset
- `CombatStateTracker.CombatStateChanged` is reserved for future UI refresh only

- `None`
- `Buff`
- `Debuff`

### `CardType`

Confirmed enum values:

- `None`
- `Attack`
- `Skill`
- `Power`
- `Status`
- `Curse`
- `Quest`

### `ModelId`

Confirmed getters:

- `Category`
- `Entry`

### `CombatState`

Confirmed getters:

- `Creatures`
- `Players`
- `RoundNumber`
- `CurrentSide`
- `Encounter`

## Implementation Notes

### `LocString`

Several display fields are `LocString`, not plain `string`.

Current code converts them with `.ToString()`.

### `History.Entries`

Do not assume random access.

Materialize the enumerable before index-based draining.

### `CombatEnded`

Current public event signature matches `Action<CombatRoom>`, not `Action<CombatState>`.

### `StateTracker`

Use `CombatStateTracker.CombatStateChanged` only as an optional UI refresh signal.

Do not use it as the accounting source.
