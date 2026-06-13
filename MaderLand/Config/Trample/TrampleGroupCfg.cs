using System;
using System.Collections.Generic;
using System.Text;

namespace MaderLand.Config.Trample;

/// <summary>
/// Describes group of blocks affected by trampling.
/// </summary>
public class TrampleGroupCfg
{
    /// <summary>
    /// Dictionary of blocks that can be affected by trampling. Block codes must be exact, no wildcards allowed.
    /// Key is source block code, value is TrampleBlockCfg describing how that block is affected.
    /// </summary>
    public Dictionary<string, TrampleBlockCfg> Blocks { get; set; } = [];

    /// <summary>
    /// List of blocks that can be affected by trampling, but instead of using block code as key, it uses wildcard.
    /// That means that if block code matches FromBlockCode with wildcard, it will be affected by trampling.
    /// This is useful for blocks that have multiple variants, like soil blocks with different fertility levels.
    /// </summary>
    public List<TrampleBlockVariantCfg> BlockVariants { get; set; } = [];
}
