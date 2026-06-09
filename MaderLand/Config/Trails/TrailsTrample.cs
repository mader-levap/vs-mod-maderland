namespace MaderLand.Config.Trails;

/// <summary>
/// Contains trampling data.
/// </summary>
public class TrailsTrample
{
    /// <summary>
    /// How powerful is walking barefoot. Note: completely destroyed shoes/boots count as barefoot.
    /// </summary>
    public float PlayerBarefoot { get; set; } = 1;

    /// <summary>
    /// How powerful is walking using shoes.
    /// </summary>
    public float PlayerShoes { get; set; } = 2;

    /// <summary>
    /// How powerful is walking using armored boots.
    /// </summary>
    public float PlayerArmored { get; set; } = 3;
}
