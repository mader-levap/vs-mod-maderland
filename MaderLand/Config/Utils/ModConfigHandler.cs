using System;
using Vintagestory.API.Common;

namespace MaderLand.Config.Utils;

/// <summary>
/// Generic helper for loading and saving mod config files.
/// </summary>
public class ModConfigHandler
{
    /// <summary>
    /// Loads a config file, or creates and stores a default instance if the file does not exist.
    /// </summary>
    /// <typeparam name="T">Config class type.</typeparam>
    /// <param name="api">Core API.</param>
    /// <param name="configPath">Relative config path, for example "maderland/trample.json".</param>
    /// <returns>The loaded config, or a default instance if loading failed or file did not exist.</returns>
    public static T Load<T>(ICoreAPI api, string configPath) where T : new()
    {
        try
        {
            T loadedConfig = api.LoadModConfig<T>(configPath);
            if (loadedConfig != null) return loadedConfig;
        }
        catch (Exception ex)
        {
            api.Logger.Error($"Failed to load config '{configPath}': {ex}");
        }

        T defaultConfig = new();
        api.StoreModConfig(defaultConfig, configPath);
        api.Logger.Notification($"Created default config '{configPath}'");
        return defaultConfig;
    }

    /// <summary>
    /// Saves a config object to disk.
    /// </summary>
    /// <typeparam name="T">Config class type.</typeparam>
    /// <param name="api">Core API.</param>
    /// <param name="configPath">Relative config path.</param>
    /// <param name="config">Config object to save.</param>
    public static void Save<T>(ICoreAPI api, string configPath, T config)
    {
        api.StoreModConfig(config, configPath);
    }
}
