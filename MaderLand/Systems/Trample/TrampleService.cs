using MaderLand.Config.Trample;
using MaderLand.Config.Utils;
using MaderLand.Systems.Trample.Data;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace MaderLand.Systems.Trample;

/// <summary>
/// Service for handling trample data. It provides methods to set, get and remove trample data for blocks, as well as load and save trample data associated with chunks.
/// </summary>
public class TrampleService
{
    /// <summary>
    /// Add trample data for a given block as is, without any modifications. Will not touch already existing trample data.
    /// </summary>
    /// <param name="api">Core server API.</param>
    /// <param name="block">Block data.</param>
    /// <param name="pos">Position of block.</param>
    /// <param name="passable">Is block passable?</param>
    public static void AddTrampleData(ICoreServerAPI api, Block block, BlockPos pos, bool passable)
    {
        TrampleBlockCfg? trampleBlockCfg = GetTrampleConfig(api, block, passable);
        if (trampleBlockCfg == null) return; // No trample config for this block, skip.

        IServerChunk chunk = api.WorldManager.GetChunk(pos);
        if (chunk == null) return; // No chunk found for this position, skip.

        ChunkTrampleData? chunkTrampleData = LoadChunkTrampleData(chunk, true);
        if (chunkTrampleData == null) return; // Failed to load or create trample data for this chunk, skip.

        // If there is already trample data for this block, we will ignore it.
        int localIndex = MapUtil.Index3d(pos.X & 31, pos.Y & 31, pos.Z & 31, 32, 32);
        if (!chunkTrampleData.Blocks.TryGetValue(localIndex, out BlockTrampleData? blockTrampleData))
        {
            // No existing trample data for this block, create new one and write to chunk data.
            blockTrampleData = CreateBlockTrampleData(trampleBlockCfg);
            chunkTrampleData.Blocks[localIndex] = blockTrampleData;
            SaveChunkTrampleData(chunk, chunkTrampleData);
        }
    }

    /// <summary>
    /// Get trample data for a specific block position. Returns null if no trample data exists for the block.
    /// </summary>
    /// <param name="api">Core server API.</param>
    /// <param name="pos">Position of block.</param>
    /// <returns>Block trample data or null if no trample data.</returns>
    public static BlockTrampleData? GetTrampleData(ICoreServerAPI api, BlockPos pos)
    {
        IServerChunk chunk = api.WorldManager.GetChunk(pos);
        if (chunk == null) return null; // No chunk found for this position, nothing to get.

        ChunkTrampleData? trampleData = LoadChunkTrampleData(chunk, false);
        if (trampleData == null) return null; // No trample data for this chunk, nothing to get.

        int localIndex = MapUtil.Index3d(pos.X & 31, pos.Y & 31, pos.Z & 31, 32, 32);
        if (!trampleData.Blocks.TryGetValue(localIndex, out BlockTrampleData? blockTrampleData)) return null; // No trample data for this block.
        return blockTrampleData;
    }

    /// <summary>
    /// Resolve trample data for given block. If no trample data exists for the block, it will create new trample data based on the block's configuration and add it to the chunk's trample data.
    /// </summary>
    /// <param name="trampleBlockCfg">Config data about current block.</param>
    /// <param name="chunkTrampleData">Trample data for given chunk.</param>
    /// <param name="pos">Position of block.</param>
    /// <returns>Trample data for given block.</returns>
    public static BlockTrampleData ResolveBlockTrampleData(TrampleBlockCfg trampleBlockCfg, ChunkTrampleData chunkTrampleData, BlockPos pos)
    {
        int localIndex = MapUtil.Index3d(pos.X & 31, pos.Y & 31, pos.Z & 31, 32, 32);
        if (!chunkTrampleData.Blocks.TryGetValue(localIndex, out BlockTrampleData? blockTrampleData))
        {
            // No existing trample data for this block, create new one and write to chunk data.
            blockTrampleData = CreateBlockTrampleData(trampleBlockCfg);
            chunkTrampleData.Blocks[localIndex] = blockTrampleData;
        }
        return blockTrampleData;
    }

    /// <summary>
    /// Create new trample data associated with given block.
    /// </summary>
    /// <param name="trampleBlockCfg">Config data about current block.</param>
    /// <returns>New trample data for given block.</returns>
    private static BlockTrampleData CreateBlockTrampleData(TrampleBlockCfg trampleBlockCfg)
    {
        BlockTrampleData blockTrampleData = new(trampleBlockCfg);
        return blockTrampleData;
    }

    /// <summary>
    /// Removes trample data for a specific block position.
    /// </summary>
    /// <param name="api">Core server API.</param>
    /// <param name="pos">Position of block.</param>
    public static void RemoveTrampleData(ICoreServerAPI api, BlockPos pos)
    {
        IServerChunk chunk = api.WorldManager.GetChunk(pos);
        if (chunk == null) return; // No chunk found for this position, nothing to cleanup.

        ChunkTrampleData? chunkTrampleData = LoadChunkTrampleData(chunk, false);
        if (chunkTrampleData == null) return; // No trample data for this chunk, nothing to cleanup.

        int localIndex = MapUtil.Index3d(pos.X & 31, pos.Y & 31, pos.Z & 31, 32, 32);
        if (chunkTrampleData.Blocks.Remove(localIndex)) SaveChunkTrampleData(chunk, chunkTrampleData);
    }

