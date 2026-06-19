using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemVisual : MonoBehaviour
{
    [SerializeField]
    private ItemData itemData;

    [SerializeField]
    private Image image;


    public void Init(ItemData itemData)
    {
        this.itemData = itemData;
        
        UpdateVisual();
    }


    [ContextMenu("UpdateVisual")]
    public void UpdateVisual()
    {
        image = GetComponent<Image>();
        image.sprite = itemData.sprite;
    }


}