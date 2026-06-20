# AGENTS.md — MaderLand Mod by Mader Levap

AI agent context file for the Vintage Story mod project called MaderLand.

## Project Overview

**MaderLand** is a "kitchen sink" code mod for Vintage Story (C#, .NET 10). The mod adds various quality-of-life and gameplay features.

GitHub: https://github.com/mader-levap/vs-mod-maderland

### Features

- **Trample**: just by walking, you can wear out pathes and trails through terrain. In development.

## Tech Stack

- Language: C# (.NET 10)
- Game: Vintage Story 1.22.3+
- Build: Cake (via `CakeBuild/` directory)

## Vintage Story

You can check API for mods:
- API docs: https://apidocs.vintagestory.at/
- Source code: https://github.com/anegostudios/vsapi
Use these websites if you are not sure about particulars of API.

Also, feel free to search internet for tutorials, other VS mods and similar resources if needed.

## Project Structure

### Main project

```
MaderLand/
├─ assets/
│   └─ maderland/               # JSON assets, lang files, etc.
├─ Common/                      # Common classes.
│   ├─ Config/                  # Classes to handle configuration.
│   └─ Manager/                 # Feature manager.
├─ Systems/                     # Contains all features.
├─ Utils/                       # Utility classes.
├─ MaderLandModSystem.cs        # Main ModSystem entry point.
└─ modinfo.json                 # Basic definition of Vintage Story mod.
```

### Feature folders
Every feature is present as separate folder in `Systems` folder. In turn, feature folder can contain subfolders described below:
```
FeatureName/
├─ Behaviors/                   # Behaviors.
├─ Blocks/                      # Blocks.
├─ Commands/                    # Console commands for this feature.
├─ Config/                      # Configuration data structure and handler.
├─ Data/                        # Other data structures, constants etc.
├─ Gui/                         # GUI-related classes.
├─ Network/                     # Network packets.
├─ Services/                    # Main codebase for feature. Usually has at least FeatureMain.cs class.
└─ FeatureSystem.cs             # ModSystem entry point for this feature.
```

Note some folders can be missing if not needed by feature.

## Conventions
- Namespace root: `MaderLand`
- XML doc comments (`///`) on all public classes and methods.

### Commands
- There is main command `/ml` (short for `maderland`). Format: `/ml [feature] [action] [all other parameters, optional]`
- Each feature has subcommand, for example Trails feature has `/ml trample`.
- Each feature has one or more actions. Example: `/ml trample check`.
- All features have at least these actions below:
  - action `active` that is used to turn whole feature on or off. Example: `/ml trample active off`.
  - action `debug` that is used to turn debug mode for this feature on or off. Example: `/ml trample debug on`.

### Configuration
- Code loads/saves/creates configuration via generic `ModConfigHandler` class.
- `ConfigService` class contains all configurations and handle them.
- Game-wise, configuration exists in `%AppData%/VintagestoryData/ModConfig/maderland` directory.
- Mod has main config file `maderland.json`.
- Every feature has its own json file. `Config/` subfolder contains data about it.

## Do Not

- Do not handle unrelated TODO code, unless explicitly asked for.
- Do not hardcode block codes or gameplay values — put them in config.
