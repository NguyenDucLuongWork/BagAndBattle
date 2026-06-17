using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class Item : MonoBehaviour
{
    public ItemDataHolderSO itemDataHolderSO;
    public int index;

    public void Start()
    {
        index = Random.Range(0, itemDataHolderSO.itemDataList.Length);
        Init();
    }
    [ContextMenu("Init")]
    public void Init()
    {
        if (index >= itemDataHolderSO.itemDataList.Length)
        {
            Debug.LogWarning("Index item out of range");
        }
        ItemData itemData = itemDataHolderSO.itemDataList[index];

        ItemVisual itemVisual= GetComponentInChildren<ItemVisual>();
        itemVisual.Init(itemData);

        Storable storable = GetComponentInChildren<Storable>();
        storable.Init(itemData.footprint);
    }

    public void DebugLogItem()
    {
        if (index >= itemDataHolderSO.itemDataList.Length) return;

        Debug.Log(itemDataHolderSO.itemDataList[index].sprite);
    }
}
