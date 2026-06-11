using MaderLand.Config.Trample;
using MaderLand.Config.Utils;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace MaderLand.Commands;

/// <summary>
/// Handles 'trample' subcommand.
/// </summary>
public static class TrampleCommands
{
    /// <summary>
    /// Handle trample-specific command.
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
                return TextCommandResult.Error($"Unknown action '{action}' for feature 'trample'!");
        }
    }

    /// <summary>
    /// Action: Activates or deactivates Trample feature.
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
            ConfigService.TrampleConfig.Active = !ConfigService.TrampleConfig.Active;
        } else if (parameters.Equals("on")) {
            if (ConfigService.TrampleConfig.Active)
            {
                player.SendMessage(GlobalConstants.GeneralChatGroup, "Trample feature is already active!", EnumChatType.CommandSuccess);
                return TextCommandResult.Success();
            }
            ConfigService.TrampleConfig.Active = true;
        } else if (parameters.Equals("off"))
        {
            if (!ConfigService.TrampleConfig.Active)
            {
                player.SendMessage(GlobalConstants.GeneralChatGroup, "Trample feature is already inactive!", EnumChatType.CommandSuccess);
                return TextCommandResult.Success();
            }
            ConfigService.TrampleConfig.Active = false;
        }
        else return TextCommandResult.Error($"Command '/ml trample active': unknown parameter '{parameters}'!");

        TrampleConfigHandler.Save(api);
        string message = "Trample feature is turned " + (ConfigService.TrampleConfig.Active ? "on" : "off") + ".";
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
