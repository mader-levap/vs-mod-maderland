namespace MaderLand.Config.Trample;

/// <summary>
/// Describes single block affected by trampling.
/// </summary>
public class TrampleBlockCfg
{
    /// <summary>
    /// Target block code. When durability hits 0, it will be replaced by block with this code. Empty string means block will be removed.
    /// </summary>
    public string ToBlockCode { get; set; } = "";

    /// <summary>
    /// Durability of block. Walking on this block will reduce that number. If it hits 0, block changes to ToBlockCode.
    /// </summary>
    public float Durability { get; set; } = 10;
}
