using MaderLand.Systems.Trample.Data;
using ProtoBuf;
using Vintagestory.API.MathTools;

namespace MaderLand.Systems.Trample.Network;

/// <summary>
/// Server response with trample data.
/// </summary>
[ProtoContract]
public class TrampleDataResp
{
    /// <summary>
    /// Position of block that we want to know about.
    /// </summary>
    [ProtoMember(1)]
    public BlockPos? Pos { get; set; }

    /// <summary>
    /// Block trample data. If null, no data for specified position.
    /// </summary>
    [ProtoMember(2)]
    public BlockTrampleData? Data { get; set; }
}
