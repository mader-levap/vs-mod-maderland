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
    public const string trampleDataKey = "maderland:trampledata";

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
        Blocks = new()
        {
            // tall grass
            ["game:tallgrass-medium-free"] = new() { ToBlockCode = "game:tallgrass-mediumshort-free", Durability = debug ? 3 : 25 },
            ["game:tallgrass-mediumshort-free"] = new() { ToBlockCode = "game:tallgrass-short-free", Durability = debug ? 3 : 20 },
            ["game:tallgrass-short-free"] = new() { ToBlockCode = "game:tallgrass-veryshort-free", Durability = debug ? 3 : 15 },
            ["game:tallgrass-veryshort-free"] = new() { ToBlockCode = "", Durability = debug ? 3 : 10 }
        },
        BlockVariants = [
            // flowers
            new() { FromBlockCode = "game:flower-*-*", ToBlockCode = "", Durability = debug ? 3 : 15 },
            // mushrooms
            new() { FromBlockCode = "game:mushroom-*-*", ToBlockCode = "", Durability = debug ? 3 : 25 },
            // other plants that should be easily trampled
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
            ["game:forestfloor-7"] = new() { ToBlockCode = "game:forestfloor-6", Durability = debug ? 3 : 25 },
            ["game:forestfloor-6"] = new() { ToBlockCode = "game:forestfloor-5", Durability = debug ? 3 : 25 },
            ["game:forestfloor-5"] = new() { ToBlockCode = "game:forestfloor-4", Durability = debug ? 3 : 25 },
            ["game:forestfloor-4"] = new() { ToBlockCode = "game:forestfloor-3", Durability = debug ? 3 : 25 },
            ["game:forestfloor-3"] = new() { ToBlockCode = "game:forestfloor-2", Durability = debug ? 3 : 25 },
            ["game:forestfloor-2"] = new() { ToBlockCode = "game:forestfloor-1", Durability = debug ? 3 : 25 },
            ["game:forestfloor-1"] = new() { ToBlockCode = "game:forestfloor-0", Durability = debug ? 3 : 25 },
            ["game:forestfloor-0"] = new() { ToBlockCode = "game:soil-low-none", Durability = debug ? 3 : 25 },
            // peat
            ["game:peat-verysparse"] = new() { ToBlockCode = "game:peat-none", Durability = debug ? 3 : 15 },
        },
        BlockVariants = [
            // soil
            new() { FromBlockCode = "game:soil-*-normal", ToBlockCode = "game:soil-*-sparse", Durability = debug ? 5 : 50 },
            new() { FromBlockCode = "game:soil-*-sparse", ToBlockCode = "game:soil-*-verysparse", Durability = debug ? 5 : 50 },
            new() { FromBlockCode = "game:soil-*-verysparse", ToBlockCode = "game:soil-*-none", Durability = debug ? 5 : 50 },
            // clay
            new() { FromBlockCode = "game:rawclay-*-verysparse", ToBlockCode = "game:rawclay-*-none", Durability = debug ? 5 : 50 },
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
