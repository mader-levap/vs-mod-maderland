using MaderLand.Config.Trample;
using MaderLand.Config.Utils;
using MaderLand.Systems.Trample.Behaviors;
using MaderLand.Systems.Trample.Data;
using System.Collections.Generic;
using System.Threading;
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
    /// Returns if the mod system should be loaded on given side.
    /// </summary>
    /// <param name="side">The side of the application.</param>
    /// <returns>True if it should load, false otherwise.</returns>
    public override bool ShouldLoad(EnumAppSide side)
    {
        // Trample feature is server-side mod.
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

        TrampleInitializer.AppendHandleBehavior(api);

        // Register game tick listener running every 100ms.
        api.Event.RegisterGameTickListener(ProcessTrampling, 100);
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

    // ////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Process player movement and perform trampling logic if they walked onto a new block.
    /// </summary>
    /// <param name="dt">Delta time.</param>
    private void ProcessTrampling(float dt)
    {
        // Only run detection if the trample feature is active or allowed in the configuration
        if (api == null || !ConfigService.TrampleConfig.Active || !ConfigService.TrampleConfig.Allow) return;

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
        TrampleBlockCfg? trampleBlockCfg = TrampleService.GetTrampleConfig(api, block, passable);
        if (trampleBlockCfg == null) return true; // No trample config for this block, skip.

        IServerChunk chunk = api.WorldManager.GetChunk(pos);
        if (chunk == null) return true; // No chunk found for this position, skip.

        ChunkTrampleData? chunkTrampleData = TrampleService.LoadChunkTrampleData(chunk, true);
        if (chunkTrampleData == null) return true; // Failed to load or create trample data for this chunk, skip.

        BlockTrampleData blockTrampleData = TrampleService.ResolveBlockTrampleData(trampleBlockCfg, chunkTrampleData, pos);
        Block? replacementBlock = TrampleBlock(trampleBlockCfg, blockTrampleData, block, pos);

        if (replacementBlock != null) RefreshTrampleData(chunk, chunkTrampleData, blockTrampleData, replacementBlock, pos, passable);

        TrampleService.SaveChunkTrampleData(chunk, chunkTrampleData);

        return false; // Block was trampled, we skip checking block underneath.
    }

    //

    /// <summary>
    /// Actually trample block.
    /// </summary>
    /// <param name="trampleBlockCfg">Trample configuration for this block.</param>
    /// <param name="blockTrampleData">Trample data for given block.</param>
    /// <param name="block">Block data.</param>
    /// <param name="pos">Position of block.</param>
    /// <returns>Replacement block or null if block was not replaced.</returns>
    private Block? TrampleBlock(TrampleBlockCfg trampleBlockCfg, BlockTrampleData blockTrampleData, Block block, BlockPos pos)
    {
        // Trample this block.
        float tramplePower = ResolveTramplePower();
        blockTrampleData.Durability -= tramplePower;

        string message = $"[Trample] Block '{block.Code}' at {pos} was trampled. Durability: {blockTrampleData.Durability}. Power: {tramplePower}.";
        api.Logger.Notification(message); // DEBUG

        if (blockTrampleData.Durability <= 0f)
        {
            blockTrampleData.Durability = 0f;
            if (block.Code == trampleBlockCfg.ToBlockCode) return null; // Same block as next block.

            Block? nextBlock = ResolveNextBlock(trampleBlockCfg, block, pos);
            if (nextBlock == null) return null; // Unknown next block, skip.

            // Replace current block with next block.
            api.World.BlockAccessor.ExchangeBlock(nextBlock.BlockId, pos); // Will NOT trigger TrampleBehavior.OnBlockRemoved().
            api.World.BlockAccessor.MarkBlockDirty(pos);

            // We will handle updating trample data for this block in RefreshTrampleData() after this function returns.

            string message1 = $"[Trample] Block '{block.Code}' at {pos} was fully trampled and replaced with '{nextBlock.Code}'.";
            api.Logger.Notification(message1); // DEBUG
            return nextBlock;
        }

        return null;
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

    /// <summary>
    /// // Block was replaced with another block, we need to refresh trample data.
    /// </summary>
    /// <param name="chunk">Chunk.</param>
    /// <param name="chunkTrampleData">Trample data for chunk.</param>
    /// <param name="blockTrampleData">Trample data for block.</param>
    /// <param name="replacementBlock">Block that replaced previous block.</param>
    /// <param name="pos">Position of block.</param>
    private void RefreshTrampleData(IServerChunk chunk, ChunkTrampleData chunkTrampleData, BlockTrampleData blockTrampleData, Block replacementBlock, BlockPos pos, bool passable)
    {
        TrampleBlockCfg? replacementTrampleBlockCfg = TrampleService.GetTrampleConfig(api, replacementBlock, passable);
        if (replacementTrampleBlockCfg == null)
        { // No trample config for this block, remove trample data completely.
            int localIndex = MapUtil.Index3d(pos.X & 31, pos.Y & 31, pos.Z & 31, 32, 32);
            chunkTrampleData.Blocks.Remove(localIndex);

            string message1 = $"[Trample] Replacement block '{replacementBlock.Code}' cannot be trampled. Removing trample data.";
            api.Logger.Notification(message1); // DEBUG
            return;
        }

        string message = $"[Trample] Replacement block '{replacementBlock.Code}' can be trampled. Resetting trample data.";
        api.Logger.Notification(message); // DEBUG

        blockTrampleData.Reset(replacementTrampleBlockCfg);
    }

    //

    /// <summary>
    /// Resolve next block after full trampling. For example, trampled grass becomes soil.
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
}
