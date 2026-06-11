using System.Collections.Generic;

namespace MaderLand.Config.Trample;

/// <summary>
/// Defines shape of config file maderland/trample.json.
/// </summary>
public class TrampleCfg
{
    /// <summary>
    /// Is Trample feature active?
    /// </summary>
    public bool Active { get; set; } = true;

    /// <summary>
    /// Trampling data.
    /// </summary>
    public TramplePowerCfg Power { get; set; } = new TramplePowerCfg();

    /// <summary>
    /// Dictionary of blocks that can be affected by trampling. Block codes must be exact, no wildcards allowed.
    /// Key is source block code, value is TrampleBlockCfg describing how that block is affected.
    /// </summary>
    public Dictionary<string, TrampleBlockCfg> Blocks { get; set; } = new()
    {
        // tallgrass
        ["game:tallgrass-medium-free"] = new () { ToBlockCode = "game:tallgrass-mediumshort-free", Durability = 25 },
        ["game:tallgrass-mediumshort-free"] = new () { ToBlockCode = "game:tallgrass-short-free", Durability = 20 },
        ["game:tallgrass-short-free"] = new() { ToBlockCode = "game:tallgrass-veryshort-free", Durability = 15 },
        ["game:tallgrass-veryshort-free"] = new() { ToBlockCode = "", Durability = 10 }
    };

    /// <summary>
    /// List of blocks that can be affected by trampling, but instead of using block code as key, it uses wildcard.
    /// That means that if block code matches FromBlockCode with wildcard, it will be affected by trampling.
    /// This is useful for blocks that have multiple variants, like soil blocks with different fertility levels.
    /// </summary>
    public List<TrampleBlockCfg> BlockVariants { get; set; } = new List<TrampleBlockCfg>()
    {
        // soil
        new() { FromBlockCode = "game:soil-*-normal", ToBlockCode = "game:soil-*-sparse", Durability = 50 },
        new() { FromBlockCode = "game:soil-*-sparse", ToBlockCode = "game:soil-*-verysparse", Durability = 50 },
        new() { FromBlockCode = "game:soil-*-verysparse", ToBlockCode = "game:soil-*-none", Durability = 50 },
    };
}
