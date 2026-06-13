using MaderLand.Config.Trample;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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
            // tallgrass
            ["game:tallgrass-medium-free"] = new() { ToBlockCode = "game:tallgrass-mediumshort-free", Durability = debug ? 3 : 25 },
            ["game:tallgrass-mediumshort-free"] = new() { ToBlockCode = "game:tallgrass-short-free", Durability = debug ? 3 : 20 },
            ["game:tallgrass-short-free"] = new() { ToBlockCode = "game:tallgrass-veryshort-free", Durability = debug ? 3 : 15 },
            ["game:tallgrass-veryshort-free"] = new() { ToBlockCode = "", Durability = debug ? 4 : 10 }
        },
        BlockVariants = []
    };

    /// <summary>
    /// Default configuration for impassable blocks affected by trampling.
    /// </summary>
    public static readonly TrampleGroupCfg DefaultImpassable = new()
    {
        Blocks = [],
        BlockVariants = [
            // soil
            new() { FromBlockCode = "game:soil-*-normal", ToBlockCode = "game:soil-*-sparse", Durability = debug ? 5 : 50 },
            new() { FromBlockCode = "game:soil-*-sparse", ToBlockCode = "game:soil-*-verysparse", Durability = debug ? 5 : 50 },
            new() { FromBlockCode = "game:soil-*-verysparse", ToBlockCode = "game:soil-*-none", Durability = debug ? 5 : 50 },
        ]
    };
};
