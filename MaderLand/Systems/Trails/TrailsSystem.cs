using MaderLand.Config.Trails;
using MaderLand.Config.Utils;
using MaderLand.Systems.Trails.Data;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace MaderLand.Systems.Trails;

/// <summary>
/// Handles the trails feature logic, including player movement detection and block updates.
/// </summary>
public class TrailsSystem : ModSystem
{
    private ICoreServerAPI? api;

    /// <summary>
    /// Map of all player positions. Key is PlayerUID, Value is block position.
    /// </summary>
    private readonly Dictionary<string, BlockPos> lastPlayerPositions = [];

    /// <summary>
    /// Returns if the mod system should be loaded on the server.
    /// </summary>
    /// <param name="side">The side of the application.</param>
    /// <returns>True if it should load, false otherwise.</returns>
    public override bool ShouldLoad(EnumAppSide side)
    {
        return side == EnumAppSide.Server;
    }

    /// <summary>
    /// Starts the mod system on the server side.
    /// </summary>
    /// <param name="api">Core server API.</param>
    public override void StartServerSide(ICoreServerAPI serverApi)
    {
        base.StartServerSide(serverApi);
        api = serverApi;

        // Register game tick listener running every 100ms.
        api.Event.RegisterGameTickListener(OnGameTick, 100);
        api.Event.PlayerLeave += OnPlayerGone;
        api.Event.PlayerDisconnect += OnPlayerGone;
    }

    /// <summary>
    /// Cleans up dictionary entry when a player leaves/disconnects to prevent memory leaks.
    /// </summary>
    /// <param name="player">The player leaving the server.</param>
    private void OnPlayerGone(IServerPlayer player)
    {
        lastPlayerPositions.Remove(player.PlayerUID);
    }

    //

    /// <summary>
    /// Registers a cleanup behavior to all blocks that can be trampled.
    /// This ensures trample data is removed if the block is broken, replaced, or eaten by animals.
    /// </summary>
    public override void AssetsFinalize(ICoreAPI api)
    {
        base.AssetsFinalize(api);
        if (ConfigService.TrailsConfig?.Blocks == null) return;

        string message = $"[Trails] AssetsFinalize: will add behavior to handled block types.";
        api.Logger.Notification(message); // DEBUG

        foreach (var blockCode in ConfigService.TrailsConfig.Blocks.Keys)
        {
            // Add cleanup behavior to blocks that can be trampled.
            AssetLocation loc = new(blockCode);
            Block? block = api.World.GetBlock(loc);
            block?.BlockBehaviors = block.BlockBehaviors.Append(new TrampleCleanupBehavior(this, block));
        }
    }

    // ////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Triggered every 100ms on the server to update and detect player position changes.
    /// </summary>
    /// <param name="dt">Delta time.</param>
    private void OnGameTick(float dt)
    {
        // Only run detection if the trails feature is active in the configuration
        if (api == null || !ConfigService.TrailsConfig.Active) return;

        // Go over all players...
        foreach (IPlayer player in api.World.AllOnlinePlayers)
        {
            CheckPlayerMove((IServerPlayer)player);
        }
    }

    /// <summary>
    /// Check given player for walking onto new block.
    /// </summary>
    /// <param name="player">Player to check.</param>
    private void CheckPlayerMove(IServerPlayer player)
    {
        if (player.Entity is not EntityPlayer entityPlayer) return;
        if (!CanUsePlayer(player)) return;

        BlockPos currentPos = entityPlayer.Pos.AsBlockPos;

        if (lastPlayerPositions.TryGetValue(player.PlayerUID, out BlockPos? lastPos))
        {
            // If they have moved to a different block position.
            if (!currentPos.Equals(lastPos))
            {
                lastPlayerPositions[player.PlayerUID] = currentPos;
                OnPlayerWalkedOntoNewBlock(player, currentPos);
            }
        }
        else
        {
            // First time detecting player position.
            lastPlayerPositions[player.PlayerUID] = currentPos;
        }
    }

    /// <summary>
    /// Check if we can use player for trails logic.
    /// We only want to detect players in survival mode who are on the ground.
    /// </summary>
    /// <param name="player">The player to check.</param>
    /// <returns>True if the player can trample blocks.</returns>
    private static bool CanUsePlayer(IServerPlayer player)
    {
        if (player.ConnectionState != EnumClientState.Playing) return false;
        if (player.WorldData.CurrentGameMode != EnumGameMode.Survival) return false;

        if (!player.Entity.OnGround) return false;
        return true;
    }

