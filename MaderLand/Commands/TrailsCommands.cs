using MaderLand.Config.Trails;
using MaderLand.Config.Utils;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
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
        // Get the player's current block position (feet level).
        BlockPos playerPos = player.Entity.Pos.AsBlockPos;
        
        // Get the position directly beneath the player's feet.
        // Use DownCopy() instead of Down() to avoid mutating the player's actual position object
        BlockPos standingOnPos = playerPos.DownCopy();

        // Get the block instance from the world.
        Block standingOnBlock = player.Entity.World.BlockAccessor.GetBlock(standingOnPos);

        // Construct the message with the block's code (e.g., game:dirt).
        string message = $"You are standing on: {standingOnBlock.Code} (ID: {standingOnBlock.Id})";

        player.SendMessage(GlobalConstants.GeneralChatGroup, message, EnumChatType.CommandSuccess);
        return TextCommandResult.Success();
    }
}
