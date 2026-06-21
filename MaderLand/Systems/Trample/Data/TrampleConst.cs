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
    /// Default items that should contribute to trampling when worn.
    /// </summary>
    public static readonly List<TrampleItemCfg> DefaultItems = [
        // specific
        new() { Code = "game:clothes-foot-metalcap-*", Power = 0.25f, Damaged = false },
        new() { Code = "game:armor-*-hide-*", Power = 0.15f, Damaged = false },
        new() { Code = "game:armor-*-antique-*", Power = 0.4f, Damaged = false },

        // general
        new() { Code = "game:clothes-*-foot-*", Power = 0.1f, Damaged = false },
        new() { Code = "game:clothes-foot-*", Power = 0.1f, Damaged = false },

        new() { Code = "game:armor-*-linen", Power = 0.1f, Damaged = false },
        new() { Code = "game:armor-*-leather", Power = 0.15f, Damaged = false },
        new() { Code = "game:armor-*-wood", Power = 0.35f, Damaged = false },
        new() { Code = "game:armor-*-silver", Power = 0.5f, Damaged = false },
        new() { Code = "game:armor-*-gold", Power = 1.0f, Damaged = false }, // gold is heavy!
        new() { Code = "game:armor-*-copper", Power = 0.4f, Damaged = false },
        new() { Code = "game:armor-*-tinbronze", Power = 0.4f, Damaged = false },
        new() { Code = "game:armor-*-bismuthbronze", Power = 0.4f, Damaged = false },
        new() { Code = "game:armor-*-blackbronze", Power = 0.4f, Damaged = false },
        new() { Code = "game:armor-*-iron", Power = 0.4f, Damaged = false },
        new() { Code = "game:armor-*-meteoriciron", Power = 0.4f, Damaged = false },
        new() { Code = "game:armor-*-steel", Power = 0.4f, Damaged = false },
    ];

    /// <summary>
    /// Default entities that should be able to trample blocks.
    /// Besides player, all larger animals are added. Note no children trample. If bear cubs existed, they would be added next to hyenas, alas.
    /// </summary>
    public static readonly List<TrampleEntityCfg> DefaultEntities = [
        // humanoid
        new() { Code = "game:player", Power = 1, FallMul = 1 },
        new() { Code = "game:trader-*", Power = 1, FallMul = 1 },
        new() { Code = "game:villager-*", Power = 1, FallMul = 1 },

        // specific species: largest
        new() { Code = "game:goat-muskox-adult-*", Power = 3, FallMul = 3 },
        new() { Code = "game:deer-moose-adult-*", Power = 3, FallMul = 3 },
        
        // specific species: large
        new() { Code = "game:tameddeer-elk-adult-*", Power = 2, FallMul = 2 },
        new() { Code = "game:deer-elk-adult-*", Power = 2, FallMul = 2 },

        // specific species: medium
        new() { Code = "game:bear-sun-adult-*", Power = 1, FallMul = 1 },
        
        // specific species: small
        new() { Code = "game:deer-pampas-adult-*", Power = 0.25f, FallMul = 2 },
        new() { Code = "game:deer-pudu-adult-*", Power = 0.25f, FallMul = 2 },

        // kinds: large
        new() { Code = "game:bear-*-adult-*", Power = 3, FallMul = 2 },

        // kinds: medium
        new() { Code = "game:wolf-*-adult-*", Power = 1, FallMul = 1 },
        new() { Code = "game:pig-*-adult-*", Power = 1, FallMul = 1 },
        new() { Code = "game:sheep-*-adult-*", Power = 1, FallMul = 1 },
        new() { Code = "game:deer-*-adult-*", Power = 1, FallMul = 1 },
        new() { Code = "game:goat-*-adult-*", Power = 1, FallMul = 1 },
        new() { Code = "game:gazelle-*-adult-*", Power = 1, FallMul = 1 },

        // kinds: small (that still trample)
        new() { Code = "game:hyena-*-adult-*", Power = 0.25f, FallMul = 0.5f },

        // rust world denizens
        new() { Code = "game:drifter-*", Power = 1, FallMul = 2 },
        new() { Code = "game:bowtorn-*", Power = 1, FallMul = 2 },
        new() { Code = "game:shiver-*", Power = 1, FallMul = 2 },
    ];

    /// <summary>
    /// Default configuration for passable blocks affected by trampling.
    /// </summary>
    public static readonly TrampleGroupCfg DefaultPassable = new()
    {
        Blocks = new()
        {
            // snow
            ["game:snowlayer-7"] = new() { ToBlockCode = "game:snowlayer-6", Durability = 15, Regen = 0.0f, DurRatio = 1.0f },
            ["game:snowlayer-6"] = new() { ToBlockCode = "game:snowlayer-5", Durability = 15, Regen = 0.0f, DurRatio = 1.0f },
            ["game:snowlayer-5"] = new() { ToBlockCode = "game:snowlayer-4", Durability = 15, Regen = 0.0f, DurRatio = 1.0f },
            ["game:snowlayer-4"] = new() { ToBlockCode = "game:snowlayer-3", Durability = 15, Regen = 0.0f, DurRatio = 1.0f },
            ["game:snowlayer-3"] = new() { ToBlockCode = "game:snowlayer-2", Durability = 15, Regen = 0.0f, DurRatio = 1.0f },
            ["game:snowlayer-2"] = new() { ToBlockCode = "game:snowlayer-1", Durability = 15, Regen = 0.0f, DurRatio = 1.0f },
            // special grass case
            ["game:tallgrass-eaten-free"] = new() { ToBlockCode = "", Durability = 5, Regen = 2.0f, DurRatio = 0.5f },
        },
        BlockVariants = [
            // snowed passable blocks (for example grass or loose sticks) will trample into passable blocks without snow
            new() { FromBlockCode = "game:*-snow7", ToBlockCode = "game:*-snow6", Durability = 15, Regen = 2.0f, DurRatio = 1.0f },
            new() { FromBlockCode = "game:*-snow6", ToBlockCode = "game:*-snow5", Durability = 15, Regen = 2.0f, DurRatio = 1.0f },
            new() { FromBlockCode = "game:*-snow5", ToBlockCode = "game:*-snow4", Durability = 15, Regen = 2.0f, DurRatio = 1.0f },
            new() { FromBlockCode = "game:*-snow4", ToBlockCode = "game:*-snow3", Durability = 15, Regen = 2.0f, DurRatio = 1.0f },
            new() { FromBlockCode = "game:*-snow3", ToBlockCode = "game:*-snow2", Durability = 15, Regen = 2.0f, DurRatio = 1.0f },
            new() { FromBlockCode = "game:*-snow2", ToBlockCode = "game:*-snow", Durability = 15, Regen = 2.0f, DurRatio = 1.0f },
            new() { FromBlockCode = "game:*-snow", ToBlockCode = "game:*-free", Durability = 15, Regen = 2.0f, DurRatio = 1.0f },
                        
            // actual grass (not cover of soil block)
            new() { FromBlockCode = "game:tallgrass-verytall-*", ToBlockCode = "game:tallgrass-tall-*", Durability = 30, Regen = 2.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:tallgrass-tall-*", ToBlockCode = "game:tallgrass-medium-*", Durability = 25, Regen = 2.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:tallgrass-medium-*", ToBlockCode = "game:tallgrass-mediumshort-*", Durability = 20, Regen = 2.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:tallgrass-mediumshort-*", ToBlockCode = "game:tallgrass-short-*", Durability = 15, Regen = 2.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:tallgrass-short-*", ToBlockCode = "game:tallgrass-veryshort-*", Durability = 10, Regen = 2.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:tallgrass-veryshort-*", ToBlockCode = "game:tallgrass-eaten-*", Durability = 5, Regen = 2.0f, DurRatio = 0.5f },

            // flowers
            new() { FromBlockCode = "game:flower-*-*", ToBlockCode = "", Durability = 35, Regen = 1.0f, DurRatio = 0.5f },
            // mushrooms
            new() { FromBlockCode = "game:mushroom-*-*", ToBlockCode = "", Durability = 35, Regen = 1.0f, DurRatio = 0.5f },
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
        Items = DefaultItems,
        Entities = DefaultEntities,
        Passable = DefaultPassable,
        Impassable = DefaultImpassable,
    };
};
