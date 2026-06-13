using MaderLand.Config.Trample;
using MaderLand.Config.Utils;
using MaderLand.Systems.Trample.Data;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace MaderLand.Systems.Trample;

/// <summary>
/// Handles the trample feature logic, including player movement detection and block updates.
/// </summary>
public class TrampleSystem : ModSystem
{
    private ICoreServerAPI api = null!;

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
        api.Event.RegisterGameTickListener(ProcessTrampling, 100);
        api.Event.PlayerLeave += OnPlayerGone;
        api.Event.PlayerDisconnect += OnPlayerGone;

        AppendCleanupBehavior();
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
    private void AppendCleanupBehavior()
    {
        if (ConfigService.TrampleConfig == null)
        {
            api.Logger.Error("[Trample] TrampleConfig is null. Make sure to load the config before finalizing assets.");
            return;
        }

        AppendCleanupBehavior(ConfigService.TrampleConfig.Passable);
        AppendCleanupBehavior(ConfigService.TrampleConfig.Impassable);
    }

    /// <summary>
    /// Appends the TrampleCleanupBehavior to all blocks defined in the given trample group configuration.
    /// </summary>
    /// <param name="trampleGroupCfg">Trample group.</param>
    private void AppendCleanupBehavior(TrampleGroupCfg trampleGroupCfg)
    {
        if (trampleGroupCfg == null) return;

        if (trampleGroupCfg.Blocks != null && trampleGroupCfg.Blocks.Count > 0)
        {
            foreach (string blockCode in trampleGroupCfg.Blocks.Keys)
            {
                AppendCleanupBehavior(blockCode);
            }
        }

        if (trampleGroupCfg.BlockVariants != null && trampleGroupCfg.BlockVariants.Count > 0)
        {
            foreach (TrampleBlockVariantCfg variant in trampleGroupCfg.BlockVariants)
            {
                Block[] allVariants = api.World.SearchBlocks(variant.FromBlockCode);
                foreach (Block block in allVariants)
                {
                    AppendCleanupBehavior(block.Code);
                }
            }
        }
    }

    /// <summary>
    /// Add cleanup behavior to blocks that can be trampled.
    /// </summary>
    /// <param name="blockCode">Block code.</param>
    private void AppendCleanupBehavior(AssetLocation blockCode)
    {
        Block? block = api.World.GetBlock(blockCode);
        if (block == null)
        {
            api.Logger.Error($"[Trample] AppendCleanupBehavior(): Could not find block class for '{blockCode}'.");
            return;
        }
        block.BlockBehaviors = block.BlockBehaviors.Append(new TrampleHandleBehavior(this, block));
    }

    // ////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Process player movement and perform trampling logic if they walked onto a new block.
    /// </summary>
    /// <param name="dt">Delta time.</param>
    private void ProcessTrampling(float dt)
    {
        // Only run detection if the trample feature is active or allowed in the configuration
        if (api == null || !ConfigService.TrampleConfig.Active || !ConfigService.TrampleConfig.Allowed) return;

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
    /// Check if we can use player for trample logic.
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
        TrampleLogic(player, posPlayer);
    }

    //

    /// <summary>
    /// Perform block trampling logic.
    /// </summary>
    /// <param name="player">Server player.</param>
    /// <param name="posPlayer">Position of that player.</param>
    private void TrampleLogic(IServerPlayer player, BlockPos posPlayer)
    {
        Block blockPlayer = api.World.BlockAccessor.GetBlock(posPlayer);
        BlockPos posUnder = posPlayer.DownCopy();
        Block blockUnder = api.World.BlockAccessor.GetBlock(posUnder);

        string message = $"[Trample] Player {player.PlayerName} walked to new block: {blockPlayer.Code} at {posPlayer}. Block under: {blockUnder.Code}.";
        api.Logger.Notification(message); // DEBUG

        // We can trample grass and similar stuff that can be walked through by players.
        bool result = TryTrampleBlock(blockPlayer, posPlayer, true);
        // If result is ok (like grass fully trampled), try to trample block under player.
        if (result) TryTrampleBlock(blockUnder, posUnder, false);
    }

