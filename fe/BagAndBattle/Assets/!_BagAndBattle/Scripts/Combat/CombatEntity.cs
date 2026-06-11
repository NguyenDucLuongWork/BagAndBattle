using UnityEngine;
using System.Collections.Generic;

public abstract class CombatEntity : MonoBehaviour
{
    // Đưa thuộc tính ra ngoài Inspector để bạn tự nhập số trong Unity
    [SerializeField] protected int maxHealth = 100;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; protected set; }
    public int Shield { get; protected set; }
    public List<StatusEffect> StatusEffects { get; private set; } = new List<StatusEffect>();

    public bool IsDead => CurrentHealth <= 0;

    // Thay thế constructor bằng hàm Virtual Start của Unity
    protected virtual void Start()
    {
        CurrentHealth = maxHealth;
        Shield = 0;
    }

    public virtual void TakeDamage(int damage)
    {
        int remainingDamage = damage;

        if (Shield > 0)
        {
            int absorbed = Mathf.Min(Shield, damage);
            Shield -= absorbed;
            remainingDamage -= absorbed;
        }

        CurrentHealth -= remainingDamage;

        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }

        Debug.Log($"{gameObject.name} nhận {damage} sát thương. Máu còn: {CurrentHealth}, Giáp còn: {Shield}");
    }

    public virtual void Heal(int amount)
    {
        if (IsDead) return;
        CurrentHealth += amount;
        if (CurrentHealth > maxHealth)
        {
            CurrentHealth = maxHealth;
        }
        Debug.Log($"{gameObject.name} được hồi {amount} máu. Máu hiện tại: {CurrentHealth}");
    }

    public virtual void AddShield(int amount)
    {
        if (IsDead) return;
        Shield += amount;
        Debug.Log($"{gameObject.name} nhận thêm {amount} giáp. Tổng giáp: {Shield}");
    }
}