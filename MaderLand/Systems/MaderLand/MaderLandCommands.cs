using MaderLand.Systems.Trample.Commands;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace MaderLand.Systems.MaderLand;

/// <summary>
/// Handles 'maderland' command.
/// </summary>
public static class MaderLandCommands
{
    /// <summary>
    /// Registers the /ml command. All commands have form of /ml [feature] [action] [all other parameters, optional]
    /// </summary>
    /// <param name="api">The server API used to register chat commands.</param>
    public static void Register(ICoreServerAPI api)
    {
        api.ChatCommands
            .Create("ml")
            .WithDescription("Shows a simple MaderLand test message")
            .RequiresPlayer()
            .RequiresPrivilege(Privilege.chat)
            .WithArgs(api.ChatCommands.Parsers.Word("feature"),
                      api.ChatCommands.Parsers.Word("action"),
                      api.ChatCommands.Parsers.OptionalAll("parameters"))
            .HandleWith(args =>
            {
                IServerPlayer? player = args.Caller.Player as IServerPlayer;
                if (player == null) return TextCommandResult.Error("[MaderLand] This command can only be used by a player.");

                string feature = (string)args[0];
                string action = (string)args[1];
                string? parameters = args[2] as string; // is null if skipped

                return Call(api, player, feature, action, parameters);
            });
    }

    /// <summary>
    /// Calls appropriate feature.
    /// </summary>
    /// <param name="api">Core API.</param>
    /// <param name="player">Player that issued command.</param>
    /// <param name="feature">Feature to use.</param>
    /// <param name="action">Action (feature-specific) to use.</param>
    /// <param name="parameters">All other parameters.</param>
    /// <returns>Result of command.</returns>
    private static TextCommandResult Call(ICoreServerAPI api, IServerPlayer player, string feature, string action, string? parameters)
    {
        switch (feature)
        {
            case "trample": return TrampleCommands.Handle(api, player, action, parameters);
            default:
                return TextCommandResult.Error($"[MaderLand] Unknown feature '{feature}'!");
        }
    }
}
