using MaderLand.Config.Trample;
using MaderLand.Config.Utils;
using MaderLand.Systems.Trample;
using MaderLand.Systems.Trample.Network;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
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
            case "debug": return Debug(api, player, parameters);
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
    /// Action: turns debug mode on or off. If on, mod will show various debug information in GUI panel.
    /// </summary>
    /// <param name="api">Core server API.</param>
    /// <param name="player"></param>
    /// <param name="parameters">All other parameters.</param>
    /// <returns>Result of command.</returns>
    private static TextCommandResult Debug(ICoreServerAPI api, IServerPlayer player, string? parameters)
    {
        if (parameters == null || parameters.Equals(""))
        { // Just flip.
            ConfigService.TrampleConfig.Debug = !ConfigService.TrampleConfig.Debug;
        }
        else if (parameters.Equals("on"))
        {
            if (ConfigService.TrampleConfig.Debug)
            {
                player.SendMessage(GlobalConstants.GeneralChatGroup, "[Trample] Debug mode is already on!", EnumChatType.CommandSuccess);
                return TextCommandResult.Success();
            }
            ConfigService.TrampleConfig.Debug = true;
        }
        else if (parameters.Equals("off"))
        {
            if (!ConfigService.TrampleConfig.Debug)
            {
                player.SendMessage(GlobalConstants.GeneralChatGroup, "[Trample] Debug mode is already off!", EnumChatType.CommandSuccess);
                return TextCommandResult.Success();
            }
            ConfigService.TrampleConfig.Debug = false;
        }
        else return TextCommandResult.Error($"[Trample] Command '/ml trample debug': unknown parameter '{parameters}'!");

        TrampleConfigHandler.Save(api);

        // Show feedback in game console.
        string message = "[Trample] Debug mode is now " + (ConfigService.TrampleConfig.Debug ? "on" : "off") + ".";
        player.SendMessage(GlobalConstants.GeneralChatGroup, message, EnumChatType.CommandSuccess);

        // Show/hide debug window on client.
        var channel = api.Network.GetChannel(TrampleConst.channelKey);
        channel?.SendPacket(new TrampleDebugPacket { Enabled = ConfigService.TrampleConfig.Debug }, player);
        return TextCommandResult.Success();
    }
}