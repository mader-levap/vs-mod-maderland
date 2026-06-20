using MaderLand.Common.Config;
using MaderLand.Systems.Trample.Data;
using MaderLand.Systems.Trample.Services;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace MaderLand.Systems.Trample.Blocks;

/// <summary>
/// Replacement for BlockSoilDeposit. While trampling can be done with BlockBehavior, we need to prevent grass growth on trampled deposit, and BlockBehavior doesn't have a way to prevent that.
/// It covers clay and peat.
/// </summary>
public class BlockSoilDepositTrampleable : BlockSoilDeposit
{
    public override void OnServerGameTick(IWorldAccessor world, BlockPos pos, object extra)
    {
        if (!ConfigService.TrampleConfig.Active)
        {
            base.OnServerGameTick(world, pos, extra);
            return;
        }

        ICoreServerAPI api = (ICoreServerAPI)world.Api;
        AllTrampleData allTrampleData = TrampleUtils.GetTrampleData(api, pos);
        if (allTrampleData.blockData != null)
        {
            TrampleUtils.DeltaTrampleData(api, allTrampleData.blockData);

            if (allTrampleData.blockData.Durability >= allTrampleData.blockData.MaxDurability)
            {   // Block fully recovered from trampling, remove trample data.
                TrampleUtils.RemoveTrampleData(allTrampleData, pos);
            } else return; // Do not allow grass to grow if the soil is trampled.
        }

        // If the soil is not trampled, or if it has fully recovered, allow grass growth as normal.
        base.OnServerGameTick(world, pos, extra);
    }
}