    /// <summary>
    /// Triggered when a player moves onto a new block position.
    /// </summary>
    /// <param name="player">The player who moved.</param>
    /// <param name="posPlayer">The block position they walked onto.</param>
    private void OnPlayerWalkedOntoNewBlock(IServerPlayer player, BlockPos posPlayer)
    {
        Block blockPlayer = api.World.BlockAccessor.GetBlock(posPlayer);
        BlockPos posUnder = posPlayer.DownCopy();
        Block blockUnder = api.World.BlockAccessor.GetBlock(posUnder);

        string message = $"[Trails] Player {player.PlayerName} walked to new block: {blockPlayer.Code} at {posPlayer}. Block under: {blockUnder.Code}";
        api.Logger.Notification(message); // DEBUG

        // We can trample grass and similar stuff that can be walked through by players.
        bool result = TryTrampleBlock(blockPlayer, posPlayer);
        // If result is ok (like grass fully trampled), try to trample block under player.
        if (result) TryTrampleBlock(blockUnder, posUnder);
    }

    //

    /// <summary>
    /// Perform block trampling logic.
    /// Note: returns false when fails to trample something that needs to be trampled. For example, you need to trample grass fully before trampling block under that grass.
    /// </summary>
    /// <param name="block">Block data.</param>
    /// <param name="pos">Position of block.</param>
    /// <returns>True if no trample needed, otherwise false.</returns>
    private bool TryTrampleBlock(Block block, BlockPos pos)
    {
        TrailsBlockCfg? trailsBlockCfg = CanTrampleOnBlock(block);
        if (trailsBlockCfg == null) return true;

        IServerChunk chunk = api.WorldManager.GetChunk(pos);
        if (chunk == null) return true;

        ChunkTrampleData chunkTrampleData = LoadChunkTrampleData(chunk);
        BlockTrampleData blockTrampleData = ResolveBlockTrampleData(trailsBlockCfg, chunkTrampleData, pos);

        TrampleBlock(trailsBlockCfg, blockTrampleData, block, pos);

        SaveChunkTrampleData(chunk, chunkTrampleData);
        return true;
    }

    /// <summary>
    /// Check if given block can be trampled.
    /// </summary>
    /// <param name="block">Block data.</param>
    /// <returns>Trails config data for given block or null if block cannot be trampled.</returns>
    private static TrailsBlockCfg? CanTrampleOnBlock(Block block)
    {
        ConfigService.TrailsConfig.Blocks.TryGetValue(block.Code.ToString(), out TrailsBlockCfg? trailsBlockCfg);
        return trailsBlockCfg;
    }

    /// <summary>
    /// Actually trample block.
    /// </summary>
    /// <param name="trailsBlockCfg">Trample configuration for this block.</param>
    /// <param name="blockTrampleData">Trample data for given block.</param>
    /// <param name="block">Block data.</param>
    /// <param name="pos">Position of block.</param>
    private void TrampleBlock(TrailsBlockCfg trailsBlockCfg, BlockTrampleData blockTrampleData, Block block, BlockPos pos)
    {
        // Trample this block.
        blockTrampleData.Durability -= ResolveTramplePower();

        string message = $"[Trails] Block '{block.Code}' at {pos} was trampled. Durability: {blockTrampleData.Durability}.";
        api.Logger.Notification(message); // DEBUG

        if (blockTrampleData.Durability <= 0f)
        {
            Block? nextBlock = ResolveNextBlock(trailsBlockCfg, block, pos);
            if (nextBlock == null) return;

            // Replace current block with next block.
            api.World.BlockAccessor.SetBlock(nextBlock.BlockId, pos);
            api.World.BlockAccessor.MarkBlockDirty(pos);

            // We don't need to cleanup as it is now handled by the TrampleCleanupBehavior attached to the block.

            // Cleanup: remove trample data for this block since it's now a different block.
            //int localIndex = MapUtil.Index3d(pos.X & 31, pos.Y & 31, pos.Z & 31, 32, 32);
            //chunkTrampleData.Blocks.Remove(localIndex);

            string message1 = $"[Trails] Block '{block.Code}' at {pos} become fully trampled and replaced with '{nextBlock.Code}'.";
            api.Logger.Notification(message1); // DEBUG
        }
    }

    /// <summary>
    /// Removes trample data for a specific block position.
    /// </summary>
    public void RemoveTrampleData(BlockPos pos)
    {
        IServerChunk chunk = api.WorldManager.GetChunk(pos);
        if (chunk == null) return;

        ChunkTrampleData trampleData = LoadChunkTrampleData(chunk);
        int localIndex = MapUtil.Index3d(pos.X & 31, pos.Y & 31, pos.Z & 31, 32, 32);

        if (trampleData.Blocks.Remove(localIndex))
        {
            SaveChunkTrampleData(chunk, trampleData);
            api.Logger.Notification($"[Trails] Cleared trample data at {pos} due to block removal or change."); // DEBUG
        }
    }

