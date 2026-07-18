using System.Collections.Generic;
using UnityEngine;

public static class ItemEffectStrategyFactory
{
    private static readonly Dictionary<ItemEffectType, IItemEffectStrategy> strategies =
        new Dictionary<ItemEffectType, IItemEffectStrategy>
    {
        { ItemEffectType.Health,      new HealthEffectStrategy() },
        { ItemEffectType.Attack,      new AttackEffectStrategy() },
        { ItemEffectType.Shield,      new ShieldEffectStrategy() },
        //{ ItemEffectType.Burn,        new BurnEffectStrategy() },
        //{ ItemEffectType.Poison,      new PoisonEffectStrategy() },
        { ItemEffectType.Lifesteal,   new LifestealEffectStrategy() },
        //{ ItemEffectType.Stun,        new StunEffectStrategy() },
        //{ ItemEffectType.Knockback,   new KnockbackEffectStrategy() },
        //{ ItemEffectType.ManaRestore, new ManaRestoreEffectStrategy() },
    };

    public static IItemEffectStrategy Get(ItemEffectType type)
    {
        if (strategies.TryGetValue(type, out var strategy)) return strategy;
        Debug.LogWarning($"[ItemEffectStrategyFactory] No strategy for {type}");
        return null;
    }
}