using System;
using System.Collections.Generic;
using System.Text;

namespace MaderLand.Systems.Trample.Config;

/// <summary>
/// Describes single block variant affected by trampling.
/// </summary>
public class TrampleBlockVariantCfg : TrampleBlockCfg
{
    /// <summary>
    /// Source block code.
    /// </summary>
    public string FromBlockCode { get; set; } = "";
}
