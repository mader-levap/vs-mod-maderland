using MaderLand.Config.Trample;
using MaderLand.Config.Utils;
using MaderLand.Systems.Trample.Data;
using MaderLand.Systems.Trample.Gui;
using MaderLand.Systems.Trample.Network;
using MaderLand.Systems.Trample.Render;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace MaderLand.Systems.Trample;

/// <summary>
/// Handles the trample feature logic, including player movement detection and block updates.
/// </summary>
public class TrampleSystem : ModSystem
{
    private ICoreServerAPI sapi = null!;
    private ICoreClientAPI capi = null!;
    private IServerNetworkChannel serverChannel = null!;
    private IClientNetworkChannel clientChannel = null!;

    private TrampleService trampleService = null!;
    private TrampleDebugWatcher blockSelWatcher = null!;

    /// <summary>
    /// Map of all player positions. Key is PlayerUID, Value is block position.
    /// </summary>
    private readonly Dictionary<string, BlockPos> lastPlayerPositions = [];

    /// <summary>
    /// Returns if the mod system should be loaded on given side.
    /// </summary>
    /// <param name="side">The side of the application.</param>
    /// <returns>True if it should load, false otherwise.</returns>
    public override bool ShouldLoad(EnumAppSide side)
    {
        // Trample feature is server-side mod, but it uses debug window for client GUI.
        return true;
    }

    //

    /// <summary>
    /// Starts the mod system on the server side.
    /// </summary>
    /// <param name="api">Core server API.</param>
    public override void StartServerSide(ICoreServerAPI serverApi)
    {
        base.StartServerSide(serverApi);
        sapi = serverApi;

        trampleService = new(sapi);

        TrampleInitializer.AppendHandleBehavior(sapi);

        // Register game tick listener running every 100ms.
        sapi.Event.RegisterGameTickListener(ProcessTramplingPlayers, 100);
        sapi.Event.PlayerLeave += OnPlayerGone;
        sapi.Event.PlayerDisconnect += OnPlayerGone;

        serverChannel = sapi.Network.RegisterChannel(TrampleConst.channelKey)
            .RegisterMessageType<TrampleDebugPacket>()
            .RegisterMessageType<TrampleDataReq>()
            .RegisterMessageType<TrampleDataResp>();
        serverChannel.SetMessageHandler<TrampleDataReq>((player, packet) => TrampleDebugService.ReactOnDataReq(sapi, player, packet));
    }

    /// <summary>
    /// Cleans up dictionary entry when a player leaves/disconnects to prevent memory leaks.
    /// </summary>
    /// <param name="player">The player leaving the server.</param>
    private void OnPlayerGone(IServerPlayer player)
    {
        lastPlayerPositions.Remove(player.PlayerUID);
    }

    //

    /// <summary>
    /// Starts the mod system on the client side.
    /// </summary>
    /// <param name="api">Core Client API</param>
    public override void StartClientSide(ICoreClientAPI api)
    {
        capi = api;

        clientChannel = api.Network.RegisterChannel(TrampleConst.channelKey)
            .RegisterMessageType<TrampleDebugPacket>()
            .RegisterMessageType<TrampleDataReq>()
            .RegisterMessageType<TrampleDataResp>();
        clientChannel.SetMessageHandler<TrampleDebugPacket>((packet) => TrampleDebugService.ReactOnDebugPacket(capi, packet));
        clientChannel.SetMessageHandler<TrampleDataResp>((packet) => TrampleDebugService.ReactOnDataResp(capi, packet));

        if (ConfigService.TrampleConfig.Debug)
        { // Debug active, already open debug window.
            GuiPanels.GetDebug(capi).TryOpen();
        }

        blockSelWatcher = new TrampleDebugWatcher(capi, clientChannel);
        capi.Event.RegisterGameTickListener(blockSelWatcher.Refresh, 100);
    }

    // ////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Process player movement and perform trampling logic if they walked onto a new block.
    /// </summary>
    /// <param name="dt">Delta time.</param>
    private void ProcessTramplingPlayers(float dt)
    {
        // Only run detection if the trample feature is active or allowed in the configuration
        if (sapi == null || !ConfigService.TrampleConfig.Active || !ConfigService.TrampleConfig.Allow) return;

        // Go over all players...
        foreach (IPlayer player in sapi.World.AllOnlinePlayers)
        {
            CheckPlayerMove((IServerPlayer)player);
        }
    }

    /// <summary>
    /// Check given player for walking onto new block.
    /// </summary>
    /// <param name="player">Player to check.</param>
    private void CheckPlayerMove(IServerPlayer player)
    {
        if (player.Entity is not EntityPlayer entityPlayer) return;
        if (!CanUsePlayer(player)) return;

        BlockPos currentPos = entityPlayer.Pos.AsBlockPos;

        if (lastPlayerPositions.TryGetValue(player.PlayerUID, out BlockPos? lastPos))
        {
            // If they have moved to a different block position.
            if (!currentPos.Equals(lastPos))
            {
                lastPlayerPositions[player.PlayerUID] = currentPos;
                OnPlayerWalkedOntoNewBlock(player, currentPos);
            }
        }
        else
        {
            // First time detecting player position.
            lastPlayerPositions[player.PlayerUID] = currentPos;
        }
    }

    /// <summary>
    /// Check if we can use player for trample logic.
    /// We only want to detect players in survival mode who are on the ground.
    /// </summary>
    /// <param name="player">The player to check.</param>
    /// <returns>True if the player can trample blocks.</returns>
    private static bool CanUsePlayer(IServerPlayer player)
    {
        if (player.ConnectionState != EnumClientState.Playing) return false;
        if (player.WorldData.CurrentGameMode != EnumGameMode.Survival) return false;

        if (!player.Entity.OnGround) return false;
        return true;
    }

    /// <summary>
    /// Triggered when a player moves onto a new block position.
    /// </summary>
    /// <param name="player">The player who moved.</param>
    /// <param name="posPlayer">The block position they walked onto.</param>
    private void OnPlayerWalkedOntoNewBlock(IServerPlayer player, BlockPos posPlayer)
    {
        trampleService.TrampleLogic(player, posPlayer);
    }
}
