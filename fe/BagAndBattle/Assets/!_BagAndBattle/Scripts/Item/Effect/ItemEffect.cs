using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ItemEffect : MonoBehaviour
{
    private ItemEffectData effectData;

    public void SetData(ItemEffectData itemEffectData)
    {
        this.effectData = itemEffectData;
    }

    public void Use()
    {
        Debug.Log(effectData.effects[0].itemEffectType);
    }
}