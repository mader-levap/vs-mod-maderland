using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace MaderLand.Systems.Trample;

/// <summary>
/// A block behavior that automatically removes trample data when the block is removed or replaced.
/// </summary>
public class TrampleCleanupBehavior(TrampleSystem trampleSystem, Block block) : BlockBehavior(block)
{
    /// <summary>
    /// When the block is removed from this position (broken, replaced, etc.), we must clear any trample data stored for it.
    /// Note it includes case when block is replaced/removed due to trampling - in that case we also want to clear old trample data,
    /// because new block may be different and have different durability.
    /// </summary>
    /// <param name="world"></param>
    /// <param name="pos">Block position.</param>
    /// <param name="handling">Handling of behavior.</param>
    public override void OnBlockRemoved(IWorldAccessor world, BlockPos pos, ref EnumHandling handling)
    {
        trampleSystem.RemoveTrampleData(pos);
        base.OnBlockRemoved(world, pos, ref handling);

        string message = $"[Trample] Block '{block.Code}' at {pos} was removed. Cleaned any trampling data.";
        world.Logger.Notification(message); // DEBUG
    }

    /*public override void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier, ref EnumHandling handling)
    {
        string message = $"[Trample] Block '{block.Code}' at {pos} was broken. Cleaned any trampling data.";
        world.Logger.Notification(message); // DEBUG
    }*/
}
