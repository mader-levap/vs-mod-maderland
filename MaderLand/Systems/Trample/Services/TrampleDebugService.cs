using MaderLand.Systems.Trample.Data;
using MaderLand.Systems.Trample.Gui;
using MaderLand.Systems.Trample.Network;
using Vintagestory.API.Client;
using Vintagestory.API.Server;

namespace MaderLand.Systems.Trample.Services;

/// <summary>
/// Handles debugging.
/// </summary>
public class TrampleDebugService
{
    // CLIENT

    /// <summary>
    /// Reacts on debug packet.
    /// </summary>
    /// <param name="capi">Core client API.</param>
    /// <param name="packet">Debug mode change.</param>
    public static void ReactOnDebugPacket(ICoreClientAPI capi, TrampleDebugPacket packet)
    {
        if (packet.Enabled) GuiPanels.GetDebug(capi).TryOpen();
        else GuiPanels.GetDebug(capi).TryClose();
    }

    /// <summary>
    /// React on data packet.
    /// </summary>
    /// <param name="capi">Core client API.</param>
    /// <param name="packet">Trample data response.</param>
    public static void ReactOnDataResp(ICoreClientAPI capi, TrampleDataResp packet)
    {
        // update debug panel
        GuiPanels.GetDebug(capi).UpdateGui(packet);
    }

    // SERVER

    /// <summary>
    /// React on request for trample data.
    /// </summary>
    /// <param name="sapi">Core server API.</param>
    /// <param name="packet">Trample data request.</param>
    public static void ReactOnDataReq(ICoreServerAPI sapi, IServerPlayer player, TrampleDataReq packet)
    {
        // Client asked for trample data.
        AllTrampleData? allTrampleData = null;
        if (packet.Pos != null) allTrampleData = TrampleUtils.GetTrampleData(sapi, packet.Pos);

        // Send requested data back to client.
        var channel = sapi.Network.GetChannel(TrampleConst.channelKey);
        channel?.SendPacket(new TrampleDataResp { Pos = packet.Pos, Data = allTrampleData?.blockData }, player);
    }
}
