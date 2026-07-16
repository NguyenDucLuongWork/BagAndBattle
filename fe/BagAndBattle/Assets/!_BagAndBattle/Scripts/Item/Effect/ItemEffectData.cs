using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemEffectDataSO", menuName = "Scriptable Objects/ItemEffectDataSO")]
public class ItemEffectDataSO : ScriptableObject
{
    public ItemEffectData itemEffectData;
}

[Serializable]
public class ItemEffectData
{
    public Sprite itemEffectSprite;

    public ItemIndividualEffect[] effects;
}

[Serializable]
public class ItemIndividualEffect
{
    public ItemEffectType itemEffectType;
    public float value;
}
