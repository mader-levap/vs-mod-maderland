using ProtoBuf;

namespace MaderLand.Systems.Trample.Data;

/// <summary>
/// Trample data for single block.
/// </summary>
[ProtoContract]
public class BlockTrampleData
{
    // Current durability of block.
    [ProtoMember(1)]
    public float Durability = 666f; // deliberately weird default value to easily spot uninitialized data in case of bugs
}
