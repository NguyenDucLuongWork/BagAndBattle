using System.Collections.Generic;
using UnityEngine;


public class ItemUser : MonoBehaviour
{
    private readonly List<IItemUseStrategy> strategies = new List<IItemUseStrategy>
    {
        new DamageUseStrategy(),
        new HealUseStrategy(),
        new ShieldUseStrategy(),
    };

    public void UseItem(ItemUseContext context)
    {
        if (context?.ItemData == null)
        {
            Debug.LogWarning("[ItemUser] Invalid context or missing ItemData.");
            return;
        }

        bool executed = false;
        foreach (var strategy in strategies)
        {
            if (strategy.CanExecute(context.ItemData))
            {
                strategy.Execute(context);
                executed = true;
            }
        }

        if (!executed)
        {
            Debug.LogWarning($"[ItemUser] Item '{context.ItemData.name}' has no active effect (all stats 0).");
        }
    }
}