    /// <summary>
    /// Try to trample given block.
    /// Note: returns false when fails to trample something that needs to be trampled. For example, you need to trample grass fully before trampling block under that grass.
    /// </summary>
    /// <param name="block">Block data.</param>
    /// <param name="pos">Position of block.</param>
    /// <param name="passable">Is block passable?</param>
    /// <returns>True if we can check block underneath next. Matters only for passable blocks.</returns>
    private bool TryTrampleBlock(Block block, BlockPos pos, bool passable)
    {
        TrampleBlockCfg? trampleBlockCfg = GetTrampleConfig(block, passable);
        if (trampleBlockCfg == null) return true;

        IServerChunk chunk = api.WorldManager.GetChunk(pos);
        if (chunk == null) return true;

        ChunkTrampleData? chunkTrampleData = LoadChunkTrampleData(chunk, true);
        if (chunkTrampleData == null) return true;

        BlockTrampleData blockTrampleData = ResolveBlockTrampleData(trampleBlockCfg, chunkTrampleData, pos);
        bool replaced = TrampleBlock(trampleBlockCfg, blockTrampleData, block, pos);

        // Only save if the block wasn't replaced. 
        // If it WAS replaced, the TrampleCleanupBehavior already handled cleanup and saving.
        if (!replaced)
        {
            SaveChunkTrampleData(chunk, chunkTrampleData);
        }
        return false; // Block was trampled, we skip checking block underneath.
    }

    /// <summary>
    /// Get trample configuration for given block.
    /// </summary>
    /// <param name="block">Block data.</param>
    /// <param name="passable">True if we check passable block, otherwise false.</param>
    /// <returns>Trample config data for given block or null if block cannot be trampled.</returns>
    private TrampleBlockCfg? GetTrampleConfig(Block block, bool passable)
    {
        // Air block will never be involved in anything. And since it is most common block in the world, we can save a lot of CPU time by just skipping it without trying to find config for it.
        if (passable && block.Code.ToString() == TrampleConst.emptyBlock) return null;

        if (passable) return GetTrampleConfigFromGroup(ConfigService.TrampleConfig.Passable, block);
        else return GetTrampleConfigFromGroup(ConfigService.TrampleConfig.Impassable, block);
    }

