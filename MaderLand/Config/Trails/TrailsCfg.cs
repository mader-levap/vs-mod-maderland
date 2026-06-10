using System.Collections.Generic;

namespace MaderLand.Config.Trails;

/// <summary>
/// Defines shape of config file maderland/trails.json.
/// </summary>
public class TrailsCfg
{
    /// <summary>
    /// Is Trails feature active?
    /// </summary>
    public bool Active { get; set; } = true;

    /// <summary>
    /// Trampling data.
    /// </summary>
    public TrailsTrampleCfg Trample { get; set; } = new TrailsTrampleCfg();

    /// <summary>
    /// List of blocks that can be affected by trampling.
    /// Key is source block code, value is TrailsBlockCfg describing how that block is affected.
    /// </summary>
    public Dictionary<string, TrailsBlockCfg> Blocks { get; set; } = new()
    {
      ["game:tallgrass-medium-free"] = new () { ToBlockCode = "game:tallgrass-mediumshort-free", Durability = 25 },
      ["game:tallgrass-mediumshort-free"] = new () { ToBlockCode = "game:tallgrass-short-free", Durability = 20 },
      ["game:tallgrass-short-free"] = new() { ToBlockCode = "game:tallgrass-veryshort-free", Durability = 15 },
      ["game:tallgrass-veryshort-free"] = new() { ToBlockCode = "", Durability = 10 }
    };
}
