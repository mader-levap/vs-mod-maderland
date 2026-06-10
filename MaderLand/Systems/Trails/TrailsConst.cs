using System;
using System.Collections.Generic;
using System.Text;

namespace MaderLand.Systems.Trails;

/// <summary>
/// Constants for Trails feature.
/// </summary>
public static class TrailsConst
{
    /// <summary>
    /// Empty block code. Used when block should be removed after trampling.
    /// </summary>
    public const string emptyBlock = "game:air";

    /// <summary>
    /// The unique data key used to store trample data in chunk mod data.
    /// </summary>
    public const string trampleDataKey = "maderland:trampledata";
}
