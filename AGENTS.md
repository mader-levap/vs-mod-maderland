# AGENTS.md — MaderLand Mod by Mader Levap

AI agent context file for the MaderLand Vintage Story mod project.

## Project Overview

**MaderLand** is a "kitchen sink" code mod for Vintage Story (C#, .NET 10). The mod adds various quality-of-life and gameplay features.

GitHub: https://github.com/mader-levap/vs-mod-maderland

## Tech Stack

- Language: C# (.NET 10)
- Game: Vintage Story 1.22+
- Build: Cake (via `CakeBuild/` directory)

## Vintage Story

You can check API for mods:
- API docs: https://apidocs.vintagestory.at/
- Source code: https://github.com/anegostudios/vsapi
Use these resources if you are not sure about particulars of API.

## Project Structure

```
MaderLand/
├─ Commands/                    # Contains command /ml (short for maderland) and its subcommands for every feature
├─ Config/                      # Utilities and config definitions for every feature
├─ Systems/                     # ModSystems for every feature: high-level, timers, game event subscriptions etc.
├─ Utils/                       # Common utility classes
├─ MaderLandModSystem.cs        # Main ModSystem entry point
├─ assets/
│   └─ maderland/               # JSON assets, lang files
└─ modinfo.json
```

## Conventions
- Namespace root: `MaderLand`
- XML doc comments (`///`) on all public classes and methods.

### Commands
- There is main command `/ml` (short for `maderland`). Format: `/ml [feature] [action] [all other parameters, optional]`
- Each feature has subcommand, for example Trails feature has `/ml trails`.
- Each feature has one or more actions. Example: `/ml trails check`.
- All features have at least one action `active` that is used to turn feature on or off. Example: `/ml trails active off`.

### Configuration
- Code loads/saves/creates configuration via generic `ModConfigHandler` class.
- `ConfigService` class contains all configurations and handle them.
- Game-wise, configuration exists in `%AppData%/VintagestoryData/ModConfig/maderland` directory.
- Mod has main config file `maderland.json`.
- Every feature has its own json file. One config class per feature.

## Do Not

- Do not handle unrelated TODO code, unless explicitly asked for.
- Do not hardcode block codes or gameplay values — put them in config.
