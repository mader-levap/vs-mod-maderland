using MaderLand.Config.Utils;
using MaderLand.Systems.Trample.Data;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace MaderLand.Systems.Trample.Blocks;

/// <summary>
/// Replacement for BlockSoil. While trampling can be done with BlockBehavior, we need to prevent grass growth on trampled soil, and BlockBehavior doesn't have a way to prevent that.
/// </summary>
public class BlockSoilTrampleable : BlockSoil
{
    public override void OnServerGameTick(IWorldAccessor world, BlockPos pos, object extra)
    {
        if (!ConfigService.TrampleConfig.Active)
        {
            base.OnServerGameTick(world, pos, extra);
            return;
        }

        ICoreServerAPI api = (ICoreServerAPI)world.Api;
        AllTrampleData allTrampleData = TrampleService.GetTrampleData(api, pos);
        if (allTrampleData.blockData != null)
        {
            TrampleService.DeltaTrampleData(api, allTrampleData.blockData);

            if (allTrampleData.blockData.Durability >= allTrampleData.blockData.MaxDurability)
            { // Block fully recovered from trampling, remove trample data.
                TrampleService.RemoveTrampleData(api, allTrampleData, pos);
            } else return; // Do not allow grass to grow if the soil is trampled.
        }

        // If the soil is not trampled, or if it has fully recovered, allow grass growth as normal.
        base.OnServerGameTick(world, pos, extra);
    }
}
