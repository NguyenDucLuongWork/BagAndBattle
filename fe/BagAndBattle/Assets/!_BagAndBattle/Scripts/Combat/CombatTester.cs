using UnityEngine;
using UnityEngine.InputSystem; 

public class CombatTester : MonoBehaviour
{
    public PlayerEntity player;
    public MonsterEntity monster;

    void Update()
    {
   

        // 1. Nhấn nút S: Player đánh Monster
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            if (monster != null) monster.TakeDamage(15);
        }

        // 2. Nhấn nút D: Monster đánh Player
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            if (player != null) player.TakeDamage(10);
        }

        // 3. Nhấn nút H: Hồi máu cho Player
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            if (player != null) player.Heal(20);
        }
    }
}