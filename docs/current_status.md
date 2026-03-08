# Current Status

## Purpose

This file is the short handoff/status document for continuing the rebuild without relying on chat history.

## Why This Rebuild Exists

The original `DamageMeter` mod works on macOS only when the game runs through Rosetta/x86_64.

It fails on native Apple Silicon because Harmony patch initialization reaches a detour path that throws `System.NotImplementedException`.

The rebuilt mod avoids Harmony and uses the game's public APIs instead.

## Implemented So Far

- .NET 9 project created around `DamageMeterRebuilt.csproj`
- references wired to the game's `sts2.dll` and `GodotSharp.dll`
- `mod_manifest.json` added
- build target now emits the runtime package to `dist/DamageMeter/`
- build now generates `DamageMeter.pck` with original-facing runtime metadata and packaged mod artwork/localization
- official runtime lifecycle wired:
  - `RunManager.RunStarted`
  - `CombatManager.CombatSetUp`
  - `CombatManager.CombatEnded`
  - `CombatManager.TurnStarted`
  - `CombatManager.TurnEnded`
  - `CombatManager.History.Changed`
- `StatsEngine` currently consumes these entries:
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
- original-like in-game panel shell now exists as code-only Godot UI
- hotkey handling now uses a dedicated root-level input node with per-frame key-edge polling:
  - avoids relying on overlay-local input dispatch
  - avoids the unreliable `_UnhandledKeyInput` path observed in this game's macOS build
  - supports `F7`, `F8`, `Shift+F8`, `F9`, and `Escape`
- the main panel now mirrors the original mod structure more closely:
  - top row with `↺`, `≡`, `◀`, title button, `▶`
  - segment row with `◀`, segment label, `▶`
  - scrollable bar list with colored fills
  - no extra footer line
- main player rows now use character-aware visuals:
  - embedded original character icons
  - player color from `Character.NameColor` with fallback rotation
- archived segment labels now include localized encounter names when available, matching the original `Fight N: Encounter` shape
- detail titles now match the original behavior across `Current`, `Overall`, and archived fights:
  - `Death Log`
  - `Combat Log`
  - `Energy` detail with wasted-energy suffix
- rebuilt UI labels now load from embedded `en` / `zhs` localization json derived from the original mod
- category selection now uses a closer clone of the original popup panel:
  - 3-column button grid
  - highlighted active category
  - bottom-right settings button
- settings now use the original submenu shape:
  - `Scale`
  - `Opacity`
  - `Max Bars`
  - `Auto-reset on new run`
  - `Reset Position`
- the `≡` button now opens a full-screen dashboard overlay modeled after the original:
  - dimmed background
  - header with title and close button
  - four-column grid of mini panels
  - each mini panel supports detail drilldown and back navigation
- reset now uses a confirmation dialog instead of wiping data immediately
- title button now opens a category menu when not in detail and acts as a back button in detail view
- segment label now opens a segment menu with `Current`, `Overall`, and archived `Fight N` entries
- a lightweight settings menu is now available from the category menu:
  - scale presets
  - opacity presets
  - max bar presets
  - auto-reset toggle
  - reset position
- `DPT` and `Overkill` categories were added to match the original category set
- archived fights can now be revisited directly from the segment menu
- overlay visibility and panel position are now persisted in `DamageMeterRebuilt_settings.json`
- panel dragging is implemented for quick in-game layout testing
- overlay now supports multiple views:
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
  - `Orbs`
  - `Threat`
  - `Advanced`
- current panel can switch between `Current`, `Overall`, and archived `Fight N`
- rebuild now imports legacy `DamageMeter_settings.json` defaults when no rebuilt settings file exists
- rebuilt-only extension categories now surface official-history data that the original mod never exposed:
  - `Orbs`: orb-channel counts and per-orb breakdowns
  - `Threat`: monster move targeting counts, attributed to actual player targets
  - `Advanced`: generated cards, afflictions, summons, and star changes

## Build

From the repository root:

```bash
dotnet build DamageMeterRebuilt.csproj \
  -p:GameAppDir="/path/to/SlayTheSpire2.app" \
  -p:GameDataDir="/path/to/SlayTheSpire2.app/Contents/Resources/data_sts2_macos_arm64" \
  -p:GodotPackerPath="/path/to/Godot.app/Contents/MacOS/Godot"
```

