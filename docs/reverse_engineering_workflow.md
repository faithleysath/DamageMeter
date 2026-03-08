# Reverse Engineering Workflow

This file explains how to continue extracting STS2 runtime knowledge for this project.

For the generic investigation loop, read the copied skill first:

- [deep-analysis skill](./skills/deep-analysis/SKILL.md)

This document only adds the project-specific rules, evidence targets, and documentation expectations.

## Primary Goal

When reverse engineering for this repository, the usual goal is not "understand all of `sts2.dll`".

The real goal is narrower:

- find a stable public runtime entry point
- confirm its shape with evidence
- use it instead of Harmony if possible
- record the result so the next maintainer does not need chat context

## What To Read Before Touching The Binary

Always start here:

- [analysis_notes.md](./analysis_notes.md)
- [api_reference.md](./api_reference.md)
- [current_status.md](./current_status.md)
- [reference/original_dll_project](../reference/original_dll_project)

This avoids re-solving questions that are already settled, such as:

- why Harmony was removed
- which `CombatHistoryEntry` types are already confirmed
- which manager events are already proven public
- how the original mod mapped stats to runtime events

## Core Project Rule

Prefer this order:

1. public manager events
2. `CombatHistory` entries
3. official hook/model systems if truly needed
4. Harmony only as a last resort

For this project, Harmony is considered a fallback of last resort because the original arm64 failure happened in the Harmony/MonoMod detour path.

## Main Investigation Targets

When adding a new feature or fixing parity, the most productive targets are usually:

- `RunManager`
- `CombatManager`
- `CombatHistory`
- `CombatStateTracker`
- relevant `CombatHistoryEntry` subclasses
- specific model types touched by those entries

Questions worth asking:

- is there already a public event for this lifecycle point?
- is there already a `CombatHistoryEntry` for this behavior?
- does the entry contain enough attribution data for the UI/stat we need?
- is this a real runtime path or just dead code?

## Practical Workflow

### 1. Start from a concrete question

Examples:

- "How do I count X without Harmony?"
- "Which entry records this combat action?"
- "Where does this manager expose a public event?"
- "How is this asset packaged into the `.pck`?"

Do not start from a vague "understand the whole game".

### 2. Search existing project evidence

Check:

- [analysis_notes.md](./analysis_notes.md)
- [api_reference.md](./api_reference.md)
- [reference/original_dll_project](../reference/original_dll_project)

If the answer is already there, reuse it.

### 3. Decompile the current game assemblies

For STS2 modding work, the main managed targets are:

- `sts2.dll`
- `GodotSharp.dll`
- optionally the original mod assembly if parity is the question

Typical questions to answer in the decompiler:

- is the member public?
- what is the delegate shape?
- what types are exposed on the entry/model?
- where are the call sites?

### 4. Confirm with call sites, not just member names

Do not stop at "I found a method named `DamageReceived`".

Also confirm:

- who calls it
- whether it is part of real combat flow
- whether the arguments and getters contain the data you need

That is why this repo stores both:

- distilled API facts in [api_reference.md](./api_reference.md)
- longer evidence trails in [analysis_notes.md](./analysis_notes.md)

### 5. Implement through the existing architecture

Once the API route is confirmed:

1. add or update the handler in [StatsEngine.cs](../Engine/StatsEngine.cs)
2. update domain models under [Domain](../Domain) if new data must be stored
3. expose it via [DashboardController.cs](../Views/DashboardController.cs)
4. test in game
5. record the API fact and assumptions in docs

## How To Investigate Packaging

This repository includes small Godot helpers under [Packaging/inspect](../Packaging/inspect):

- `list_pack.gd`
  list files inside a `.pck`
- `extract_pack_file.gd`
  extract a raw file from a `.pck`
- `extract_mod_image.gd`
  verify that `ResourceLoader` can actually load the packaged mod image

Use these when the question is about:

- why a `.pck` is not being discovered
- why the mod image is broken
- whether a resource path exists vs is truly loadable

Important lesson already learned in this project:

- a visible `res://DamageMeter/mod_image.png` path was not sufficient by itself
- the `.import` and `.ctex` files also had to be packaged for the mod list artwork to load correctly

## What To Record When You Learn Something New

If you confirm a new API fact, write it down in one of two places:

- [api_reference.md](./api_reference.md)
  for concise, actionable facts
- [analysis_notes.md](./analysis_notes.md)
  for reasoning, tradeoffs, failures, and evidence chains

Record at least:

- exact type/member name
- whether it is public and usable
- what data it exposes
- what feature it supports
- any remaining assumptions or edge cases

## Reverse Engineering Boundaries for This Project

Good reasons to reverse engineer deeper:

- there is no existing `CombatHistoryEntry` for a needed stat
- multiplayer attribution behaves unexpectedly
- a manager event is insufficiently precise
- packaging/runtime identity issues affect compatibility

Bad reasons to reverse engineer deeper:

- curiosity without a feature question
- rewriting confirmed public routes as Harmony patches
- replacing documented evidence with unsupported assumptions

## Suggested Starting Points For Common Tasks

If the task is about:

- lifecycle/reset bugs:
  start with `RunManager`, `CombatManager`, `CombatHistory.Clear()`
- missing stat attribution:
  start with the relevant `CombatHistoryEntry`
- UI mismatch:
  start with the original decompiled UI under [reference/original_dll_project](../reference/original_dll_project)
- mod-list art or manifest issues:
  start with [pack.gd](../Packaging/GodotPackProject/pack.gd) and [Packaging/inspect](../Packaging/inspect)
- multiplayer compatibility:
  start with runtime identity, manifest, and player identity resolution in [NameResolver.cs](../Infrastructure/NameResolver.cs)

## Short Rule Set

- prefer public runtime APIs over patches
- verify with call sites, not names alone
- keep runtime identity stable unless you intentionally want a different mod
- update docs whenever a reverse-engineering result changes project decisions
- use the copied [deep-analysis skill](./skills/deep-analysis/SKILL.md) when the question gets binary-specific
