using ProtoBuf;

namespace MaderLand.Systems.Trample.Data;

/// <summary>
/// Trample data for single block.
/// </summary>
[ProtoContract]
public class BlockTrampleData
{
    /// <summary>
    /// Current durability of block. Reduced by trampling power. When it reaches 0, block should be fully trampled (replaced with different block).
    /// </summary>
    [ProtoMember(1)]
    public float Durability = 666f;

    /// <summary>
    /// Regeneration rate of block.
    /// </summary>
    [ProtoMember(2)]
    public float Regen = 666f;
}
