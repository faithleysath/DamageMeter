# Maintainer Handoff

This document is the shortest path for a new maintainer to take over the rebuild.

## Goal

Keep the rebuilt mod functionally aligned with the original `DamageMeter` while preserving native `arm64` compatibility on macOS.

The rebuild is not a Harmony port. It is a no-Harmony implementation built on public game APIs.

Repository identity, source identity, and runtime identity are intentionally different:

- repository root: this repo
- source namespace/project identity: `DamageMeterRebuilt`
- packaged runtime identity: `DamageMeter`
- deployed runtime files: `mods/DamageMeter/DamageMeter.dll`, `DamageMeter.pck`, `mod_manifest.json`

This runtime alias exists to maximize compatibility with the original mod's multiplayer/mod-matching behavior.

## Environment Bring-Up

For a fresh machine or no-context AI handoff:

1. Install a `.NET 9` SDK compatible with `global.json`.
2. Install `Godot 4.5.1`.
3. Find the local Slay the Spire 2 install.
4. Build with explicit MSBuild properties unless you are on the original maintainer machine.

Important:

- the checked-in `GameAppDir` default in `DamageMeterRebuilt.csproj` is a local convenience path, not a portable default
- the project currently assumes the native macOS `arm64` data directory unless `GameDataDir` is overridden
- do not use `Godot 4.6+` for packing

## Core Technical Decision

The original mod patches `CombatHistory.*` methods with Harmony and collects data inside patch callbacks.

The rebuild does not patch anything. It subscribes to official runtime signals and consumes `CombatHistory.Entries` directly.

Primary sources:

- `CombatManager.CombatSetUp`
- `CombatManager.CombatEnded`
- `CombatManager.TurnStarted`
- `CombatManager.TurnEnded`
- `CombatManager.History.Changed`

## Architecture

### Data flow

1. `RunLifecycleBridge` resets run state on `RunStarted`
2. `CombatLifecycleBridge` starts/stops encounter tracking
3. `HistoryTailer` drains new `CombatHistory.Entries`
4. `StatsEngine` maps entries to `PlayerStats`
5. `SessionStore` maintains current encounter, archived fights, and persistent records
6. `DashboardController` projects that data into the main panel and dashboard UI

### Main files

- `ModEntry.cs`
  root wiring
- `Lifecycle/HistoryTailer.cs`
  incremental `CombatHistory` reader
- `Engine/StatsEngine.cs`
  stat collection logic
- `Domain/SessionStore.cs`
  encounter/run aggregation and records
- `Views/DashboardController.cs`
  UI projection logic
- `Views/DashboardOverlay.cs`
  main panel
- `Views/DashboardModalOverlay.cs`
  full-screen dashboard
- `Views/DashboardInputHandler.cs`
  root-level hotkey handling

## Parity Status

### Matches original closely

- main stat categories
- `Current` / `Overall` / archived `Fight N`
- records
- original-style panel/dashboard shell
- original-style category menu/settings menu/reset flow
- original-like player icon/color/localization treatment
- native `arm64` compatibility

### Intentional additions

- `Orbs`
- `Threat`
- `Advanced`

These categories use public `CombatHistory` data the original mod never surfaced.

## Current Input Handling

The rebuild uses `Views/DashboardInputHandler.cs`, attached to `SceneTree.Root`.

Hotkeys are polled from the root-level node so the panel itself does not need focus.

If input regresses again, start investigation there first.

## Runtime Packaging Notes

The packaged mod intentionally mimics the original mod metadata:

- `pck_name = DamageMeter`
- `name = Skada: Spire Edition`
- `author = 皮一下就很凡`
- `version = 1.1.0`

The mod-list artwork also requires Godot import metadata inside the `.pck`, not just a raw `png`.

Current packaged resources include:

- `res://DamageMeter/mod_image.png`
- `res://DamageMeter/mod_image.png.import`
- `res://.godot/imported/mod_image.png-43b80ce458f4c387952d9e3d7794760b.ctex`
- `res://DamageMeter/localization/en.json`
- `res://DamageMeter/localization/zhs.json`

## Data Sources Already Mapped

Implemented `CombatHistoryEntry` handlers:

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

## Testing Checklist

### Core smoke test

1. Build with `dotnet build`
2. Deploy with `dotnet build -t:DeployModToGame`
3. Make sure only one runtime `DamageMeter` folder exists under `mods/`
4. Enable `Skada: Spire Edition`
5. Verify:
   - mod loads
   - panel appears
   - `F7`, `F8`, `Shift+F8`, `F9`, `Escape` work
   - stats update during combat
   - mod list artwork renders correctly

### Parity regression test

- `Damage Dealt`
- `Damage Taken`
- `DPT`
- `Cards Played`
- `Block`
- `Energy`
- `DMG/Energy`
- `Overkill`
- `Potions`
- `Debuffs Applied`
- `Death Log`
- `Card Flow`
- `Combat Log`
- `Records`

### Rebuilt-only regression test

- `Orbs`
- `Threat`
- `Advanced`

## Known Follow-up Work

- live multiplayer regression testing
- optional cleanup of some convenience interactions that are not bug-for-bug identical to the original
- optional richer visualization of rebuilt-only categories

## Packaging Expectations

The project is intentionally small-source-first:

- source code
- docs
- references
- ready-to-test `dist/` output

Local heavyweight packaging tools are not required in the handoff package.
