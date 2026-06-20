namespace MaderLand.Systems.Trample.Config;

/// <summary>
/// Contains data about entity that can trample.
/// </summary>
public class TrampleEntityCfg
{
    /// <summary>
    /// Entity code. Can contain wildcards.
    /// </summary>
    public string Code { get; set; } = "";

    /// <summary>
    /// Base value of trampling power - how much Durability will be substracted from trampleable block during single trample event.
    /// </summary>
    public float Power { get; set; } = 1;

    /// <summary>
    /// Fall multiplier. If no trampling should occur when entity falls/jumps into ground, set 0. Result is added to base Power.
    /// </summary>
    public float FallMul { get; set; } = 1;
}
