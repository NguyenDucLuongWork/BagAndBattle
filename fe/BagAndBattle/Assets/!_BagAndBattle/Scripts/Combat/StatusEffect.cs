using UnityEngine;

public abstract class StatusEffect
{
    public string Name { get; protected set; }
    public int Duration { get; protected set; }

    public abstract void ApplyEffect(CombatEntity entity);
}