using MaderLand.Config.MaderLand;
using MaderLand.Config.Trample;
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
    /// Configuration for Trample feature.
    /// </summary>
    public static TrampleCfg TrampleConfig { get; private set; } = new();

    /// <summary>
    /// Load all configuration files.
    /// </summary>
    /// <param name="api">Core API.</param>
    public static void LoadAll(ICoreAPI api)
    {
        MaderLandConfig = MaderLandConfigHandler.Load(api);
        TrampleConfig = TrampleConfigHandler.Load(api);
    }

    /// <summary>
    /// Save all configuration files.
    /// </summary>
    /// <param name="api">Core API.</param>
    public static void SaveAll(ICoreAPI api)
    {
        MaderLandConfigHandler.Save(api);
        TrampleConfigHandler.Save(api);
    }
}
