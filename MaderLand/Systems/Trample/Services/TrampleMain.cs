using MaderLand.Common.Config;
using MaderLand.Systems.Trample.Config;
using MaderLand.Systems.Trample.Data;
using MaderLand.Utils;
using System.Collections.Concurrent;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace MaderLand.Systems.Trample.Services;

/// <summary>
/// Main trample code.
/// </summary>
public class TrampleMain
{
    readonly ICoreServerAPI sapi;
    readonly TrampleService trampleService;

    /// <summary>
    /// List of trampleable entities. Adding and removing entities is called via events separately.
    /// </summary>
    private readonly ConcurrentDictionary<long, EntityTrampleEntry> entityEntries = [];

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

        TrampleEntityCfg? trampleEntityCfg = TrampleUtils.GetEntityCfg(entity);
        if (trampleEntityCfg == null) return; // Will not add this entity - it does not appear in configuration.

        EntityTrampleEntry newEntry = Create(entity, trampleEntityCfg);
        entityEntries.TryAdd(entity.EntityId, newEntry);
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
        entityEntries.TryRemove(entity.EntityId, out _);
    }

    // ////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Process entity movement and perform trampling logic if they walked onto a new block.
    /// </summary>
    /// <param name="dt">Delta time.</param>
    public void ProcessTramplingEntities(float _)
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

        HandleFall(entry);

        if (!entry.Entity.OnGround) return; // Any trampling happens only when we touch ground.

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
    /// Check if we can use entity for trample logic. If player, allow only players in survival mode.
    /// </summary>
    /// <param name="entity">The entity to check.</param>
    /// <returns>True if the entity can trample blocks.</returns>
    private static bool CanUseEntity(Entity entity)
    {
        IServerPlayer? player = EntityUtils.ResolvePlayer(entity);
        if (player == null) return true; // Not a player, skip rest of checks.

        if (player.ConnectionState != EnumClientState.Playing) return false;
        if (player.WorldData.CurrentGameMode != EnumGameMode.Survival) return false;
        return true;
    }

    //

    /// <summary>
    /// Handle falling.
    /// </summary>
    /// <param name="entry">Entry to handle.</param>
    private static void HandleFall(EntityTrampleEntry entry)
    {
        double DiffY = 0;
        if (entry.LastOnGround && !entry.Entity.OnGround) entry.FallStartY = entry.Entity.Pos.Y; // started falling
        if (!entry.LastOnGround && entry.Entity.OnGround) DiffY = entry.FallStartY - entry.Entity.Pos.Y; // stopped falling
        entry.LastOnGround = entry.Entity.OnGround;
        entry.ImpactStrength = CalcImpactStrength(DiffY);
    }

    /// <summary>
    /// Calculate impact strength, taking in account fall configuration.
    /// </summary>
    /// <param name="FallY">Difference between Y position where entity start falling and stop falling.</param>
    /// <returns>Impact strength.</returns>
    private static float CalcImpactStrength(double FallY)
    {
        if (FallY <= 0) return 0; // That was not a fall.

        TrampleFallCfg FallCfg = ConfigService.TrampleConfig.Fall;
        double EffectiveFallY = FallY;

        // Enforce cap height.
        double CapHeight = FallCfg.CapHeight;
        if (CapHeight > 0 && EffectiveFallY > CapHeight) EffectiveFallY = CapHeight;

        // Enforce minimum height.
        EffectiveFallY -= FallCfg.MinimumHeight;

        if (EffectiveFallY <= 0) return 0;
        return (float)(EffectiveFallY * FallCfg.Mul);
    }
}
