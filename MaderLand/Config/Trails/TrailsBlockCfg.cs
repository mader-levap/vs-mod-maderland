namespace MaderLand.Config.Trails;

/// <summary>
/// Describes single block affected by trails.
/// </summary>
public class TrailsBlockCfg
{
    /// <summary>
    /// Target block code. When durability hits 0, it will be replaced by block with this code. Empty string means block will be removed.
    /// </summary>
    public string ToBlockCode { get; set; } = "";

    /// <summary>
    /// Durability of block. Walking on this block will reduce that number. If it hits 0, block changes to ToBlockCode.
    /// See Trampling to see power of trampling.
    /// </summary>
    public float Durability { get; set; } = 10;
}
