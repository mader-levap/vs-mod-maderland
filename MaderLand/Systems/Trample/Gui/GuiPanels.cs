using Vintagestory.API.Client;

namespace MaderLand.Systems.Trample.Gui;

/// <summary>
/// Manages GUI panels for Trample feature.
/// </summary>
public class GuiPanels
{
    /// <summary>
    /// Debug panel instance.
    /// </summary>
    private static GuiDebugPanel? Debug;

    /// <summary>
    /// Resolve debug panel.
    /// </summary>
    /// <param name="api">Core client API.</param>
    /// <returns>Initialized debug panel.</returns>
    public static GuiDebugPanel GetDebug(ICoreClientAPI api) {
        if (Debug == null) Debug = new GuiDebugPanel(api);
        return Debug;
    }
}
