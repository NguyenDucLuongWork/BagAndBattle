using System;
using UnityEngine;

[Serializable]
public class ItemData
{
    public string id;
    public string name;
    public string description;
    public Sprite sprite;
    public StorableFootprint footprint;

    [Header("Combat Stats")]
    public float triggerSpeed = 1f;
    public int damage = 0;
    public int heal = 0;
    public int shield = 0;
}