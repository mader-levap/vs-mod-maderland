using MaderLand.Config.Utils;
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

    public static TrampleCfg Load(ICoreAPI api) => ModConfigHandler.Load<TrampleCfg>(api, ConfigPath);

    /// <summary>
    /// Save configuration for Trample feature.
    /// </summary>
    /// <param name="api">Core API.</param>
    public static void Save(ICoreAPI api) => ModConfigHandler.Save(api, ConfigPath, ConfigService.TrampleConfig);
}
