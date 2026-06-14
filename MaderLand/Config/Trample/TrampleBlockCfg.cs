namespace MaderLand.Config.Trample;

/// <summary>
/// Describes single block affected by trampling.
/// </summary>
public class TrampleBlockCfg
{
    /// <summary>
    /// Target block code. When durability hits 0, it will be replaced by block with this code. Empty string means block will be removed (replaced with air block).
    /// </summary>
    public string ToBlockCode { get; set; } = "";

    /// <summary>
    /// Durability of block. Walking on this block will reduce that number.
    /// </summary>
    public float Durability { get; set; } = 10;

    /// <summary>
    /// Regeneration of block. Specifies amount of in-game minutes needed for block to regenerate 1 durability point.
    /// </summary>
    public float Regen { get; set; } = 10;
}
