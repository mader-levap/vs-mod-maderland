using ProtoBuf;

namespace MaderLand.Systems.Trample.Network;

/// <summary>
/// For showing debug window when debug mode is turned on on server.
/// </summary>
[ProtoContract]
public class TrampleDebugPacket
{
    [ProtoMember(1)]
    public bool Enabled { get; set; }
}
