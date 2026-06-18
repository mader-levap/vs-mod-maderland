using MaderLand.Systems.Trample;
using System.Collections.Generic;

namespace MaderLand.Config.Trample;

/// <summary>
/// Defines shape of config file maderland/trample.json.
/// </summary>
public class TrampleCfg
{
    /// <summary>
    /// Is Trample feature active?
    /// </summary>
    public bool Active { get; set; } = true;

    /// <summary>
    /// Is trampling allowed?
    /// </summary>
    public bool Allow { get; set; } = true;

    /// <summary>
    /// Is debug mode active?
    /// </summary>
    public bool Debug { get; set; } = false;

    /// <summary>
    /// Who can trample and how?
    /// </summary>
    public List<TrampleEntityCfg> Entities { get; set; } = [];

    /// <summary>
    /// Passable blocks are blocks that can be walked through and being above impassable block. Grass, bush, flowers, etc.
    /// If there is something at passable block, it will be affected by trampling instead of impassable block below it.
    /// </summary>
    public TrampleGroupCfg Passable { get; set; } = new();

    /// <summary>
    /// Impassable blocks are blocks that you walk on, will be affected by trampling if nothing is above it. Solid blocks like soil, etc.
    /// </summary>
    public TrampleGroupCfg Impassable { get; set; } = new();
}
