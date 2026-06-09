using MaderLand.Config.Utils;
using Vintagestory.API.Common;

namespace MaderLand.Config.MaderLand;

/// <summary>
/// Handles the main MaderLand config file.
/// </summary>
public class MaderLandConfigHandler
{
    /// <summary>
    /// Relative path inside the Vintage Story ModConfig area.
    /// </summary>
    public const string ConfigPath = "maderland/maderland.json";

    public static MaderLandConfig Load(ICoreAPI api) => ModConfigHandler.Load<MaderLandConfig>(api, ConfigPath);

    public static void Save(ICoreAPI api) => ModConfigHandler.Save(api, ConfigPath, ConfigService.MaderLandConfig);
}
