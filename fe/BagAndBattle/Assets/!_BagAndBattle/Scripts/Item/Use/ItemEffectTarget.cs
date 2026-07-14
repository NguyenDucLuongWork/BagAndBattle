using System;
using System.Collections.Generic;
using System.Text;

public interface IDamageable
{
    void TakeDamage(int amount);
}

public interface IHealable
{
    void Heal(int amount);
}

public interface IShieldable
{
    void AddShield(int amount);
}
