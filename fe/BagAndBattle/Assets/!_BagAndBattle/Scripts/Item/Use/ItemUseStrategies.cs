using UnityEngine;

public interface IItemUseStrategy
{
    bool CanExecute(ItemData data);
    void Execute(ItemUseContext context);
}

public class DamageUseStrategy : IItemUseStrategy
{
    public bool CanExecute(ItemData data) => data != null && data.damage > 0;

    public void Execute(ItemUseContext context)
    {
        context.DamageTarget?.TakeDamage(context.ItemData.damage);
        Debug.Log($"[ItemUse] {context.SourceName} dealt {context.ItemData.damage} damage.");
    }
}

public class HealUseStrategy : IItemUseStrategy
{
    public bool CanExecute(ItemData data) => data != null && data.heal > 0;

    public void Execute(ItemUseContext context)
    {
        context.HealTarget?.Heal(context.ItemData.heal);
        Debug.Log($"[ItemUse] {context.SourceName} healed {context.ItemData.heal}.");
    }
}

public class ShieldUseStrategy : IItemUseStrategy
{
    public bool CanExecute(ItemData data) => data != null && data.shield > 0;

    public void Execute(ItemUseContext context)
    {
        context.ShieldTarget?.AddShield(context.ItemData.shield);
        Debug.Log($"[ItemUse] {context.SourceName} granted {context.ItemData.shield} shield.");
    }
}