    /// <summary>
    /// Get trample configuration for given block based on given group.
    /// </summary>
    /// <param name="trampleGroupCfg">Trample group.</param>
    /// <param name="block">Block data.</param>
    /// <returns>Trample config data for given block or null if block cannot be trampled.</returns>
    private TrampleBlockCfg? GetTrampleConfigFromGroup(TrampleGroupCfg trampleGroupCfg, Block block)
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
                Durability = variant.Durability
            };
        }

        return null; // Failed to find anything that can be trampled.
    }

    //

    /// <summary>
    /// Actually trample block.
    /// </summary>
    /// <param name="trampleBlockCfg">Trample configuration for this block.</param>
    /// <param name="blockTrampleData">Trample data for given block.</param>
    /// <param name="block">Block data.</param>
    /// <param name="pos">Position of block.</param>
    /// <returns>True if block was replaced, otherwise false.</returns>
    private bool TrampleBlock(TrampleBlockCfg trampleBlockCfg, BlockTrampleData blockTrampleData, Block block, BlockPos pos)
    {
        // Trample this block.
        blockTrampleData.Durability -= ResolveTramplePower();

        if (blockTrampleData.Durability <= 0f)
        {
            Block? nextBlock = ResolveNextBlock(trampleBlockCfg, block, pos);
            if (nextBlock == null) return false;

            // Replace current block with next block.
            api.World.BlockAccessor.SetBlock(nextBlock.BlockId, pos);
            api.World.BlockAccessor.MarkBlockDirty(pos);

            // We don't need to cleanup as it is now handled by the TrampleCleanupBehavior attached to the block.

            string message1 = $"[Trample] Block '{block.Code}' at {pos} was fully trampled and replaced with '{nextBlock.Code}'.";
            api.Logger.Notification(message1); // DEBUG
            return true;
        }
        string message = $"[Trample] Block '{block.Code}' at {pos} was trampled. Durability: {blockTrampleData.Durability}.";
        api.Logger.Notification(message); // DEBUG

        return false;
    }

    /// <summary>
    /// Resolve trample power.
    /// </summary>
    /// <returns>Trample power.</returns>
    private static float ResolveTramplePower()
    {
        TramplePowerCfg TramplePower = ConfigService.TrampleConfig.Power;
        // TODO actually resolve trampling power of player, for now just assume everyone is barefoot and has same trample power.
        return TramplePower.PlayerBarefoot;
    }

    //

    /// <summary>
    /// Resolve next block after trampling. For example, trampled grass becomes dirt.
    /// </summary>
    /// <param name="trampleBlockCfg">Config data about current block.</param>
    /// <param name="block">Block data.</param>
    /// <param name="pos">Position of block.</param>
    /// <returns>Next block to use or null if failed to find next block.</returns>
    private Block? ResolveNextBlock(TrampleBlockCfg trampleBlockCfg, Block block, BlockPos pos)
    {
        string toBlockCode = trampleBlockCfg.ToBlockCode;
        if (toBlockCode == "") toBlockCode = TrampleConst.emptyBlock;

        AssetLocation loc = new(toBlockCode);
        Block? nextBlock = api.World.GetBlock(loc);
        if (nextBlock == null)
            api.Logger.Error($"[Trample] Failed to get block with code '{toBlockCode}' when trying to trample block '{block.Code}' at {pos}. Make sure the block code in config is correct.");

        return nextBlock;
    }

    /// <summary>
    /// Removes trample data for a specific block position.
    /// </summary>
    public void RemoveTrampleData(BlockPos pos)
    {
        IServerChunk chunk = api.WorldManager.GetChunk(pos);
        if (chunk == null) return; // No chunk found for this position, nothing to cleanup.

        ChunkTrampleData? trampleData = LoadChunkTrampleData(chunk, false);
        if (trampleData == null) return; // No trample data for this chunk, nothing to cleanup.

        int localIndex = MapUtil.Index3d(pos.X & 31, pos.Y & 31, pos.Z & 31, 32, 32);
        if (trampleData.Blocks.Remove(localIndex)) SaveChunkTrampleData(chunk, trampleData);
    }

    // ////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Load or create trample data associated with given chunk.
    /// </summary>
    /// <param name="chunk">Chunk.</param>
    /// <param name="create">If true and trample data is missing, create trample data.</param>
    /// <returns>Trample data or null if no trample data.</returns>
    private ChunkTrampleData? LoadChunkTrampleData(IServerChunk chunk, bool create)
    {
        ChunkTrampleData? trampleData = null;
        // Try to get existing data, otherwise initialize a new one (if can).
        byte[] rawData = chunk.GetServerModdata(TrampleConst.trampleDataKey);

        if (rawData != null) trampleData = SerializerUtil.Deserialize<ChunkTrampleData>(rawData);
        else if (create) trampleData = new ChunkTrampleData();

        return trampleData;
    }

    /// <summary>
    /// Resolve trample data for given block.
    /// </summary>
    /// <param name="trampleBlockCfg">Config data about current block.</param>
    /// <param name="chunkTrampleData">Trample data for given chunk.</param>
    /// <param name="pos">Position of block.</param>
    /// <returns>Trample data for given block.</returns>
    private static BlockTrampleData ResolveBlockTrampleData(TrampleBlockCfg trampleBlockCfg, ChunkTrampleData chunkTrampleData, BlockPos pos)
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
        BlockTrampleData blockTrampleData = new();
        blockTrampleData.Durability = trampleBlockCfg.Durability;
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
        chunk.SetServerModdata(TrampleConst.trampleDataKey, serializedBytes);

        // Mark the chunk dirty so the save system knows to write it to disk.
        chunk.MarkModified();
    }
}
