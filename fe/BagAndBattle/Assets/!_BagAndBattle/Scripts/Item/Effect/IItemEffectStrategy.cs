public interface IItemEffectStrategy
{
    void Execute(EffectContext context);
}

public class HealthEffectStrategy : IItemEffectStrategy
{
    public void Execute(EffectContext context) => context.Self?.Heal((int)context.Value);
}

public class AttackEffectStrategy : IItemEffectStrategy
{
    public void Execute(EffectContext context) => context.Enemy?.TakeDamage((int)context.Value);
}

public class ShieldEffectStrategy : IItemEffectStrategy
{
    public void Execute(EffectContext context) => context.Self?.AddShield((int)context.Value);
}

public class LifestealEffectStrategy : IItemEffectStrategy
{
    public void Execute(EffectContext context)
    {
        int amount = (int)context.Value;
        context.Enemy?.TakeDamage(amount);
        context.Self?.Heal(amount); // change to amount/2 etc. for a lifesteal %
    }
}

//public class BurnEffectStrategy : IItemEffectStrategy
//{
//    private const int Ticks = 3;
//    private const float Interval = 1f;
//    public void Execute(EffectContext context) =>
//        context.Enemy?.ApplyDamageOverTime((int)context.Value, Ticks, Interval, "Burn");
//}

//public class PoisonEffectStrategy : IItemEffectStrategy
//{
//    private const int Ticks = 5;
//    private const float Interval = 1.5f;
//    public void Execute(EffectContext context) =>
//        context.Enemy?.ApplyDamageOverTime((int)context.Value, Ticks, Interval, "Poison");
//}

//public class StunEffectStrategy : IItemEffectStrategy
//{
//    public void Execute(EffectContext context) => context.Enemy?.ApplyStun(context.Value);
//}

//public class KnockbackEffectStrategy : IItemEffectStrategy
//{
//    // No positioning in an auto-battler, so interpret it as "delays enemy's next trigger".
//    // Swap this for real physics/position logic if you add one.
//    public void Execute(EffectContext context) => context.Enemy?.ApplyStun(context.Value * 0.5f);
//}

//public class ManaRestoreEffectStrategy : IItemEffectStrategy
//{
//    public void Execute(EffectContext context)
//    {
//        // TODO: hook up once you add a Mana stat to CombatEntity
//        Debug.Log($"{context.Self?.name} restores {context.Value} mana.");
//    }
//}