# Analysis Notes

## Scope

This file preserves the reverse-engineering findings that led to the rebuilt mod architecture.

It exists so the project can continue without relying on chat context.

## Original Mod Failure on Apple Silicon

### Root cause

The original `DamageMeter.dll` is not x86-only.

Observed CLR flags:

- `ILONLY = true`
- `32BITREQUIRED = false`
- `32BITPREFERRED = false`

So the failure is not a PE architecture target problem.

The failure happens when the mod initializes Harmony patches:

- `MainFile.Initialize()`
  - load settings
  - load i18n
  - initialize UI
  - call `new Harmony(...).PatchAll()`

On native `arm64`, patch initialization failed with:

- `HarmonyLib.HarmonyException`
- inner `System.NotImplementedException`

The failure path reached Harmony/MonoMod detour creation. The same mod worked when the game was forced through Rosetta/x86_64.

### Practical conclusion

The original mod's business logic is fine.

The broken part is:

- Harmony patching on the game's `arm64` runtime path

The rebuilt mod should avoid Harmony entirely unless absolutely necessary.

## Original Mod Architecture

### Patch targets

The old mod patched these methods:

- `CombatHistory.BlockGained`
- `CombatHistory.CardDrawn`
- `CombatHistory.CardDiscarded`
- `CombatHistory.CardExhausted`
- `CombatHistory.CardPlayFinished`
- `CombatHistory.DamageReceived`
- `CombatHistory.PotionUsed`
- `CombatHistory.PowerReceived`
- `CombatManager.SetUpCombat`
- `CombatManager.Reset`
- `RunManager.SetUpNewSinglePlayer`
- `RunManager.SetUpNewMultiPlayer`

### Collector methods used by the original mod

The old `CombatDataCollector` used these methods:

- `ArchiveAndStartNew`
- `StopTracking`
- `ResetAll`
- `OnTurnStarted`
- `OnTurnEnded`
- `RecordDamage`
- `RecordCardPlay`
- `RecordBlockGained`
- `RecordPotionUsed`
- `RecordPowerReceived`
- `RecordCardDrawn`
- `RecordCardDiscarded`
- `RecordCardExhausted`

### Important implication

Most of the old mod was already conceptually built on top of `CombatHistory`.

Harmony was only used to intercept history-producing methods, not to patch arbitrary deep combat code.

That makes a non-Harmony rebuild realistic.

## Official Runtime APIs Confirmed in `sts2.dll`

### Public managers

Confirmed public singletons:

- `CombatManager.Instance`
- `RunManager.Instance`

Confirmed public manager getters:

- `CombatManager.History`
- `CombatManager.StateTracker`

### Public combat events

Confirmed public event add/remove methods on `CombatManager`:

- `CombatSetUp`
- `CombatEnded`
- `CombatWon`
- `TurnStarted`
- `TurnEnded`
- `PlayerEndedTurn`
- `AboutToSwitchToEnemyTurn`

### Public run events

Confirmed public event add/remove methods on `RunManager`:

- `RunStarted`
- `RoomEntered`
- `RoomExited`
- `ActEntered`

These are preferred over Harmony patches.

## `CombatHistory` Findings

### Public API

Confirmed public members:

- `add_Changed`
- `remove_Changed`
- `get_Entries`
- `get_CardPlaysStarted`
- `get_CardPlaysFinished`
- `Clear`
- `Add`

The `Changed` event is enough for incremental consumption.

### Entry types present

Confirmed entry classes:

- `BlockGainedEntry`
- `CardAfflictedEntry`
- `CardDiscardedEntry`
- `CardDrawnEntry`
- `CardExhaustedEntry`
- `CardGeneratedEntry`
- `CardPlayFinishedEntry`
- `CardPlayStartedEntry`
- `CreatureAttackedEntry`
- `DamageReceivedEntry`
- `EnergySpentEntry`
- `MonsterPerformedMoveEntry`
- `OrbChanneledEntry`
- `PotionUsedEntry`
- `PowerReceivedEntry`
- `StarsModifiedEntry`
- `SummonedEntry`

### Entry base class

`CombatHistoryEntry` exposes:

- `Actor`
- `RoundNumber`
- `CurrentSide`
- `History`
- `HappenedThisTurn(state)`

`HappenedThisTurn` compares:

- entry round number
- entry side

against the current combat state.

This is enough for per-turn grouping.

### Clear behavior

`CombatHistory.Clear()` clears `_entries` and immediately invokes `Changed`.

`CombatManager.Reset()` calls `CombatManager.History.Clear()`.

The rebuilt mod must treat a shorter entry list as a reset signal.

### Confirmed call sites

These history methods were confirmed to be part of real combat flow:

- `DamageReceived` from a damage state machine
- `BlockGained` from gain-block flow
- `CardDrawn` from draw flow
- `CardPlayStarted` and `CardPlayFinished` from card play wrapper flow
- `PotionUsed` from potion wrapper flow
- `PowerReceived` from power apply/modify flow
- `MonsterPerformedMove` from monster move execution
- `EnergySpent` from spend-energy flow
- `OrbChanneled` from orb channel flow

This is not dead code. It is the official runtime history stream.

## `CombatStateTracker` Findings

### What it is

`CombatStateTracker` is a higher-level aggregation layer over combat state changes.

It subscribes to:

- `CombatHistory.Changed`
- combat creature/pile/power/turn events

and exposes:

- `CombatStateChanged`

### Event semantics

