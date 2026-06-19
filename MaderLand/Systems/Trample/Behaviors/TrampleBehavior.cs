using MaderLand.Common.Config;
using MaderLand.Systems.Trample.Services;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace MaderLand.Systems.Trample.Behaviors;

/// <summary>
/// A block behavior that automatically handles trample data when the block is removed or replaced.
/// Right now it only clears trample data, but in future it might update data if block was replaced with other trample-able block.
/// </summary>
public class TrampleBehavior(ICoreServerAPI api, Block block) : BlockBehavior(block)
{
    /// <summary>
    /// When the block is removed from this position (broken, replaced, etc.), we must clear any trample data stored for it.
    /// Note: it is NOT called when block is removed due to trampling.
    /// </summary>
    /// <param name="world">World accessor.</param>
    /// <param name="pos">Block position.</param>
    /// <param name="handling">Handling of behavior.</param>
    public override void OnBlockRemoved(IWorldAccessor world, BlockPos blockPos, ref EnumHandling handling)
    {
        if (!ConfigService.TrampleConfig.Active) return; // Only run behavior if the trample feature is active in the configuration.

        TrampleUtils.RemoveTrampleData(api, blockPos);
    }
}
