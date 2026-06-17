using ProtoBuf;
using Vintagestory.API.MathTools;

namespace MaderLand.Systems.Trample.Network;

/// <summary>
/// Client request for trample data.
/// </summary>
[ProtoContract]
public class TrampleDataReq
{
    /// <summary>
    /// Position of block that we want to know about.
    /// </summary>
    [ProtoMember(1)]
    public BlockPos? Pos { get; set; }
}