    // ////////////////////////////////////////////////////////////////////////
    // CONFIG

    /// <summary>
    /// Get trample configuration for given block.
    /// </summary>
    /// <param name="api">Core server API.</param>
    /// <param name="block">Block data.</param>
    /// <param name="passable">True if we check passable block, otherwise false.</param>
    /// <returns>Trample config data for given block or null if block cannot be trampled.</returns>
    public static TrampleBlockCfg? GetTrampleConfig(ICoreServerAPI api, Block block, bool passable)
    {
        // Air block will never be involved in anything. And since it is most common block in the world, we can save a lot of CPU time by just skipping it without trying to find config for it.
        if (passable && block.Code.ToString() == TrampleConst.emptyBlock) return null;

        if (passable) return GetTrampleConfigFromGroup(api, ConfigService.TrampleConfig.Passable, block);
        else return GetTrampleConfigFromGroup(api, ConfigService.TrampleConfig.Impassable, block);
    }

    /// <summary>
    /// Get trample configuration for given block based on given group.
    /// </summary>
    /// <param name="api">Core server API.</param>
    /// <param name="trampleGroupCfg">Trample group.</param>
    /// <param name="block">Block data.</param>
    /// <returns>Trample config data for given block or null if block cannot be trampled.</returns>
    private static TrampleBlockCfg? GetTrampleConfigFromGroup(ICoreServerAPI api, TrampleGroupCfg trampleGroupCfg, Block block)
    {
        string currBlockCode = block.Code.ToString();

        // First: try to find config by exact block code. This will change current block type to new, exact block type.
        trampleGroupCfg.Blocks.TryGetValue(currBlockCode, out TrampleBlockCfg? trampleBlockCfg);
        if (trampleBlockCfg != null) return trampleBlockCfg;

        // Second: try to find config by block code with wildcard. For example, "game:soil-*-normal" will match "game:soil-medium-normal", "game:soil-low-normal", etc.
        // Once found, we will calculate the new block code based on the wildcard. For example, "game:soil-*-normal" will become "game:soil-medium-sparse" if the original block was "game:soil-medium-normal".
        foreach (TrampleBlockVariantCfg variant in trampleGroupCfg.BlockVariants)
        {
            if (!WildcardUtil.Match(variant.FromBlockCode, currBlockCode)) continue;

            // If ToBlockCode is empty, we want to remove block. If ToBlockCode has no wildcard, we change block to exact block type instead of calculated block type.
            string resolvedToBlockCode = "";
            try
            {
                if (variant.ToBlockCode.Contains('*')) resolvedToBlockCode = block.Code.WildCardReplace(variant.FromBlockCode, variant.ToBlockCode);
            }
            catch
            {
                api.Logger.Error($"[Trample] Failed to resolve wildcard block code for currBlockCode='{currBlockCode}' with FromBlockCode='{variant.FromBlockCode}' and ToBlockCode='{variant.ToBlockCode}'. Make sure the wildcard is used correctly in config.");
                return null;
            }

            //string message = $"[Trample] Found match for wildcard FromBlockCode '{variant.FromBlockCode}' and wildcard ToBlockCode '{variant.ToBlockCode}' against '{currBlockCode}'. Calculated full ToBlockCode: '{resolvedTo}'.";
            //api.Logger.Notification(message); // DEBUG

            return new TrampleBlockVariantCfg
            {
                FromBlockCode = variant.FromBlockCode,
                ToBlockCode = resolvedToBlockCode,
                Durability = variant.Durability,
                Regen = variant.Regen
            };
        }

        return null; // Failed to find anything that can be trampled.
    }

    // ////////////////////////////////////////////////////////////////////////
    // LOAD/SAVE

    /// <summary>
    /// Load or create trample data associated with given chunk.
    /// </summary>
    /// <param name="chunk">Chunk.</param>
    /// <param name="create">If true and trample data is missing, create trample data.</param>
    /// <returns>Trample data or null if no trample data.</returns>
    public static ChunkTrampleData? LoadChunkTrampleData(IServerChunk chunk, bool create)
    {
        ChunkTrampleData? trampleData = null;
        // Try to get existing data, otherwise initialize a new one (if can).
        byte[] rawData = chunk.GetServerModdata(TrampleConst.trampleDataKey);

        if (rawData != null) trampleData = SerializerUtil.Deserialize<ChunkTrampleData>(rawData);
        else if (create) trampleData = new ChunkTrampleData();

        return trampleData;
    }

    /// <summary>
    /// Save trample data associated with given chunk.
    /// </summary>
    /// <param name="chunk">Chunk.</param>
    /// <param name="chunkTrampleData">Trample data for given chunk.</param>
    public static void SaveChunkTrampleData(IServerChunk chunk, ChunkTrampleData chunkTrampleData)
    {
        // Serialize and save back to chunk.
        byte[] serializedBytes = SerializerUtil.Serialize(chunkTrampleData);
        chunk.SetServerModdata(TrampleConst.trampleDataKey, serializedBytes);

        // Mark the chunk dirty so the save system knows to write it to disk.
        chunk.MarkModified();
    }
}
