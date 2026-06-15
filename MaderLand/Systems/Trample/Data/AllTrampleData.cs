using Vintagestory.API.Server;

namespace MaderLand.Systems.Trample.Data;

public class AllTrampleData
{
    public IServerChunk chunk = null!;
    public ChunkTrampleData chunkData = null!;
    public BlockTrampleData blockData = null!;

    public bool IsValid()
    {
        return chunk != null && chunkData != null && blockData != null;
    }
}
