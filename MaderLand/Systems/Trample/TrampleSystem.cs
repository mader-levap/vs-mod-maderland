using MaderLand.Common.Config;
using MaderLand.Systems.Trample.Data;
using MaderLand.Systems.Trample.Gui;
using MaderLand.Systems.Trample.Network;
using MaderLand.Systems.Trample.Services;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
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

    private TrampleMain trampleMain = null!;
    private TrampleDebugWatcher blockSelWatcher = null!;

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

        trampleMain = new(sapi);

        TrampleInitializer.AppendHandleBehavior(sapi);

        // Register game tick listener running every 100ms.
        sapi.Event.RegisterGameTickListener(trampleMain.ProcessTramplingEntities, 100);

        sapi.Event.OnEntitySpawn += OnEntitySpawn;
        sapi.Event.OnEntityDespawn += OnEntityDespawn;
        sapi.Event.OnEntityLoaded += OnEntityLoaded;

        serverChannel = sapi.Network.RegisterChannel(TrampleConst.channelKey)
            .RegisterMessageType<TrampleDebugPacket>()
            .RegisterMessageType<TrampleDataReq>()
            .RegisterMessageType<TrampleDataResp>();
        serverChannel.SetMessageHandler<TrampleDataReq>((player, packet) => TrampleDebugService.ReactOnDataReq(sapi, player, packet));
    }

    private void OnEntitySpawn(Entity entity)
    {
        trampleMain.Add(entity);
    }

    private void OnEntityLoaded(Entity entity)
    {
        trampleMain.Add(entity);
    }

    private void OnEntityDespawn(Entity entity, EntityDespawnData reasonData)
    {
        trampleMain.Remove(entity);
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
        {   // Debug active, already open debug window.
            GuiPanels.GetDebug(capi).TryOpen();
        }

        blockSelWatcher = new TrampleDebugWatcher(capi, clientChannel);
        capi.Event.RegisterGameTickListener(blockSelWatcher.Refresh, 100);
    }
}
