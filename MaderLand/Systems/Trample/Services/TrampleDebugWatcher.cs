using MaderLand.Common.Config;
using MaderLand.Systems.Trample.Network;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace MaderLand.Systems.Trample.Services;

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
    public void Refresh(float _)
    {
        if (!ConfigService.TrampleConfig.Debug) return;

        // Position of selection can be null if player pointed at sky or was blocked by entity.
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
