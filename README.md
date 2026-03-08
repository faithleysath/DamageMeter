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

- `.NET 9 SDK` matching [global.json](/Users/laysath/proj/DamageMeter/global.json)
- `Godot 4.5.1`
- local Slay the Spire 2 install for `sts2.dll` and `GodotSharp.dll`

Important constraints:

- the checked-in `GameAppDir` default inside [DamageMeterRebuilt.csproj](/Users/laysath/proj/DamageMeter/DamageMeterRebuilt.csproj) points to the original maintainer's local Mac path and is only a convenience default
- on any other machine, you should assume you must pass `GameAppDir`
- if you are not targeting the native macOS `arm64` data folder, also pass `GameDataDir`
- do not use `Godot 4.6+` for packing; the game runtime is `4.5.1`, and newer Godot versions can produce incompatible `.pck` files

MSBuild properties you may need to override:

- `GameAppDir`
- `GameDataDir`
- `GameModsDir`
- `GodotPackerPath`

## No-Context Quick Start

1. Ensure `dotnet --version` resolves to an SDK compatible with [global.json](/Users/laysath/proj/DamageMeter/global.json).
2. Ensure `Godot 4.5.1` is available either as `godot` in `PATH` or via `-p:GodotPackerPath=/path/to/Godot`.
3. Find the local game install path.
4. Build from the repository root with explicit properties if you are not on the original maintainer machine.

Example for macOS Apple Silicon:

```bash
dotnet build DamageMeterRebuilt.csproj \
  -p:GameAppDir="/path/to/SlayTheSpire2.app" \
  -p:GameDataDir="/path/to/SlayTheSpire2.app/Contents/Resources/data_sts2_macos_arm64" \
  -p:GodotPackerPath="/path/to/Godot.app/Contents/MacOS/Godot"
```

If `dotnet` is already on `PATH`, no extra `DOTNET_ROOT` export is required.

If you are on the original maintainer machine, plain `dotnet build` also works because the local game path is already embedded in the project file.

Build outputs:

- `bin/Debug/net9.0/DamageMeter.dll`
- `dist/DamageMeter/DamageMeter.dll`
- `dist/DamageMeter/DamageMeter.pck`
- `dist/DamageMeter/mod_manifest.json`

Deploy directly into the local game install:

```bash
dotnet build DamageMeterRebuilt.csproj -t:DeployModToGame \
  -p:GameAppDir="/path/to/SlayTheSpire2.app" \
  -p:GameDataDir="/path/to/SlayTheSpire2.app/Contents/Resources/data_sts2_macos_arm64" \
  -p:GodotPackerPath="/path/to/Godot.app/Contents/MacOS/Godot"
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

This repository intentionally does not bundle a local `Godot.app` or Godot zip. The project can use any externally installed `Godot 4.5.1` via `GodotPackerPath` or the `godot` command in `PATH`.

The mod-list artwork is not just a raw `png`. Packaging also depends on:

- `Resources/mod_image.png.import`
- `Resources/imported/mod_image.png-43b80ce458f4c387952d9e3d7794760b.ctex`

If a future maintainer replaces the mod artwork, they must preserve the Godot import chain or regenerate an equivalent one.
