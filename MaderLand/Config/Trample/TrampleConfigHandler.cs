using MaderLand.Config.Utils;
using MaderLand.Systems.Trample;
using Vintagestory.API.Common;

namespace MaderLand.Config.Trample;

/// <summary>
/// Handles the Trample config file.
/// </summary>
public class TrampleConfigHandler
{
    /// <summary>
    /// Relative path inside the Vintage Story ModConfig area.
    /// </summary>
    public const string ConfigPath = "maderland/trample.json";

    public static TrampleCfg Load(ICoreAPI api)
    {
        TrampleCfg trampleCfg = ModConfigHandler.Load<TrampleCfg>(api, ConfigPath, TrampleConst.DefaultTrampleCfg);

        string message = $"[Trample] Loaded config. Passable: Blocks={trampleCfg.Passable.Blocks.Count}, BlockVariants={trampleCfg.Passable.BlockVariants.Count}. Impassable: Blocks={trampleCfg.Impassable.Blocks.Count}, BlockVariants={trampleCfg.Impassable.BlockVariants.Count}.";
        api.Logger.Notification(message);
        return trampleCfg;
    }

    /// <summary>
    /// Save configuration for Trample feature.
    /// </summary>
    /// <param name="api">Core API.</param>
    public static void Save(ICoreAPI api)
    {
        ModConfigHandler.Save(api, ConfigPath, ConfigService.TrampleConfig);
    }
}
