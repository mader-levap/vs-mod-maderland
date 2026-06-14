using Vintagestory.API.Common;

namespace MaderLand.Systems.Manager;

/// <summary>
/// Specifies a contract for classes that can initialize features.
/// </summary>
public interface IInitializer
{
    /// <summary>
    /// Stuff to do when registering the feature. It can be used to set up any necessary data structures, register block behaviors, etc.
    /// </summary>
    /// <param name="api">Core API.</param>
    static abstract void Init(ICoreAPI api);
}
