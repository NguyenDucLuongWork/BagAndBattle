using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class Item : MonoBehaviour
{
    public ItemDataHolderSO itemDataHolderSO;
    public int index;

    private ItemVisual itemVisual;
    private Storable storable;

    public ItemData Data
    {
        get
        {
            if (itemDataHolderSO != null && index >= 0 && index < itemDataHolderSO.itemDataList.Length)
                return itemDataHolderSO.itemDataList[index];
            return null;
        }
    }

    private void Awake()
    {
        itemVisual = GetComponentInChildren<ItemVisual>();
        storable = GetComponentInChildren<Storable>();
    }

    public void Start()
    {
        if (itemDataHolderSO == null || itemDataHolderSO.itemDataList.Length == 0)
        {
            Debug.LogWarning($"{name}: itemDataHolderSO is missing or empty.");
            return;
        }

        index = Random.Range(0, itemDataHolderSO.itemDataList.Length);
        Init();
    }

    [ContextMenu("Init")]
    public void Init()
    {
        if (itemDataHolderSO == null || index < 0 || index >= itemDataHolderSO.itemDataList.Length)
        {
            Debug.LogWarning($"{name}: index {index} out of range.");
            return;
        }

        ItemData itemData = itemDataHolderSO.itemDataList[index];

        if (itemVisual != null)
            itemVisual.Init(itemData);
        else
            Debug.LogWarning($"{name}: missing ItemVisual in children.");

        if (storable != null)
            storable.Init(itemData.footprint);
        else
            Debug.LogWarning($"{name}: missing Storable in children.");
    }

    public void DebugLogItem()
    {
        if (itemDataHolderSO == null || index < 0 || index >= itemDataHolderSO.itemDataList.Length)
        {
            Debug.LogWarning($"{name}: index {index} out of range.");
            return;
        }

        Debug.Log(itemDataHolderSO.itemDataList[index].sprite);
    }
}