    /// <summary>
    /// Resolve trample power.
    /// </summary>
    /// <returns>Trample power.</returns>
    private static float ResolveTramplePower()
    {
        TrailsTrampleCfg Trample = ConfigService.TrailsConfig.Trample;
        // TODO actually resolve trampling power of player, for now just assume everyone is barefoot and has same trample power.
        return Trample.PlayerBarefoot;
    }

    //

    /// <summary>
    /// Resolve next block after trampling. For example, trampled grass becomes dirt.
    /// </summary>
    /// <param name="trailsBlockCfg">Config data about current block.</param>
    /// <param name="block">Block data.</param>
    /// <param name="pos">Position of block.</param>
    /// <returns>Next block to use or null if failed to find next block.</returns>
    private Block? ResolveNextBlock(TrailsBlockCfg trailsBlockCfg, Block block, BlockPos pos)
    {
        string toBlockCode = trailsBlockCfg.ToBlockCode;
        if (toBlockCode == "") toBlockCode = TrailsConst.emptyBlock;

        AssetLocation loc = new(toBlockCode);
        Block? nextBlock = api.World.GetBlock(loc);
        if (nextBlock == null)
            api.Logger.Error($"[Trails] Failed to get block with code '{toBlockCode}' when trying to trample block '{block.Code}' at {pos}. Make sure the block code in config is correct.");

        return nextBlock;
    }

    // ////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Load or create trample data associated with given chunk.
    /// </summary>
    /// <param name="chunk">Chunk.</param>
    /// <returns>Trample data.</returns>
    private static ChunkTrampleData LoadChunkTrampleData(IServerChunk chunk)
    {
        ChunkTrampleData trampleData;
        // Try to get existing data, otherwise initialize a new one.
        byte[] rawData = chunk.GetServerModdata(TrailsConst.trampleDataKey);
        if (rawData != null)
        {
            trampleData = SerializerUtil.Deserialize<ChunkTrampleData>(rawData);
        }
        else
        {
            trampleData = new ChunkTrampleData();
        }
        return trampleData;
    }

    /// <summary>
    /// Resolve trample data for given block.
    /// </summary>
    /// <param name="trailsBlockCfg">Config data about current block.</param>
    /// <param name="chunkTrampleData">Trample data for given chunk.</param>
    /// <param name="pos">Position of block.</param>
    /// <returns>Trample data for given block.</returns>
    private static BlockTrampleData ResolveBlockTrampleData(TrailsBlockCfg trailsBlockCfg, ChunkTrampleData chunkTrampleData, BlockPos pos)
    {
        int localIndex = MapUtil.Index3d(pos.X & 31, pos.Y & 31, pos.Z & 31, 32, 32);
        if (!chunkTrampleData.Blocks.TryGetValue(localIndex, out BlockTrampleData? blockTrampleData))
        {
            // No existing trample data for this block, create new one and save to chunk data.
            blockTrampleData = CreateBlockTrampleData(trailsBlockCfg);
            chunkTrampleData.Blocks[localIndex] = blockTrampleData;
        }
        return blockTrampleData;
    }

    /// <summary>
    /// Create new trample data associated with given block.
    /// </summary>
    /// <param name="trailsBlockCfg">Config data about current block.</param>
    /// <returns>New trample data for given block.</returns>
    private static BlockTrampleData CreateBlockTrampleData(TrailsBlockCfg trailsBlockCfg)
    {
        BlockTrampleData blockTrampleData = new();
        blockTrampleData.Durability = trailsBlockCfg.Durability;
        return blockTrampleData;
    }

    /// <summary>
    /// Save trample data associated with given chunk.
    /// </summary>
    /// <param name="chunk">Chunk.</param>
    /// <param name="chunkTrampleData">Trample data for given chunk.</param>
    private static void SaveChunkTrampleData(IServerChunk chunk, ChunkTrampleData chunkTrampleData)
    {
        // Serialize and save back to chunk.
        byte[] serializedBytes = SerializerUtil.Serialize(chunkTrampleData);
        chunk.SetServerModdata(TrailsConst.trampleDataKey, serializedBytes);

        // Mark the chunk dirty so the save system knows to write it to disk.
        chunk.MarkModified();
    }
}
