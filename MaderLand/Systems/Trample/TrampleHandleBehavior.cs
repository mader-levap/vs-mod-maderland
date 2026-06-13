using MaderLand.Config.Utils;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace MaderLand.Systems.Trample;

/// <summary>
/// A block behavior that automatically handles trample data when the block is removed or replaced.
/// Right now it only clears trample data, but in future it might update data if block was replaced with other trample-able block.
/// </summary>
public class TrampleHandleBehavior(TrampleSystem trampleSystem, Block block) : BlockBehavior(block)
{
    /// <summary>
    /// When the block is removed from this position (broken, replaced, etc.), we must clear any trample data stored for it.
    /// Note it includes case when block is replaced/removed due to trampling - in that case we also want to clear old trample data,
    /// because new block may be different and have different durability.
    /// </summary>
    /// <param name="world">World accessor.</param>
    /// <param name="pos">Block position.</param>
    /// <param name="handling">Handling of behavior.</param>
    public override void OnBlockRemoved(IWorldAccessor world, BlockPos pos, ref EnumHandling handling)
    {
        // Only run behavior if the trample feature is active in the configuration
        if (!ConfigService.TrampleConfig.Active) return;

        trampleSystem.RemoveTrampleData(pos);

        string message = $"[Trample] Block '{block.Code}' at {pos} was removed. Cleaned any trampling data.";
        world.Logger.Notification(message); // DEBUG

        base.OnBlockRemoved(world, pos, ref handling);
    }
}
