using ProtoBuf;
using System.Collections.Generic;

namespace MaderLand.Systems.Trample.Data;

/// <summary>
/// Trample data for single chunk.
/// </summary>
[ProtoContract]
public class ChunkTrampleData
{
    // Maps a local block index in the chunk (0 to 32767) to instance of BlockTrampleData.
    [ProtoMember(1)]
    public Dictionary<int, BlockTrampleData> Blocks { get; set; } = [];
}
