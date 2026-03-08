# STS2 Modding From Scratch

This file is the shortest project-specific explanation of how Slay the Spire 2 mods are built in this repository's style.

For reverse-engineering workflow details, read:

- [deep-analysis skill](./skills/deep-analysis/SKILL.md)
- [analysis_notes.md](./analysis_notes.md)
- [api_reference.md](./api_reference.md)

## What the Game Expects

At runtime, the game expects a mod folder containing:

- `DamageMeter.dll`
- `DamageMeter.pck`
- `mod_manifest.json`

For this project, the runtime folder is intentionally:

- `mods/DamageMeter/`

even though the source project and namespaces still use `DamageMeterRebuilt`.

## Core Runtime Pieces

An STS2 C# mod in this repo's style has three parts:

1. A C# assembly referenced against the game's managed assemblies.
2. A `.pck` file containing the mod's Godot-side resources and manifest-visible assets.
3. A `mod_manifest.json` file describing the runtime mod identity.

In this repository:

- C# entrypoint: [ModEntry.cs](../ModEntry.cs)
- project/build rules: [DamageMeterRebuilt.csproj](../DamageMeterRebuilt.csproj)
- runtime manifest: [mod_manifest.json](../mod_manifest.json)
- pack generation: [pack.gd](../Packaging/GodotPackProject/pack.gd)

## Toolchain

Required:

- `.NET 9 SDK` compatible with [global.json](../global.json)
- `Godot 4.5.1`
- a local Slay the Spire 2 install

Important constraints:

- use `Godot 4.5.1`, not `4.6+`
- the checked-in `GameAppDir` inside the project file is a machine-local default, not a portable path
- if you are not using the native macOS `arm64` game data directory, override `GameDataDir`

## Minimal Build Loop

Build from the repository root:

```bash
dotnet build DamageMeterRebuilt.csproj \
  -p:GameAppDir="/path/to/SlayTheSpire2.app" \
  -p:GameDataDir="/path/to/SlayTheSpire2.app/Contents/Resources/data_sts2_macos_arm64" \
  -p:GodotPackerPath="/path/to/Godot.app/Contents/MacOS/Godot"
```

Deploy into the game install:

```bash
dotnet build DamageMeterRebuilt.csproj -t:DeployModToGame \
  -p:GameAppDir="/path/to/SlayTheSpire2.app" \
  -p:GameDataDir="/path/to/SlayTheSpire2.app/Contents/Resources/data_sts2_macos_arm64" \
  -p:GodotPackerPath="/path/to/Godot.app/Contents/MacOS/Godot"
```

Expected outputs:

- `bin/Debug/net9.0/DamageMeter.dll`
- `dist/DamageMeter/DamageMeter.dll`
- `dist/DamageMeter/DamageMeter.pck`
- `dist/DamageMeter/mod_manifest.json`

## How the Mod Initializes

The game's mod loader discovers the manifest and `.pck`, then loads the managed assembly and looks for a mod initializer.

This project uses:

- `[ModInitializer("Initialize")]` on [ModEntry.cs](../ModEntry.cs)

Inside `Initialize()`, this project wires:

- localization
- settings persistence
- session/record storage
- stat collection
- combat/run lifecycle bridges
- dashboard UI

If you are creating a new STS2 mod, this is the first file to mirror.

## Recommended Architecture for New Work

This repository has already validated a reliable arm64-safe route:

- prefer public manager events
- prefer `CombatHistory` as the accounting source
- avoid Harmony unless there is no public route

For this project, the preferred sources are:

- `RunManager.RunStarted`
- `CombatManager.CombatSetUp`
- `CombatManager.CombatEnded`
- `CombatManager.TurnStarted`
- `CombatManager.TurnEnded`
- `CombatManager.History.Changed`

That means the usual implementation path for a new stat is:

1. identify a `CombatHistoryEntry` or public manager event
2. map it in [StatsEngine.cs](../Engine/StatsEngine.cs)
3. store or aggregate it through the domain models in [Domain](../Domain)
4. expose it in [DashboardController.cs](../Views/DashboardController.cs)
5. verify it in-game

## Packaging Notes

The `.pck` is not optional in this runtime layout.

This repository's packer currently includes:

- `mod_manifest.json`
- `mod_image.png`
- `mod_image.png.import`
- imported `.ctex` artwork
- localization json

The mod-list artwork specifically depends on the Godot import chain:

- [mod_image.png](../Resources/mod_image.png)
- [mod_image.png.import](../Resources/mod_image.png.import)
- [mod_image ctex](../Resources/imported/mod_image.png-43b80ce458f4c387952d9e3d7794760b.ctex)

If you replace the artwork, preserve or regenerate that chain.

## Source Identity vs Runtime Identity

Do not confuse these:

- source namespace/project: `DamageMeterRebuilt`
- runtime mod identity: `DamageMeter`

This split exists for multiplayer/mod-matching compatibility with the original mod.

When creating an unrelated new mod from this repository as a starting point, you will likely want to change all of:

- `AssemblyName`
- `RuntimeModId`
- `mod_manifest.json`
- deployed folder name
- UI titles and logging prefixes

## Where To Look Next

- broad architecture and current truth: [current_status.md](./current_status.md)
- maintainer workflow: [maintainer_handoff.md](./maintainer_handoff.md)
- confirmed game API surface: [api_reference.md](./api_reference.md)
- reverse-engineering evidence: [analysis_notes.md](./analysis_notes.md)
- project-specific RE workflow: [reverse_engineering_workflow.md](./reverse_engineering_workflow.md)
