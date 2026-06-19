using MaderLand.Common.Config;
using MaderLand.Systems.Trample.Config;
using MaderLand.Systems.Trample.Data;
using MaderLand.Utils;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace MaderLand.Systems.Trample.Services;

/// <summary>
/// Main trample code.
/// </summary>
public class TrampleMain
{
    readonly ICoreServerAPI sapi;
    readonly TrampleService trampleService;

    /// <summary>
    /// List of trampleable entities.
    /// </summary>
    private readonly Dictionary<long, EntityTrampleEntry> entityEntries = [];

    public TrampleMain(ICoreServerAPI api)
    {
        sapi = api;
        trampleService = new(sapi);
    }

    //

    /// <summary>
    /// Add entity to list, if it is eligible. Can safely add same entity multiple times, will simply ignore later calls.
    /// </summary>
    /// <param name="entity">Entity to add.</param>
    public void Add(Entity entity)
    {
        // If we already have this entity on list, skip.
        if (entityEntries.ContainsKey(entity.EntityId)) return;

        TrampleEntityCfg? trampleEntityCfg = ResolveCfg(entity);
        if (trampleEntityCfg == null) return; // Will not add this entity - it does not appear in configuration.

        EntityTrampleEntry newEntry = Create(entity, trampleEntityCfg);

        //string message = $"[Trample] TrampleMain.Add(). Added entity '{newEntry.Name}'.";
        //sapi.Logger.Notification(message); // DEBUG

        entityEntries.Add(entity.EntityId, newEntry);
    }

    /// <summary>
    /// Find out if this entity has configuration entry.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <returns>Configuration entry for this entity or null if no configuration exist</returns>
    private static TrampleEntityCfg? ResolveCfg(Entity entity)
    {
        foreach (TrampleEntityCfg entityCfg in ConfigService.TrampleConfig.Entities)
        {
            if (Matches(entityCfg, entity)) return entityCfg;
        }
        return null;
    }

    /// <summary>
    /// Check if config entry matches given entity
    /// </summary>
    /// <param name="entityCfg">Trample entity config.</param>
    /// <param name="entity">Entity to check.</param>
    /// <returns>True if configuration matches entity, otherwise false.</returns>
    private static bool Matches(TrampleEntityCfg entityCfg, Entity entity)
    {
        return WildcardUtil.Match(entityCfg.EntityCode, entity.Code.ToString());
    }

    /// <summary>
    /// Create trampleable entity entry for given data.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="trampleEntityCfg">Trample configuration entry for this entity.</param>
    /// <returns></returns>
    private static EntityTrampleEntry Create(Entity entity, TrampleEntityCfg trampleEntityCfg)
    {
        string name;
        IServerPlayer? player = EntityUtils.ResolvePlayer(entity);
        if (player != null) name = player.PlayerName;
        else name = $"{entity.GetName()} ({entity.EntityId})";
        return new EntityTrampleEntry(entity, name, trampleEntityCfg);
    }

    //

    /// <summary>
    /// Remove entity from list.
    /// </summary>
    /// <param name="entity">Entity to remove.</param>
    public void Remove(Entity entity)
    {
        entityEntries.Remove(entity.EntityId);
        /*string? name = entityEntries.TryGetValue(entity.EntityId, out var e) ? e.Name : null;
        // Find entry with this entity and remove it.
        bool result = entityEntries.Remove(entity.EntityId);
        if (result)
        {
            string message = $"[Trample] TrampleMain.Remove(). Removed entity '{name}'.";
            sapi.Logger.Notification(message); // DEBUG
        }*/
    }

    // ////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Process entity movement and perform trampling logic if they walked onto a new block.
    /// </summary>
    /// <param name="dt">Delta time.</param>
    public void ProcessTramplingEntities(float dt)
    {
        // Only run detection if the trample feature is active or allowed in the configuration
        if (sapi == null || !ConfigService.TrampleConfig.Active || !ConfigService.TrampleConfig.Allow) return;

        // Go over all entities...
        foreach (long key in entityEntries.Keys)
        {
            CheckEntityMove(entityEntries[key]);
        }
    }

    /// <summary>
    /// Check given entity in entry for walking onto new block.
    /// </summary>
    /// <param name="entry">Entry to check.</param>
    private void CheckEntityMove(EntityTrampleEntry entry)
    {
        if (!CanUseEntity(entry.Entity)) return;

        BlockPos currentPos = entry.Entity.Pos.AsBlockPos;
        if (entry.LastPos == null)
        {
            entry.LastPos = currentPos;
        }
        else if (!entry.LastPos.Equals(currentPos))
        {   // Now we know entity moved to different block.
            entry.LastPos = currentPos;
            trampleService.TrampleLogic(entry);
        }
    }

    /// <summary>
    /// Check if we can use entity for trample logic.
    /// We only want to detect players in survival mode who are on the ground.
    /// </summary>
    /// <param name="entity">The entity to check.</param>
    /// <returns>True if the entity can trample blocks.</returns>
    private static bool CanUseEntity(Entity entity)
    {
        if (!entity.OnGround) return false;

        IServerPlayer? player = EntityUtils.ResolvePlayer(entity);
        if (player == null) return true; // Not a player, skip rest of checks.

        if (player.ConnectionState != EnumClientState.Playing) return false;
        if (player.WorldData.CurrentGameMode != EnumGameMode.Survival) return false;

        return true;
    }
}
