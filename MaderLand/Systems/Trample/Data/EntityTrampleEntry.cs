using MaderLand.Systems.Trample.Config;
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
    /// Name of entity. For players it will be player name, otherwise general name of entity.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Last block position of entity.
    /// </summary>
    public BlockPos LastPos { get; set; } = null!;

    /// <summary>
    /// Last state of OnGround.
    /// </summary>
    public bool LastOnGround { get; set; } = true;

    /// <summary>
    /// Y where fall started.
    /// </summary>
    public double FallStartY { get; set; } = 0d;

    /// <summary>
    /// Strength of impact. Base for calculating bonus trampling power.
    /// </summary>
    public float ImpactStrength { get; set; } = 0f;


    /// <summary>
    /// Trample configuration entry for this entity.
    /// </summary>
    public TrampleEntityCfg TrampleEntityCfg { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="Entity">Entity.</param>
    /// <param name="Name">Name of entity. Shown in debug.</param>
    /// <param name="TrampleEntityCfg">Trample configuration entry for this entity.</param>
    public EntityTrampleEntry(Entity Entity, string Name, TrampleEntityCfg TrampleEntityCfg)
    {
        this.Entity = Entity;
        this.Name = Name;
        this.TrampleEntityCfg = TrampleEntityCfg;
    }
}
