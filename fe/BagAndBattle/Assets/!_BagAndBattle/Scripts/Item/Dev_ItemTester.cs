using UnityEngine;

public class Dev_ItemTester : MonoBehaviour
{
    public ItemDataHolderSO itemDataHolderSO;

    public ItemVisual itemUIVisual;

    private void Start()
    {
        UpdateItemVisualWithRandomItem();
    }

    [ContextMenu("UpdateItemVisualWithRandomItem")]
    public void UpdateItemVisualWithRandomItem()
    {
        ItemData[] items = itemDataHolderSO.itemDataList;

        int index = Random.Range(0, items.Length);

        itemUIVisual.Init((RectTransform)this.transform, items[index]);
    }
}