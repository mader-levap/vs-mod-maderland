using MaderLand.Systems.Trample.Config;
using MaderLand.Systems.Trample.Data;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace MaderLand.Systems.Trample.Services;

/// <summary>
/// Main trample service class. It is executed on server.
/// </summary>
public class TrampleService
{
    readonly ICoreServerAPI sapi;
    readonly TrampleItem trampleItem;

    public TrampleService(ICoreServerAPI api)
    {
        sapi = api;
        trampleItem = new();
    }

    /// <summary>
    /// Perform block trampling logic.
    /// </summary>
    /// <param name="entry">Trampleable entity data.</param>
    public void TrampleLogic(EntityTrampleEntry entry)
    {
        Block block = sapi.World.BlockAccessor.GetBlock(entry.LastPos);
        BlockPos posUnder = entry.LastPos.DownCopy();
        Block blockUnder = sapi.World.BlockAccessor.GetBlock(posUnder);

        // We can trample grass and similar stuff that can be walked through by players.
        bool result = TryTrampleBlock(entry, block, entry.LastPos, true);
        // If result is ok (like grass fully trampled), try to trample block under player.
        if (result) TryTrampleBlock(entry, blockUnder, posUnder, false);
    }

    /// <summary>
    /// Try to trample given block.
    /// Note: returns false when fails to trample something that needs to be trampled. For example, you need to trample grass fully before trampling block under that grass.
    /// </summary>
    /// <param name="entry">Trampleable entity data.</param>
    /// <param name="block">Block data.</param>
    /// <param name="pos">Position of block.</param>
    /// <param name="passable">Is block passable?</param>
    /// <returns>True if we can check block underneath next. Matters only for passable blocks.</returns>
    private bool TryTrampleBlock(EntityTrampleEntry entry, Block block, BlockPos pos, bool passable)
    {
        TrampleBlockCfg? trampleBlockCfg = TrampleUtils.GetBlockConfig(sapi, block, passable);
        if (trampleBlockCfg == null) return true; // No trample config for this block, skip.

        AllTrampleData allTrampleData = TrampleUtils.ResolveBlockTrampleData(sapi, trampleBlockCfg, pos);
        if (!allTrampleData.IsValid()) return true;

        TrampleUtils.DeltaTrampleData(sapi, allTrampleData.blockData);
        Block? replacementBlock = TrampleBlock(entry, trampleBlockCfg, allTrampleData.blockData, block, pos);

        if (replacementBlock != null) RefreshTrampleData(allTrampleData, replacementBlock, pos, passable);

        TrampleUtils.SaveChunkTrampleData(allTrampleData.chunk, allTrampleData.chunkData);
        return false; // Block was trampled, we skip checking block underneath.
    }

    //

    /// <summary>
    /// Actually trample block.
    /// </summary>
    /// <param name="entry">Trampleable entity data.</param>
    /// <param name="trampleBlockCfg">Trample configuration for this block.</param>
    /// <param name="blockTrampleData">Trample data for given block.</param>
    /// <param name="block">Block data.</param>
    /// <param name="pos">Position of block.</param>
    /// <returns>Replacement block or null if block was not replaced.</returns>
    private Block? TrampleBlock(EntityTrampleEntry entry, TrampleBlockCfg trampleBlockCfg, BlockTrampleData blockTrampleData, Block block, BlockPos pos)
    {
        float tramplePower = ResolveTramplePower(entry);

        // Trample this block.
        blockTrampleData.Durability -= tramplePower; // Most important line in whole feature, this is where the block actually gets trampled.

        if (blockTrampleData.Durability <= 0f)
        {
            blockTrampleData.Durability = 0f;
            if (block.Code == trampleBlockCfg.ToBlockCode) return null; // Exactly same block as next block.

            Block? nextBlock = ResolveNextBlock(trampleBlockCfg, block, pos);
            if (nextBlock == null) return null; // Unknown next block, skip.

            // Replace current block with next block.
            sapi.World.BlockAccessor.ExchangeBlock(nextBlock.BlockId, pos); // Will NOT trigger TrampleBehavior.OnBlockRemoved().
            sapi.World.BlockAccessor.MarkBlockDirty(pos);

            // We will handle updating trample data for this block in RefreshTrampleData() after this function returns.
            return nextBlock;
        }

        // Still some durability left, so no block replaced.
        return null;
    }

    /// <summary>
    /// Block was replaced with another block, we need to refresh trample data.
    /// If that new block has no trample config, we will remove trample data for this block completely.
    /// Otherwise, we will reset trample data for this block to match new block's trample config.
    /// </summary>
    /// <param name="allTrampleData">All trample data.</param>
    /// <param name="replacementBlock">Block that replaced previous block.</param>
    /// <param name="pos">Position of block.</param>
    /// <param name="passable">Is block passable?</param>
    private void RefreshTrampleData(AllTrampleData allTrampleData, Block replacementBlock, BlockPos pos, bool passable)
    {
        TrampleBlockCfg? replacementTrampleBlockCfg = TrampleUtils.GetBlockConfig(sapi, replacementBlock, passable);
        if (replacementTrampleBlockCfg == null)
        {   // No trample config for this block, remove trample data completely.
            int localIndex = MapUtil.Index3d(pos.X & 31, pos.Y & 31, pos.Z & 31, 32, 32);
            allTrampleData.chunkData.Blocks.Remove(localIndex);
            return;
        }

        allTrampleData.blockData.Reset(replacementTrampleBlockCfg, sapi.World.Calendar.TotalDays);
        allTrampleData.blockData.Durability *= replacementTrampleBlockCfg.DurRatio; // Note we reduce durability of blocks placed due to trampling.
    }

    //

    /// <summary>
    /// Resolve next block after full trampling that should replace current block. For example, trampled grass becomes soil.
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
        Block? nextBlock = sapi.World.GetBlock(loc);
        if (nextBlock == null)
            sapi.Logger.Error($"[Trample] Failed to get block with code '{toBlockCode}' when trying to trample block '{block.Code}' at '{pos}'. Make sure the block code in config is correct.");

        return nextBlock;
    }

    //

    /// <summary>
    /// Resolve trample power.
    /// </summary>
    /// <param name="entry">Trampleable entity data.</param>
    /// <returns>Trample power.</returns>
    private float ResolveTramplePower(EntityTrampleEntry entry)
    {
        TrampleEntityCfg Entity = entry.TrampleEntityCfg;

        float BasePower = Entity.Power;
        float FallPower = entry.ImpactStrength * entry.TrampleEntityCfg.FallMul;
        float EquipmentMul = trampleItem.ResolveEquipmentMul(entry);
        return BasePower * EquipmentMul + FallPower;
    }
}