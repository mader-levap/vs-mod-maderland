using MaderLand.Config.Trails;
using MaderLand.Config.Utils;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace MaderLand.Commands;

/// <summary>
/// Handles 'trails' subcommand.
/// </summary>
public static class TrailsCommands
{
    /// <summary>
    /// Handle trail-specific command.
    /// </summary>
    /// <param name="api">Core API.</param>
    /// <param name="player">Player that issued command.</param>
    /// <param name="action">Action (feature-specific) to use.</param>
    /// <param name="parameters">All other parameters.</param>
    public static TextCommandResult Handle(ICoreServerAPI api, IServerPlayer player, string action, string? parameters)
    {
        switch (action)
        {
            case "active": return Active(api, player, parameters);
            case "check": return CheckBlock(player);
            default:
                return TextCommandResult.Error($"Unknown action '{action}' for feature 'trails'!");
        }
    }

    /// <summary>
    /// Action: Activates or deactivates Trails feature.
    /// </summary>
    /// <param name="api">Core API.</param>
    /// <param name="player"></param>
    /// <param name="parameters">All other parameters.</param>
    /// <returns>Result of command.</returns>
    private static TextCommandResult Active(ICoreServerAPI api, IServerPlayer player, string? parameters)
    {
        if (parameters == null || parameters.Equals(""))
        {
            // Just flip.
            ConfigService.TrailsConfig.Active = !ConfigService.TrailsConfig.Active;
        } else if (parameters.Equals("on")) {
            if (ConfigService.TrailsConfig.Active)
            {
                player.SendMessage(GlobalConstants.GeneralChatGroup, "Trails feature is already active!", EnumChatType.CommandSuccess);
                return TextCommandResult.Success();
            }
            ConfigService.TrailsConfig.Active = true;
        } else if (parameters.Equals("off"))
        {
            if (!ConfigService.TrailsConfig.Active)
            {
                player.SendMessage(GlobalConstants.GeneralChatGroup, "Trails feature is already inactive!", EnumChatType.CommandSuccess);
                return TextCommandResult.Success();
            }
            ConfigService.TrailsConfig.Active = false;
        }
        else return TextCommandResult.Error($"Command '/ml trails active': unknown parameter '{parameters}'!");

        TrailsConfigHandler.Save(api);
        string message = "Trails feature is turned " + (ConfigService.TrailsConfig.Active ? "on" : "off") + ".";
        player.SendMessage(GlobalConstants.GeneralChatGroup, message, EnumChatType.CommandSuccess);
        return TextCommandResult.Success();
    }

    /// <summary>
    /// Action: Check code of block player is standing on.
    /// </summary>
    /// <param name="player">Player that input this command.</param>
    /// <returns>Result of command.</returns>
    private static TextCommandResult CheckBlock(IServerPlayer player)
    {
        // TODO: check code of block player is standing on.
        string message = "Here will be message about block you are standing on.";

        player.SendMessage(GlobalConstants.GeneralChatGroup, message, EnumChatType.CommandSuccess);
        return TextCommandResult.Success();
    }
}
