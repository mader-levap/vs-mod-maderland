using MaderLand.Systems.Trample.Services;
using Vintagestory.API.Common;

namespace MaderLand.Common.Manager;

/// <summary>
/// Manager for handling features of the mod.
/// </summary>
public class FeatureManager
{
    /// <summary>
    /// Initializes all features of the mod.
    /// </summary>
    /// <param name="api">Core API.</param>
    public static void InitializeAll(ICoreAPI api)
    {
        TrampleInitializer.Init(api);
    }
}
