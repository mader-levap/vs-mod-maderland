using MaderLand.Config.Trample;
using MaderLand.Config.Utils;
using MaderLand.Systems.Trample;
using MaderLand.Systems.Trample.Data;
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
            case "allow": return Allow(api, player, parameters);
            case "check": return CheckBlock(player);
            case "debug": return DebugBlock(api, player);
            default:
                return TextCommandResult.Error($"[Trample] Unknown action '{action}' for feature 'trample'!");
        }
    }

    /// <summary>
    /// Action: Activates or deactivates Trample feature. If off, trampling data is completely frozen.
    /// </summary>
    /// <param name="api">Core API.</param>
    /// <param name="player"></param>
    /// <param name="parameters">All other parameters.</param>
    /// <returns>Result of command.</returns>
    private static TextCommandResult Active(ICoreServerAPI api, IServerPlayer player, string? parameters)
    {
        if (parameters == null || parameters.Equals(""))
        { // Just flip.
            ConfigService.TrampleConfig.Active = !ConfigService.TrampleConfig.Active;
        }
        else if (parameters.Equals("on"))
        {
            if (ConfigService.TrampleConfig.Active)
            {
                player.SendMessage(GlobalConstants.GeneralChatGroup, "[Trample] Trample feature is already active!", EnumChatType.CommandSuccess);
                return TextCommandResult.Success();
            }
            ConfigService.TrampleConfig.Active = true;
        }
        else if (parameters.Equals("off"))
        {
            if (!ConfigService.TrampleConfig.Active)
            {
                player.SendMessage(GlobalConstants.GeneralChatGroup, "[Trample] Trample feature is already inactive!", EnumChatType.CommandSuccess);
                return TextCommandResult.Success();
            }
            ConfigService.TrampleConfig.Active = false;
        }
        else return TextCommandResult.Error($"[Trample] Command '/ml trample active': unknown parameter '{parameters}'!");

        TrampleConfigHandler.Save(api);
        string message = "[Trample] Trample feature is turned " + (ConfigService.TrampleConfig.Active ? "on" : "off") + ".";
        player.SendMessage(GlobalConstants.GeneralChatGroup, message, EnumChatType.CommandSuccess);
        return TextCommandResult.Success();
    }

    /// <summary>
    /// Action: allows or disallows trampling. If off, trampling does not occur, but trampling data is still processed.
    /// </summary>
    /// <param name="api">Core server API.</param>
    /// <param name="player"></param>
    /// <param name="parameters">All other parameters.</param>
    /// <returns>Result of command.</returns>
    private static TextCommandResult Allow(ICoreServerAPI api, IServerPlayer player, string? parameters)
    {
        if (parameters == null || parameters.Equals(""))
        { // Just flip.
            ConfigService.TrampleConfig.Allow = !ConfigService.TrampleConfig.Allow;
        }
        else if (parameters.Equals("on"))
        {
            if (ConfigService.TrampleConfig.Allow)
            {
                player.SendMessage(GlobalConstants.GeneralChatGroup, "[Trample] Trampling is already allowed!", EnumChatType.CommandSuccess);
                return TextCommandResult.Success();
            }
            ConfigService.TrampleConfig.Allow = true;
        }
        else if (parameters.Equals("off"))
        {
            if (!ConfigService.TrampleConfig.Allow)
            {
                player.SendMessage(GlobalConstants.GeneralChatGroup, "[Trample] Trampling is already disallowed!", EnumChatType.CommandSuccess);
                return TextCommandResult.Success();
            }
            ConfigService.TrampleConfig.Allow = false;
        }
        else return TextCommandResult.Error($"[Trample] Command '/ml trample allow': unknown parameter '{parameters}'!");

        TrampleConfigHandler.Save(api);
        string message = "[Trample] Trampling is now " + (ConfigService.TrampleConfig.Allow ? "allowed" : "not allowed") + ".";
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
        BlockPos standingOnPos = playerPos.DownCopy();

        // Get the block instance from the world.
        Block standingOnBlock = player.Entity.World.BlockAccessor.GetBlock(standingOnPos);

        // Construct the message with the block's code (e.g., game:dirt).
        string message = $"[Trample] You are standing on: {standingOnBlock.Code} (ID: {standingOnBlock.Id})";

        player.SendMessage(GlobalConstants.GeneralChatGroup, message, EnumChatType.CommandSuccess);
        return TextCommandResult.Success();
    }

    /// <summary>
    /// Action: Check trample data of block player is standing on. Creative mode is recommended as in this case player does not trample.
    /// </summary>
    /// <param name="api">Core server API.</param>
    /// <param name="player">Player that input this command.</param>
    /// <returns>Result of command.</returns>
    private static TextCommandResult DebugBlock(ICoreServerAPI api, IServerPlayer player)
    {
        // Get the player's current block position (feet level).
        BlockPos playerPos = player.Entity.Pos.AsBlockPos;
        // Get the position directly beneath the player's feet.
        BlockPos standingOnPos = playerPos.DownCopy();

        string message = "[Trample] No trample data for block under you.";

        BlockTrampleData ? trampleData = TrampleService.GetTrampleData(api, standingOnPos);
        if (trampleData != null)
        {
            message = $"[Trample] Trample data: Durability={trampleData.Durability}, MaxDurability={trampleData.MaxDurability}, Regen={trampleData.Regen}";
        }

        player.SendMessage(GlobalConstants.GeneralChatGroup, message, EnumChatType.CommandSuccess);
        return TextCommandResult.Success();
    }
}