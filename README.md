# DamageMeter Rebuilt

`DamageMeter Rebuilt` is a native `arm64` rebuild of the original `DamageMeter` mod for Slay the Spire 2.

For multiplayer/runtime compatibility, the packaged mod now ships under the original `DamageMeter` identity:
- folder: `mods/DamageMeter/`
- payloads: `DamageMeter.dll`, `DamageMeter.pck`
- manifest: `pck_name = DamageMeter`, `name = Skada: Spire Edition`, `version = 1.1.0`

It preserves the original mod's core UX and stat model, but does not use Harmony. Instead, it reads the game's public combat lifecycle and `CombatHistory` entries directly.

## Why This Exists

The original `DamageMeter` mod works on macOS only through Rosetta/x86_64.

On native Apple Silicon, its Harmony patch path reaches a detour implementation that throws during initialization. This rebuild avoids that entire runtime patching path.

## Current State

- native `arm64` macOS compatible
- original-style main panel and dashboard UI
- original-style stat categories and records
- additional rebuilt-only categories:
  - `Orbs`
  - `Threat`
  - `Advanced`
- main remaining caveat:
  - multiplayer has code-level parity work, but has not been fully regression-tested in a live multiplayer session

## Build Requirements

- `.NET 9`
- `Godot 4.5.1`
- local Slay the Spire 2 install for `sts2.dll` and `GodotSharp.dll`

The project defaults to looking for the game here:

`/Users/laysath/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.app`

Override paths through MSBuild properties if needed:

- `GameAppDir`
- `GameDataDir`
- `GameModsDir`
- `GodotPackerPath`

## Build

```bash
export DOTNET_ROOT=/opt/homebrew/opt/dotnet@9/libexec
export PATH=/opt/homebrew/opt/dotnet@9/bin:$PATH
dotnet build
```

Build output:

- `bin/Debug/net9.0/DamageMeter.dll`
- `dist/DamageMeter/DamageMeter.dll`
- `dist/DamageMeter/DamageMeter.pck`
- `dist/DamageMeter/mod_manifest.json`

Deploy directly into the local game install:

```bash
dotnet build -t:DeployModToGame
```

## Project Layout

- `Domain/`
  core stat/session models
- `Engine/`
  `CombatHistoryEntry` to stat-aggregation logic
- `Lifecycle/`
  run/combat/history/turn bridges
- `Infrastructure/`
  settings, localization, logging, naming, player visuals
- `Views/`
  main panel, dashboard, input handling
- `Resources/`
  localization json and character icons
- `Packaging/`
  `.pck` generation scripts
- `docs/`
  reverse-engineering notes, API notes, current state, maintainer handoff
- `reference/`
  extracted original mod source references used during rebuild/parity work

## Key Docs

- `docs/current_status.md`
- `docs/maintainer_handoff.md`
- `docs/analysis_notes.md`
- `docs/api_reference.md`

## Input Model

The rebuild currently uses a root-level global input node, separate from the panel itself.

Supported keys:

- `F7`: toggle main panel
- `F8`: next category
- `Shift+F8`: previous category
- `F9`: toggle `Current` / `Overall`
- `Escape`: close dashboard

## Packaging Notes

This handoff package intentionally does not bundle a local `Godot.app` or Godot zip. The project can use any externally installed `Godot 4.5.1` via `GodotPackerPath` or the `godot` command in `PATH`.
