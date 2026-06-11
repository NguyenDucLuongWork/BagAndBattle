using UnityEngine;

public class MonsterEntity : CombatEntity
{
    public string monsterName;

 
    protected override void Start()
    {
        base.Start(); // Gọi logic đặt máu của lớp cha CombatEntity
        if (string.IsNullOrEmpty(monsterName)) monsterName = gameObject.name;
    }
}