using System.Collections.Generic;

namespace MaderLand.Config.Trails;

/// <summary>
/// Defines shape of config file maderland/trails.json
/// </summary>
public class TrailsConfig
{
    /// <summary>
    /// Is Trails feature active?
    /// </summary>
    public bool Active { get; set; } = true;

    /// <summary>
    /// Trampling.
    /// </summary>
    public TrailsTrample Trample { get; set; } = new TrailsTrample();

    /// <summary>
    /// List of blocks that can be affected by trampling.
    /// </summary>
    public List<TrailsBlock> Blocks { get; set; } = [];

}