Expected outputs:

- `bin/Debug/net9.0/DamageMeter.dll`
- `dist/DamageMeter/DamageMeter.dll`
- `dist/DamageMeter/DamageMeter.pck`
- `dist/DamageMeter/mod_manifest.json`

Optional direct deploy to the local game install:

```bash
dotnet build DamageMeterRebuilt.csproj -t:DeployModToGame \
  -p:GameAppDir="/path/to/SlayTheSpire2.app" \
  -p:GameDataDir="/path/to/SlayTheSpire2.app/Contents/Resources/data_sts2_macos_arm64" \
  -p:GodotPackerPath="/path/to/Godot.app/Contents/MacOS/Godot"
```

Default deploy target:

- `SlayTheSpire2.app/Contents/MacOS/mods/DamageMeter/`

Environment notes:

- `global.json` pins the SDK line; if `dotnet` already works, no Homebrew-specific exports are required
- the checked-in `GameAppDir` in the project file is the original maintainer's local Mac path and should usually be overridden on any other machine
- `Godot 4.5.1` is required for pack generation; `4.6+` can produce incompatible `.pck` files

## Current Product Shape

The rebuild is currently a code-only mod:

- no Harmony
- no Harmony-driven input handling

The current overlay is now effectively an original-style clone for the main in-combat/dashboard surfaces:

- original-like top row, segment row, category popup, settings submenu, reset confirmation, and dashboard grid
- original-like player icons, player colors, localized labels, and archived fight labels
- original-like detail/back behavior on both the main panel and full-screen dashboard

Current input model:

- `F7` toggle panel
- `F8` next category
- `Shift+F8` previous category
- `F9` toggle current/overall
- click title to open the category menu
- click segment label to open the segment menu
- click `≡` to open the full-screen dashboard
- drag panel with left mouse button

## Next High-Value Work

- validate stat attribution edge cases in multiplayer
- continue optional post-clone extensions on top of the now-stable original-style UI

## Active Parity Fixes

The current parity pass has now addressed these original-vs-rebuilt differences:

- `Records` are now persisted through the rebuilt settings file and imported from legacy records when present.
- multiplayer player identity now keys players by `NetId` when available and resolves display names through the game's platform-name path first.
- `DPT` main bars now use average damage per turn instead of best-turn damage.
- `Overkill` now exposes the original three-line detail view (`Damage Dealt`, `Overkill`, `Blocked`).
- `Death Log` snapshots now capture the player's own recent legacy combat-log events instead of a global timeline tail.
- rebuilt debuff attribution now keeps the original enemy-owner restriction.
- panel reset/default placement now returns to the original top-right anchored behavior, and panel opacity only changes the panel background.

## Remaining Known Differences

- function-key usability on macOS can still depend on system keyboard settings even though the mod-side polling path is fixed.
- the rebuilt UI still keeps a few convenience interactions that are not bug-for-bug identical to the original, such as opening menus from additional click targets.
- multiplayer edge cases have not yet been re-validated in a live multiplayer session after the `NetId`/platform-name parity pass.
- the new rebuilt-only categories are intentionally additive and therefore have no original-mod parity target.

## Runtime Packaging Identity

As of 2026-03-08, the source project still lives under `DamageMeterRebuilt/`, but the packaged/deployed runtime identity intentionally matches the original mod for multiplayer compatibility:

- folder: `mods/DamageMeter/`
- files: `DamageMeter.dll`, `DamageMeter.pck`, `mod_manifest.json`
- manifest identity:
  - `pck_name = DamageMeter`
  - `name = Skada: Spire Edition`
  - `author = 皮一下就很凡`
  - `version = 1.1.0`
- pack contents now also include:
  - `res://DamageMeter/mod_image.png`
  - `res://DamageMeter/mod_image.png.import`
  - `res://.godot/imported/mod_image.png-43b80ce458f4c387952d9e3d7794760b.ctex`
  - `res://DamageMeter/localization/en.json`
  - `res://DamageMeter/localization/zhs.json`
