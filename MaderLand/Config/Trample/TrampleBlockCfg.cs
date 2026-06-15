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
    /// Regeneration of block. Specifies amount of in-game days needed for block to regenerate 1 durability point.
    /// </summary>
    public float Regen { get; set; } = 0.1f;

    /// <summary>
    /// Durability ratio to apply when this block is placed due to trampling. Must be from 0 to 1.
    /// </summary>
    public float DurRatio { get; set; } = 0.5f;
}