`CombatStateChanged` is a standard .NET event.

It defers callback dispatch to the next frame and then invokes subscribers with the current combat state.

This makes it suitable for:

- UI refresh
- coarse synchronization

It is not the preferred accounting source because it coalesces changes.

### Design rule

Use:

- `CombatHistory.Changed` for accounting
- `CombatStateTracker.CombatStateChanged` only for optional UI refresh

## Official Hook System Findings

The game contains a broad official hook framework:

- `MegaCrit.Sts2.Core.Hooks.Hook`
- many `After*`, `Before*`, `Modify*`, `Should*` methods

But the hook listener model is not a simple global subscribe API.

The listener surface is mainly:

- `AbstractModel` virtual methods
- combat/run state iterating over models already present in state

That is useful for future deep integrations, but it is more complex than the official manager/history event route.

## Old Patch to Rebuild Mapping

The original mod patched `CombatHistory.*` producers. The rebuild should consume emitted history entries directly.

Mapping:

- `DamageReceivedPatch` -> `DamageReceivedEntry`
- `BlockGainedPatch` -> `BlockGainedEntry`
- `CardDrawnPatch` -> `CardDrawnEntry`
- `CardDiscardedPatch` -> `CardDiscardedEntry`
- `CardExhaustedPatch` -> `CardExhaustedEntry`
- `CardPlayFinishedPatch` -> `CardPlayFinishedEntry`
- `PotionUsedPatch` -> `PotionUsedEntry`
- `PowerReceivedPatch` -> `PowerReceivedEntry`
- `CombatSetUpPatch` -> `CombatManager.CombatSetUp`
- `CombatResetPatch` -> `CombatManager.CombatEnded` plus `CombatHistory.Clear()` semantics
- `RunStartSinglePlayerPatch` -> `RunManager.RunStarted`
- `RunStartMultiPlayerPatch` -> `RunManager.RunStarted`

This means the core old features can be rebuilt without Harmony.

## Type Notes That Matter in Code

The following runtime details were confirmed and should be treated as implementation constraints:

- `CombatHistory.Entries` is exposed as `IEnumerable<CombatHistoryEntry>`, so indexed draining currently materializes with `ToList()`
- `CombatEnded` uses `Action<CombatRoom>`
- `TurnStarted` and `TurnEnded` use `Action<CombatState>`
- `CombatHistoryEntry.Actor` is usable as a `Creature` for at least `EnergySpentEntry`, `SummonedEntry`, and `StarsModifiedEntry`
- `PowerReceivedEntry.Applier` is a `Creature`
- `MonsterPerformedMoveEntry.Monster` is a `MonsterModel`
- `MoveState` exposes `Id`, but no confirmed public `Title`
- many titles are `LocString`, so display should use `.ToString()`

## Rebuild Status

Current implementation lives under `DamageMeterRebuilt/` and already follows the non-Harmony architecture:

- `ModEntry` wires lifecycle, history tailing, stats engine, and dashboard
- `HistoryTailer` consumes `CombatHistory.Changed`
- `TurnTracker` consumes `TurnStarted` and `TurnEnded`
- `CombatLifecycleBridge` consumes `CombatSetUp` and `CombatEnded`
- `RunLifecycleBridge` consumes `RunStarted`
- `StatsEngine` currently handles:
  - `DamageReceivedEntry`
  - `BlockGainedEntry`
  - `CardPlayFinishedEntry`
  - `CardDrawnEntry`
  - `CardDiscardedEntry`
  - `CardExhaustedEntry`
  - `PotionUsedEntry`
  - `PowerReceivedEntry`
  - `MonsterPerformedMoveEntry`
  - `OrbChanneledEntry`
  - `CardGeneratedEntry`
  - `CardAfflictedEntry`
  - `EnergySpentEntry`
  - `SummonedEntry`
  - `StarsModifiedEntry`

The current UI is a code-only Godot overlay. No `.pck` resources are required yet because the panel is built entirely from C# nodes.

## Known Gaps

The rebuild is not feature-complete yet. Missing or unfinished areas:

- persistent settings
- category-based multi-panel UI
- richer overall/records views
- install/test loop inside the actual game
- validation of whether a `.pck` is optional for this pure-code mod layout

For this project, manager/history events are the simplest and safest architecture.

## Mapping from Old Metrics to New Official Sources

### Can be rebuilt directly from history

- damage dealt / taken
- blocked by target
- overkill
- highest hit
- damage by card
- damage by source
- total block gained
- block by card
- cards played
- card type counts
- card draw / discard / exhaust
- potions used
- debuffs applied
- generated cards
- afflictions
- monster moves
- orb channeling
- raw energy spend
- summons
- stars modified

### Still needs combat manager events

- wasted energy at turn end
- encounter start/stop
- run reset

## Current Rebuild Direction

### Hard rule

Do not use Harmony unless there is no public official route.

### Current architecture

- `RunManager.RunStarted`
- `CombatManager.CombatSetUp`
- `CombatManager.CombatEnded`
- `CombatManager.TurnStarted`
- `CombatManager.TurnEnded`
- `CombatManager.History.Changed`

### Current code status

Implemented and compiling:

- data model skeleton
- session store
- history tailer
- combat lifecycle bridge
- turn tracker
- stats engine for the main entry types
- dashboard controller placeholder

### Not done yet

- real on-screen UI
- mod packaging manifest and install layout
- settings persistence
- export/import
- polish around multiplayer attribution and display formatting
