using MaderLand.Systems.Trample.Config;
using System.Collections.Generic;

namespace MaderLand.Systems.Trample.Data;

/// <summary>
/// Constants for Trample feature.
/// </summary>
public static class TrampleConst
{
    /// <summary>
    /// Render key used to register renderer "BlockSelectionWatcher".
    /// </summary>
    public const string renderWatcherKey = "maderland-trample-watcher";

    /// <summary>
    /// The unique channel name used to send trample-related data between server and client.
    /// </summary>
    public const string channelKey = "maderland-trample";

    /// <summary>
    /// The unique data name used to store trample data in chunk mod data.
    /// </summary>
    public const string dataChunkKey = "maderland:trample";

    /// <summary>
    /// Empty block code. Used when block should be removed after trampling.
    /// </summary>
    public const string emptyBlock = "game:air";

    //

    /// <summary>
    /// Default entities that should be able to trample blocks.
    /// Besides player, all larger animals are added. Note no children trample. If bear cubs existed, they would be added next to hyenas, alas.
    /// </summary>
    public static readonly List<TrampleEntityCfg> DefaultEntities = [
        // humanoid
        new() { EntityCode = "game:player", Power = 1, FallMul = 1 },
        new() { EntityCode = "game:trader-*-*-*", Power = 1, FallMul = 1 },
        new() { EntityCode = "game:villager-*-*-*-*", Power = 1, FallMul = 1 },

        // specific species: largest
        new() { EntityCode = "game:goat-muskox-adult-*", Power = 3, FallMul = 3 },
        new() { EntityCode = "game:deer-moose-adult-*", Power = 3, FallMul = 3 },
        
        // specific species: large
        new() { EntityCode = "game:tameddeer-elk-adult-*", Power = 2, FallMul = 2 },
        new() { EntityCode = "game:deer-elk-adult-*", Power = 2, FallMul = 2 },

        // specific species: medium
        new() { EntityCode = "game:bear-sun-adult-*", Power = 1, FallMul = 1 },
        
        // specific species: small
        new() { EntityCode = "game:deer-pampas-adult-*", Power = 0.25f, FallMul = 2 },
        new() { EntityCode = "game:deer-pudu-adult-*", Power = 0.25f, FallMul = 2 },

        // kinds: large
        new() { EntityCode = "game:bear-*-adult-*", Power = 3, FallMul = 2 },

        // kinds: medium
        new() { EntityCode = "game:wolf-*-adult-*", Power = 1, FallMul = 1 },
        new() { EntityCode = "game:pig-*-adult-*", Power = 1, FallMul = 1 },
        new() { EntityCode = "game:sheep-*-adult-*", Power = 1, FallMul = 1 },
        new() { EntityCode = "game:deer-*-adult-*", Power = 1, FallMul = 1 },
        new() { EntityCode = "game:goat-*-adult-*", Power = 1, FallMul = 1 },
        new() { EntityCode = "game:gazelle-*-adult-*", Power = 1, FallMul = 1 },

        // kinds: small (that still trample)
        new() { EntityCode = "game:hyena-*-adult-*", Power = 0.25f, FallMul = 0.5f },

        // rust world denizens
        new() { EntityCode = "game:drifter-*", Power = 1, FallMul = 2 },
        new() { EntityCode = "game:bowtorn-*", Power = 1, FallMul = 2 },
        new() { EntityCode = "game:shiver-*", Power = 1, FallMul = 2 },
    ];

