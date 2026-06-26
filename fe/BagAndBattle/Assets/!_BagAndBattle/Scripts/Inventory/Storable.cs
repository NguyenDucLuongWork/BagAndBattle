using System;
using UnityEngine;

public class Storable : MonoBehaviour
{
    [SerializeField] private StorableFootprint footprint;
    [SerializeField] private RectTransform itemHolder;

    public StorableFootprint Footprint => footprint;

    [SerializeField] private int rotation = 0;
    public int Rotation => rotation;

    private StorableVisual visual;
    private RectTransform rectTransform;
    public StoredObject PlacedObject { get; private set; }

    public bool IsPlaced => PlacedObject != null;

    private void Awake()
    {
        visual = GetComponentInChildren<StorableVisual>();
        rectTransform = GetComponent<RectTransform>();
        visual.ShowStorableFootprint(footprint.Rotated(rotation));
    }

    public CellState GetCellState(int col, int row)
    {
        var rotated = footprint.Rotated(rotation);
        return rotated[col, row] ? CellState.Required : CellState.Disable;
    }

    [ContextMenu("Init")]
    public void Init(StorableFootprint footprint)
    {
        this.footprint = footprint;
        rotation = 0;

        if (itemHolder == null)
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();

            itemHolder = rectTransform.parent as RectTransform;
        }

        UpdateVisual();
    }

    [ContextMenu("UpdateVisual")]
    public void UpdateVisual()
    {
        visual.ShowStorableFootprint(footprint.Rotated(rotation));
    }

    public void SnapToCell(RectTransform cellRectTransform)
    {
        if (cellRectTransform == null) return;

        rectTransform.SetParent(cellRectTransform, false);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    public RectTransform GetCellItemSlotTransform()
    {
        var cell = GetNearestCell();
        return cell?.rectTransform;
    }

    public InventoryCell GetNearestCell()
    {
        if (Inventory.Instance == null || Inventory.Instance.grid == null)
        {
            Debug.LogWarning($"{name}: Inventory or grid is not available.");
            return null;
        }

        var cells = Inventory.Instance.grid.cells;

        InventoryCell nearestCell = null;
        float nearestSqrDistance = float.MaxValue;

        Vector3 itemOrigin = GetWorldBottomLeft(rectTransform);

        foreach (InventoryCell cell in cells)
        {
            if (cell.rectTransform == null) continue;

            Vector3 cellOrigin = GetWorldBottomLeft(cell.rectTransform);
            float sqrDistance = (cellOrigin - itemOrigin).sqrMagnitude;

            if (sqrDistance < nearestSqrDistance)
            {
                nearestSqrDistance = sqrDistance;
                nearestCell = cell;
            }
        }

        return nearestCell;
    }

    private static Vector3 GetWorldBottomLeft(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return corners[0];
    }

    public void UpdateStorableVisual(bool show)
    {
        visual.gameObject.SetActive(show);
    }

    public void TryPlaceOrSnapToInventory()
    {
        PlaceItem();
    }

    public bool PlaceItem()
    {
        if (InteractionManager.Instance.IsPointerOverItemHolder())
        {
            Debug.Log("Is over item holder");
            ReturnToItemHolder();
            return false;
        }

        var nearestCell = GetNearestCell();
        if (nearestCell == null)
        {
            Debug.LogWarning($"{name}: PlaceItem failed, no nearest cell found.");
            SnapBackOnFailure();
            return false;
        }

        Vector2Int origin = nearestCell.coord;

        bool result = Inventory.Instance.TryPlaceOrMove(
            this, origin, rotation, PlacedObject, out StoredObject placed);

        if (result)
        {
            PlacedObject = placed;
            SnapToCell(nearestCell.rectTransform);
        }
        else
        {
            //Debug.Log($"{name}: PlaceItem failed at {origin} (rotation {rotation}).");
            SnapBackOnFailure();
        }

        return result;
    }


    private void ReturnToItemHolder()
    {
        if (PlacedObject != null)
            RemoveFromInventory();

        SnapToCell(itemHolder);
    }

    private void SnapBackOnFailure()
    {
        if (PlacedObject != null)
        {
            var cell = Inventory.Instance.grid.GetCell(PlacedObject.origin);
            SnapToCell(cell.rectTransform);
        }
        else
        {
            SnapToCell(itemHolder);
        }
    }
    public void RemoveFromInventory()
    {
        if (PlacedObject == null) return;

        Inventory.Instance.Remove(PlacedObject);
        PlacedObject = null;
    }

    public bool RotateStorable(int steps = 1)
    {
        int newRotation = ((rotation + steps) % 4 + 4) % 4;

        if (PlacedObject != null)
        {
            bool moved = Inventory.Instance.TryMove(PlacedObject, PlacedObject.origin, newRotation);
            if (!moved)
            {
                Debug.Log($"{name}: RotateStorable blocked, no room to rotate at {PlacedObject.origin}.");
                return false;
            }
        }

        rotation = newRotation;
        UpdateVisual();
        return true;
    }
}