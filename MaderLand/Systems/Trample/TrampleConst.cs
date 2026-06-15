using MaderLand.Config.Trample;

namespace MaderLand.Systems.Trample;

/// <summary>
/// Constants for Trample feature.
/// </summary>
public static class TrampleConst
{
    private const bool debug = true;

    // ////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// The unique data key used to store trample data in chunk mod data.
    /// </summary>
    public const string trampleDataKey = "maderland:trample";

    /// <summary>
    /// Empty block code. Used when block should be removed after trampling.
    /// </summary>
    public const string emptyBlock = "game:air";

    //

    /// <summary>
    /// Default configuration for passable blocks affected by trampling.
    /// </summary>
    public static readonly TrampleGroupCfg DefaultPassable = new()
    {
        Blocks = [],
        BlockVariants = [
            // tall grass
            new() { FromBlockCode = "game:tallgrass-medium-*", ToBlockCode = "game:tallgrass-mediumshort-*", Durability = debug ? 3 : 25, Regen = 1.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:tallgrass-mediumshort-*", ToBlockCode = "game:tallgrass-short-*", Durability = debug ? 3 : 20, Regen = 1.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:tallgrass-short-*", ToBlockCode = "game:tallgrass-veryshort-*", Durability = debug ? 3 : 15, Regen = 1.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:tallgrass-veryshort-*", ToBlockCode = "game:tallgrass-eaten-*", Durability = debug ? 3 : 10, Regen = 1.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:tallgrass-eaten-*", ToBlockCode = "", Durability = debug ? 3 : 10, Regen = 1.0f, DurRatio = 0.5f },
            // flowers
            new() { FromBlockCode = "game:flower-*-*", ToBlockCode = "", Durability = debug ? 3 : 15, Regen = 1.0f, DurRatio = 0.5f },
            // mushrooms
            new() { FromBlockCode = "game:mushroom-*-*", ToBlockCode = "", Durability = debug ? 3 : 25, Regen = 1.0f, DurRatio = 0.5f },
            // ferns
            new() { FromBlockCode = "game:fern-*", ToBlockCode = "", Durability = debug ? 3 : 35, Regen = 1.0f, DurRatio = 0.5f },
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
            ["game:forestfloor-7"] = new() { ToBlockCode = "game:forestfloor-6", Durability = debug ? 4 : 25, Regen = 1.0f, DurRatio = 0.5f },
            ["game:forestfloor-6"] = new() { ToBlockCode = "game:forestfloor-5", Durability = debug ? 4 : 25, Regen = 1.0f, DurRatio = 0.5f },
            ["game:forestfloor-5"] = new() { ToBlockCode = "game:forestfloor-4", Durability = debug ? 4 : 25, Regen = 1.0f, DurRatio = 0.5f },
            ["game:forestfloor-4"] = new() { ToBlockCode = "game:forestfloor-3", Durability = debug ? 4 : 25, Regen = 1.0f, DurRatio = 0.5f },
            ["game:forestfloor-3"] = new() { ToBlockCode = "game:forestfloor-2", Durability = debug ? 4 : 25, Regen = 1.0f, DurRatio = 0.5f },
            ["game:forestfloor-2"] = new() { ToBlockCode = "game:forestfloor-1", Durability = debug ? 4 : 25, Regen = 1.0f, DurRatio = 0.5f },
            ["game:forestfloor-1"] = new() { ToBlockCode = "game:forestfloor-0", Durability = debug ? 4 : 25, Regen = 1.0f, DurRatio = 0.5f },
            ["game:forestfloor-0"] = new() { ToBlockCode = "game:soil-low-none", Durability = debug ? 4 : 25, Regen = 1.0f, DurRatio = 0.5f },
            // peat
            ["game:peat-verysparse"] = new() { ToBlockCode = "game:peat-none", Durability = debug ? 4 : 15, Regen = 1.0f, DurRatio = 0.5f },
            ["game:peat-none"] = new() { ToBlockCode = "game:peat-none", Durability = debug ? 4 : 15, Regen = 5.0f, DurRatio = 0.5f }, // special
        },
        BlockVariants = [
            // soil
            new() { FromBlockCode = "game:soil-*-normal", ToBlockCode = "game:soil-*-sparse", Durability = debug ? 6 : 50, Regen = 1.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:soil-*-sparse", ToBlockCode = "game:soil-*-verysparse", Durability = debug ? 6 : 50, Regen = 1.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:soil-*-verysparse", ToBlockCode = "game:soil-*-none", Durability = debug ? 6 : 50, Regen = 1.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:soil-*-none", ToBlockCode = "game:soil-*-none", Durability = debug ? 6 : 50, Regen = 5.0f, DurRatio = 0.5f }, // special
            // clay
            new() { FromBlockCode = "game:rawclay-*-verysparse", ToBlockCode = "game:rawclay-*-none", Durability = debug ? 6 : 50, Regen = 1.0f, DurRatio = 0.5f },
            new() { FromBlockCode = "game:rawclay-*-none", ToBlockCode = "game:rawclay-*-none", Durability = debug ? 6 : 50, Regen = 5.0f, DurRatio = 0.5f }, // special
        ]
    };

    /// <summary>
    /// Default configuration for Trample feature, used when config file is missing.
    /// </summary>
    public static readonly TrampleCfg DefaultTrampleCfg = new()
    {
        Passable = DefaultPassable,
        Impassable = DefaultImpassable,
    };
};
