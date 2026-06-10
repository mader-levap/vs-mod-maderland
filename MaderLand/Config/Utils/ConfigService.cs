using MaderLand.Config.MaderLand;
using MaderLand.Config.Trails;
using Vintagestory.API.Common;

namespace MaderLand.Config.Utils;

/// <summary>
/// Manages configuration. This is also where you access configuration.
/// </summary>
public class ConfigService
{
    /// <summary>
    /// Main MaderLand configuration.
    /// </summary>
    public static MaderLandConfig MaderLandConfig { get; private set; } = new();
    /// <summary>
    /// Configuration for Trails feature.
    /// </summary>
    public static TrailsCfg TrailsConfig { get; private set; } = new();

    /// <summary>
    /// Load all configuration files.
    /// </summary>
    /// <param name="api">Core API.</param>
    public static void LoadAll(ICoreAPI api)
    {
        MaderLandConfig = MaderLandConfigHandler.Load(api);
        TrailsConfig = TrailsConfigHandler.Load(api);
    }

    /// <summary>
    /// Save all configuration files.
    /// </summary>
    /// <param name="api">Core API.</param>
    public static void SaveAll(ICoreAPI api)
    {
        MaderLandConfigHandler.Save(api);
        TrailsConfigHandler.Save(api);
    }
}
