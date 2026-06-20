namespace MaderLand.Systems.Trample.Config;

/// <summary>
/// Contains data about item that can contribute to trampling when entity wears it.
/// </summary>
public class TrampleItemCfg
{
    /// <summary>
    /// Item code. Can contain wildcards.
    /// </summary>
    public string Code { get; set; } = "";

    /// <summary>
    /// This value is summed with other worn items to get overall value to multiply base trampling power of entity.
    /// Example: player wears plate armor (2.25) and shoes (0.1). Add them together and you get 2.35 - our multiplier to trampling power.
    /// </summary>
    public float Power { get; set; } = 1;

    /// <summary>
    /// If true, condition of item decreases Power proportionally.
    /// </summary>
    public bool Damaged { get; set; } = false;
}
