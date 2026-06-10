using ProtoBuf;

namespace MaderLand.Systems.Trails.Data;

/// <summary>
/// Trample data for single block.
/// </summary>
[ProtoContract]
public class BlockTrampleData
{
    // Current durability of block.
    [ProtoMember(1)]
    public float Durability = 100f;
}
