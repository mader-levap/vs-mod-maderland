using System;
using System.Collections.Generic;
using System.Text;

namespace MaderLand.Config.Trails;

/// <summary>
/// Describes single block affected by trails.
/// </summary>
public class TrailsBlock
{
    /// <summary>
    /// Source block code.
    /// </summary>
    public string FromBlockCode { get; set; } = "";

    /// <summary>
    /// Target block code. Can be empty if this trail cannot degrade anymore.
    /// </summary>
    public string ToBlockCode { get; set; } = "";

    /// <summary>
    /// Durability of block. Walking on this block will reduce that number. If it hits 0, block changes to ToBlockCode.
    /// See Trampling to see power of trampling.
    /// </summary>
    public float Durability { get; set; } = 100;

    /// <summary>
    /// Regeneration of block in units per in-game minute. Higher number means it will restore Durability faster.
    /// </summary>
    public float Regeneration { get; set; } = 1;

    /// <summary>
    /// After regenerating block to full durability, this is amount of time in in-game minutes before block becomes normal (all trail data are removed).
    /// </summary>
    public float BackToNormal { get; set; } = 1;
}
