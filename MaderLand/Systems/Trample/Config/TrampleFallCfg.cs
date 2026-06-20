namespace MaderLand.Systems.Trample.Config;

/// <summary>
/// Configures all things related to falling.
/// </summary>
public class TrampleFallCfg
{
    /// <summary>
    /// If fall height was bigger than that number, it will be capped at this number as far as trampling power is concerned. Set 0 to remove cap.
    /// </summary>
    public double CapHeight { get; set; } = 0.0d;

    /// <summary>
    /// If fall height was smaller than that number, it will be ignored as far as trampling power is concerned. Note effective height will be subtracted by this number!
    /// </summary>
    public double MinimumHeight { get; set; } = 1.5d;

    /// <summary>
    /// Basic multiplier of trampling power. 1 means trampling power will increase by 1 with every unit.
    /// Example: Mul = 2.0 and entity fell from height of 4.0 (or 4 blocks). After substracting MinimumHeight (default 1.5) it will be 2.5. Multiply by 2.0 to get 5.0.
    /// And this 5.0 will be additional trampling power of fall before multiplying by entity-specific FallMul.
    /// </summary>
    public double Mul = 1;
}
