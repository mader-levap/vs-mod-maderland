using MaderLand.Config.Trample;
using MaderLand.Config.Utils;
using MaderLand.Systems.Manager;
using MaderLand.Systems.Trample.Behaviors;
using MaderLand.Systems.Trample.Blocks;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace MaderLand.Systems.Trample;

/// <summary>
/// Initializer for the trample system. It can be used to set up any necessary data structures, register block behaviors, etc.
/// </summary>
public class TrampleInitializer : IInitializer
{
    public static void Init(ICoreAPI api)
    {
        /// We need to replace in-place some blocks like soil so it works seamlessly with existing worldgen and structures.
        /// This is done by mapping the existing blocks to our trampleable versions.
        api.ClassRegistry.BlockClassToTypeMapping["BlockSoil"] = typeof(BlockSoilTrampleable);
        api.ClassRegistry.BlockClassToTypeMapping["BlockSoilDeposit"] = typeof(BlockSoilDepositTrampleable);
    }

    //

    /// <summary>
    /// Add trample handling behavior to all blocks that can be trampled.
    /// This ensures trample data is updated/removed if the block is broken, placed, replaced, eaten by animals, etc.
    /// </summary>
    /// <param name="api">Core server API.</param>
    /// <param name="trampleSystem">Trample system.</param>
    public static void AppendHandleBehavior(ICoreServerAPI api)
    {
        if (ConfigService.TrampleConfig == null)
        {
            api.Logger.Error("[Trample] TrampleConfig is null. Make sure to load the config before finalizing assets.");
            return;
        }

        AppendHandleBehavior(api, ConfigService.TrampleConfig.Passable);
        AppendHandleBehavior(api, ConfigService.TrampleConfig.Impassable);
    }

    /// <summary>
    /// Add trample handling behavior to all blocks defined in the given trample group configuration.
    /// </summary>
    /// <param name="api">Core server API.</param>
    /// <param name="trampleGroupCfg">Trample group.</param>
    private static void AppendHandleBehavior(ICoreServerAPI api, TrampleGroupCfg trampleGroupCfg)
    {
        if (trampleGroupCfg == null) return;

        if (trampleGroupCfg.Blocks != null && trampleGroupCfg.Blocks.Count > 0)
        {
            foreach (string blockCode in trampleGroupCfg.Blocks.Keys)
            {
                AppendHandleBehavior(api, blockCode);
            }
        }

        if (trampleGroupCfg.BlockVariants != null && trampleGroupCfg.BlockVariants.Count > 0)
        {
            foreach (TrampleBlockVariantCfg variant in trampleGroupCfg.BlockVariants)
            {
                Block[] allVariants = api.World.SearchBlocks(variant.FromBlockCode);
                foreach (Block block in allVariants)
                {
                    AppendHandleBehavior(api, block.Code);
                }
            }
        }
    }

    /// <summary>
    /// Appends the TrampleHandleBehavior to blocks that can be trampled.
    /// </summary>
    /// <param name="api">Core server API.</param>
    /// <param name="blockCode">Block code.</param>
    private static void AppendHandleBehavior(ICoreServerAPI api, AssetLocation blockCode)
    {
        Block? block = api.World.GetBlock(blockCode);
        if (block == null)
        {
            api.Logger.Error($"[Trample] AppendCleanupBehavior(): Could not find block class for '{blockCode}'.");
            return;
        }
        block.BlockBehaviors = block.BlockBehaviors.Append(new TrampleBehavior(api, block));

        //string message = $"[Trample] Added trample behavior to '{blockCode}'.";
        //api.Logger.Notification(message); // DEBUG
    }
}
