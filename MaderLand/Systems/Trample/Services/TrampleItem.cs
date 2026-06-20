using MaderLand.Systems.Trample.Config;
using MaderLand.Systems.Trample.Data;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace MaderLand.Systems.Trample.Services;

/// <summary>
/// Handle items in context of trampling.
/// </summary>
public class TrampleItem(ICoreServerAPI sapi)
{
    /// <summary>
    /// Cache of item configurations. It also contains items without config.
    /// </summary>
    private readonly Dictionary<string, TrampleItemCfg?> itemCfgs = [];

    /// <summary>
    /// Checks inventory of entity (if it has one) and determine multiplier to base trampling power.
    /// </summary>
    /// <param name="entry">Trampleable entity data.</param>
    /// <returns>Multiplier for base trampling power.</returns>
    public float ResolveEquipmentMul(EntityTrampleEntry entry)
    {
        EntityBehaviorTexturedClothing? invBh = entry.Entity.GetBehavior<EntityBehaviorTexturedClothing>();
        if (invBh?.Inventory == null) return 1;

        float EquipMul = 1;
        foreach (ItemSlot slot in invBh.Inventory)
        {
            EquipMul += ResolveSingleItem(slot);
        }
        return EquipMul;
    }

    /// <summary>
    /// Resolve multiplier for single item.
    /// </summary>
    /// <param name="slot">Item slot in inventory to check.</param>
    /// <returns>Equipment multiplier.</returns>
    private float ResolveSingleItem(ItemSlot slot)
    {
        if (slot.Empty) return 0;

        ItemStack? itemStack = slot.Itemstack;
        if (itemStack == null) return 0;
        float condition = itemStack.Attributes.GetFloat("condition", 1);
        if (condition == 0) return 0; // Completely worn out items do not count.

        CollectibleObject? item = itemStack.Collectible;
        if (item == null) return 0;
        //string message3 = $"[Trample] |- Found slot: item '{item.Code}' exists. Item: Condition={condition}.";
        //sapi.Logger.Notification(message3); // DEBUG

        TrampleItemCfg? itemCfg = ResolveItemConfig(item);
        if (itemCfg == null) return 0;

        //string message = $"[Trample] |- ITEM '{item.Code}' IS IN CONFIG. Item: Condition={condition}, Dur={item.GetRemainingDurability(itemStack)}/{item.Durability}, Power={itemCfg.Power}, Damaged={itemCfg.Damaged}.";
        //sapi.Logger.Notification(message); // DEBUG

        float powerMul = itemCfg.Power;
        if (!itemCfg.Damaged) return powerMul;
        return powerMul * condition;
    }

    /// <summary>
    /// Resolve item config using cache first.
    /// </summary>
    /// <param name="item">Item.</param>
    /// <returns>Item configuration for trampling.</returns>
    private TrampleItemCfg? ResolveItemConfig(CollectibleObject item)
    {
        if (itemCfgs.ContainsKey(item.Code)) return itemCfgs[item.Code]; // Already in cache.
        TrampleItemCfg? itemCfg = TrampleUtils.GetItemConfig(item);
        itemCfgs.TryAdd(item.Code, itemCfg);
        return itemCfg;
    }
}
