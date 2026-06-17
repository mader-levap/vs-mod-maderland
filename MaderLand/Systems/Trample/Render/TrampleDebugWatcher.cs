using MaderLand.Config.Utils;
using MaderLand.Systems.Trample.Network;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace MaderLand.Systems.Trample.Render;

/// <summary>
/// Checks for change in block selection.
/// </summary>
public class TrampleDebugWatcher(ICoreClientAPI capi, IClientNetworkChannel channel)
{
    private BlockPos? lastSelectedPos;

    /// <summary>
    /// Recheck if selection changed. If so, request trampling data from server. 
    /// </summary>
    /// <param name="dt">Delta time.</param>
    public void Refresh(float dt)
    {
        if (!ConfigService.TrampleConfig.Debug) return;

        // Position of selection can be null if player pointed at sky.
        BlockPos? currentPos = capi.World.Player.CurrentBlockSelection?.Position;

        bool changed = (currentPos == null) != (lastSelectedPos == null)
                    || (currentPos != null && !currentPos.Equals(lastSelectedPos));

        if (changed)
        {
            lastSelectedPos = currentPos?.Copy();
            channel.SendPacket(new TrampleDataReq { Pos = currentPos });
        }
    }

}
