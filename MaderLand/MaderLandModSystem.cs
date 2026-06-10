using MaderLand.Commands;
using MaderLand.Config.Utils;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace MaderLand;

/// <summary>
/// Main entry point for this mod.
/// </summary>
public class MaderLandModSystem : ModSystem
{
    /// <summary>
    /// Initialization common for both server and client.
    /// </summary>
    /// <param name="api">Core API.</param>
    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        ConfigService.LoadAll(api);

        Mod.Logger.Notification($"MaderLand mod initialized. TrailsConfig.Active: {ConfigService.TrailsConfig.Active}");
    }

    /// <summary>
    /// Initialization for server.
    /// </summary>
    /// <param name="api">Core server API.</param>
    public override void StartServerSide(ICoreServerAPI api)
    {
        MaderLandCommand.Register(api);
        Mod.Logger.Notification("MaderLand mod, server side initialized.");
    }

    /// <summary>
    /// Initialization for client.
    /// </summary>
    /// <param name="api">Core client API.</param>
    public override void StartClientSide(ICoreClientAPI api)
    {
        Mod.Logger.Notification("MaderLand mod, client side initialized.");
    }
}