    /// <summary>
    /// Default configuration for passable blocks affected by trampling.
    /// </summary>
    public static readonly TrampleGroupCfg DefaultPassable = new()
    {
        Blocks = [],
        BlockVariants = [
            // actual grass (not cover of soil block)
            new() { FromBlockCode = "game:tallgrass-verytall-*", ToBlockCode = "game:tallgrass-tall-*", Durability = 30, Regen = 2.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:tallgrass-tall-*", ToBlockCode = "game:tallgrass-medium-*", Durability = 25, Regen = 2.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:tallgrass-medium-*", ToBlockCode = "game:tallgrass-mediumshort-*", Durability = 20, Regen = 2.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:tallgrass-mediumshort-*", ToBlockCode = "game:tallgrass-short-*", Durability = 15, Regen = 2.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:tallgrass-short-*", ToBlockCode = "game:tallgrass-veryshort-*", Durability = 10, Regen = 2.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:tallgrass-veryshort-*", ToBlockCode = "game:tallgrass-eaten-*", Durability = 5, Regen = 2.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:tallgrass-eaten-*", ToBlockCode = "", Durability = 2.5f, Regen = 2.0f, DurRatio = 0.5f },
            // flowers
            new() { FromBlockCode = "game:flower-*-*", ToBlockCode = "", Durability = 20, Regen = 1.0f, DurRatio = 0.5f },
            // mushrooms
            new() { FromBlockCode = "game:mushroom-*-*", ToBlockCode = "", Durability = 20, Regen = 1.0f, DurRatio = 0.5f },
            // ferns
            new() { FromBlockCode = "game:fern-*", ToBlockCode = "", Durability = 35, Regen = 1.0f, DurRatio = 0.5f },
        ]
    };

    /// <summary>
    /// Default configuration for impassable blocks affected by trampling.
    /// </summary>
    public static readonly TrampleGroupCfg DefaultImpassable = new()
    {
        Blocks = new()
        {
            // forest floor
            ["game:forestfloor-7"] = new() { ToBlockCode = "game:forestfloor-6", Durability = 25, Regen = 2.0f, DurRatio = 0.5f },
            ["game:forestfloor-6"] = new() { ToBlockCode = "game:forestfloor-5", Durability = 25, Regen = 2.0f, DurRatio = 0.5f },
            ["game:forestfloor-5"] = new() { ToBlockCode = "game:forestfloor-4", Durability = 25, Regen = 2.0f, DurRatio = 0.5f },
            ["game:forestfloor-4"] = new() { ToBlockCode = "game:forestfloor-3", Durability = 25, Regen = 2.0f, DurRatio = 0.5f },
            ["game:forestfloor-3"] = new() { ToBlockCode = "game:forestfloor-2", Durability = 25, Regen = 2.0f, DurRatio = 0.5f },
            ["game:forestfloor-2"] = new() { ToBlockCode = "game:forestfloor-1", Durability = 25, Regen = 2.0f, DurRatio = 0.5f },
            ["game:forestfloor-1"] = new() { ToBlockCode = "game:forestfloor-0", Durability = 25, Regen = 2.0f, DurRatio = 0.5f },
            ["game:forestfloor-0"] = new() { ToBlockCode = "game:soil-low-none", Durability = 25, Regen = 2.0f, DurRatio = 0.5f },
            // peat
            ["game:peat-verysparse"] = new() { ToBlockCode = "game:peat-none", Durability = 15, Regen = 2.0f, DurRatio = 0.5f },
            ["game:peat-none"] = new() { ToBlockCode = "game:peat-none", Durability = 15, Regen = 5.0f, DurRatio = 0.5f }, // special
        },
        BlockVariants = [
            // soil
            new() { FromBlockCode = "game:soil-*-normal", ToBlockCode = "game:soil-*-sparse", Durability = 25, Regen = 2.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:soil-*-sparse", ToBlockCode = "game:soil-*-verysparse", Durability = 25, Regen = 2.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:soil-*-verysparse", ToBlockCode = "game:soil-*-none", Durability = 25, Regen = 2.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:soil-*-none", ToBlockCode = "game:soil-*-none", Durability = 25, Regen = 5.0f, DurRatio = 0.5f }, // special
            // clay
            new() { FromBlockCode = "game:rawclay-*-verysparse", ToBlockCode = "game:rawclay-*-none", Durability = 15, Regen = 2.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:rawclay-*-none", ToBlockCode = "game:rawclay-*-none", Durability = 15, Regen = 5.0f, DurRatio = 0.5f }, // special
        ]
    };

    /// <summary>
    /// Default configuration for Trample feature, used when config file is missing.
    /// </summary>
    public static readonly TrampleCfg DefaultTrampleCfg = new()
    {
        Entities = DefaultEntities,
        Passable = DefaultPassable,
        Impassable = DefaultImpassable,
    };
};
