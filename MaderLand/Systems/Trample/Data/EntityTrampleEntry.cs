using MaderLand.Systems.Trample.Config;
using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace MaderLand.Systems.Trample.Data;

/// <summary>
/// Data about single entity that can trample blocks.
/// </summary>
public class EntityTrampleEntry
{
    /// <summary>
    /// Entity.
    /// </summary>
    public Entity Entity { get; }

    /// <summary>
    /// Name of entity. For players it will be player name, otherwise EntityId.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Last position of entity.
    /// </summary>
    public BlockPos LastPos { get; set; } = null!;

    /// <summary>
    /// Trample configuration entry for this entity.
    /// </summary>
    public TrampleEntityCfg TrampleEntityCfg { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="Entity">Entity.</param>
    /// <param name="TrampleEntityCfg">Trample configuration entry for this entity.</param>
    public EntityTrampleEntry(Entity Entity, string Name, TrampleEntityCfg TrampleEntityCfg)
    {
        this.Entity = Entity;
        this.Name = Name;
        this.TrampleEntityCfg = TrampleEntityCfg;
    }
}
