using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;

namespace MaderLand.Utils;

/// <summary>
/// Utilities for entities.
/// </summary>
public class EntityUtils
{
    /// <summary>
    /// Tries to resolve server player from entity. Use on server side.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <returns>Player or null if entity is not player.</returns>
    public static IServerPlayer? ResolvePlayer(Entity entity)
    {
        if (entity is not EntityPlayer entityPlayer) return null;
        if (entityPlayer.Player is not IServerPlayer serverPlayer) return null;
        return serverPlayer;
    }
}
