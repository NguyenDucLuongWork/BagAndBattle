using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Storable : MonoBehaviour
{
    public StorableFootprint footprint;

    [SerializeField] private int rotation = 0;

    private StorableVisual visual;
    private RectTransform rectTransform;
    public StoredObject PlacedObject { get; private set; }

    public bool IsPlaced => PlacedObject != null;

    private void Awake()
    {
        visual = GetComponentInChildren<StorableVisual>();
        rectTransform = GetComponent<RectTransform>();
        visual.ShowStorableFootprint(footprint);
    }

    public CellState GetCellState(int col, int row)
    {
        return footprint[col, row] ? CellState.Required : CellState.Disable;
    }

    [ContextMenu("Init")]
    public void Init(StorableFootprint footprint)
    {
        this.footprint = footprint;
        UpdateVisual();
    }

    [ContextMenu("UpdateVisual")]
    public void UpdateVisual()
    {
        visual.ShowStorableFootprint(footprint);
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
        if (PlaceItem())

            return;
        
    }

    public bool PlaceItem()
    {
        var nearestCell = GetNearestCell();
        if (nearestCell == null)
        {
            Debug.LogWarning($"{name}: PlaceItem failed, no nearest cell found.");
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
            Debug.Log($"{name}: PlaceItem failed at {origin} (rotation {rotation}).");
        }

        return result;
    }

    public void RemoveFromInventory()
    {
        if (PlacedObject == null) return;

        Inventory.Instance.Remove(PlacedObject);
        PlacedObject = null;
    }

    public void RotateStorable(int steps = 1)
    {
        rotation = ((rotation + steps) % 4 + 4) % 4;
        footprint.RotateSelf(steps);
        UpdateVisual();
    }
}