using UnityEngine;

// Item.InventoryRelated
public partial class Item
{
    public StorableFootprint Footprint => storable.Footprint;
    public int Rotation => storable.Rotation;
    public StoredObject PlacedObject => storable.PlacedObject;
    public bool IsPlaced => storable.IsPlaced;

    public void Rotate()
    {
        bool rotatable = storable.RotateStorable();
        if (!rotatable)
        {
            return;
        }

        ApplySize(storableVisual);
        itemVisual.Rotate((RectTransform)this.transform);
    }

    public bool TryPlaceOrSnapToInventory()
    {
        return storable.PlaceItem();
    }

    public void RemoveFromInventory()
    {
        storable.RemoveFromInventory();
    }

    public void UpdateStorableVisual(bool show)
    {
        storable.UpdateStorableVisual(show);
    }

    public void ApplySize(StorableVisual storableVisual)
    {
        RectTransform rt = (RectTransform)transform;
        RectTransform sourceRt = (RectTransform)storableVisual.transform;
        rt.sizeDelta = sourceRt.sizeDelta;
    }

    public void OnDragStarted()
    {
        storableVisual.gameObject.SetActive(true);
    }

    public void OnDragEnded()
    {
        if (TryPlaceOrSnapToInventory())
        {
            progressTimer.Play(Data.triggerSpeed);
        }
        else
        {
            progressTimer.Stop();
        }
        UpdateStorableVisual(false);
        storableVisual.gameObject.SetActive(false);
    }
}