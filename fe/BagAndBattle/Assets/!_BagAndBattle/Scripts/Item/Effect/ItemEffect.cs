using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public class ItemEffect : MonoBehaviour
{
    [field:SerializeField]
    private ItemEffectData effectData;

    public void SetData(ItemEffectData itemEffectData) => effectData = itemEffectData;

    public void Use()
    {
        CombatEntity self = CombatManager.Instance.player;
        CombatEntity enemy = CombatManager.Instance.monster;
        if (effectData?.effects == null) return;

        foreach (var effect in effectData.effects)
        {
            var strategy = ItemEffectStrategyFactory.Get(effect.itemEffectType);
            if (strategy == null) continue;

            strategy.Execute(new EffectContext
            {
                Self = self,
                Enemy = enemy,
                Value = effect.value,
                VisualInstance = gameObject
            });
        }
    }
}