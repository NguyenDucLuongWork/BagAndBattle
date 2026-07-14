using System;
using System.Collections.Generic;
using System.Text;

public class ItemUseContext
{
    public ItemData ItemData { get; }
    public string SourceName { get; }

    public IDamageable DamageTarget { get; }
    public IHealable HealTarget { get; }
    public IShieldable ShieldTarget { get; }

    public ItemUseContext(
        ItemData itemData,
        string sourceName,
        IDamageable damageTarget = null,
        IHealable healTarget = null,
        IShieldable shieldTarget = null)
    {
        ItemData = itemData;
        SourceName = sourceName;
        DamageTarget = damageTarget;
        HealTarget = healTarget;
        ShieldTarget = shieldTarget;
    }
}