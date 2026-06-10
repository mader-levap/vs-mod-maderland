using MaderLand.Config.Utils;
using Vintagestory.API.Common;

namespace MaderLand.Config.Trails;

/// <summary>
/// Handles the Trails config file.
/// </summary>
public class TrailsConfigHandler
{
    /// <summary>
    /// Relative path inside the Vintage Story ModConfig area.
    /// </summary>
    public const string ConfigPath = "maderland/trails.json";

    public static TrailsCfg Load(ICoreAPI api) => ModConfigHandler.Load<TrailsCfg>(api, ConfigPath);

    /// <summary>
    /// Save configuration for Trails feature.
    /// </summary>
    /// <param name="api">Core API.</param>
    public static void Save(ICoreAPI api) => ModConfigHandler.Save(api, ConfigPath, ConfigService.TrailsConfig);
}
