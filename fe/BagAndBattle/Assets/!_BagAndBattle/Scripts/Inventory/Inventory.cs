using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    [Header("Initial size")]
    [SerializeField] private int startWidth = 5;
    [SerializeField] private int startHeight = 5;

    [Header("Lock initial cells? (for upgrade-gated grids)")]
    [SerializeField] private bool startWithLockedRegion = false;
    [SerializeField] private int unlockedWidth = 3;
    [SerializeField] private int unlockedHeight = 3;

    public event Action OnCellsChanged;

    public event Action<StoredObject> OnItemPlaced;

    public event Action<StoredObject> OnItemRemoved;

    [field:SerializeField]
    public InventoryGrid Grid { get; private set; }

    private void Awake()
    {
        Grid = new InventoryGrid(startWidth, startHeight);

        if (startWithLockedRegion)
            LockRegionOutside(unlockedWidth, unlockedHeight);
    }

    private void LockRegionOutside(int playableW, int playableH)
    {
        for (int x = 0; x < Grid.width; x++)
            for (int y = 0; y < Grid.height; y++)
                if (x >= playableW || y >= playableH)
                    Grid.GetCell(x, y).state = CellState.Locked;
    }

    public StoredObject TryPlace(Storable storable, Vector2Int origin, int rotation)
    {
        var result = PlacementValidator.CanPlace(Grid, storable, origin, rotation);
        if (!result.Success)
        {
            Debug.Log($"[InventoryGridMono] TryPlace failed: {result}");
            return null;
        }

        var obj = new StoredObject
        {
            storable = storable,
            origin = origin,
            rotation = rotation
        };

        Grid.Place(obj);
        OnItemPlaced?.Invoke(obj);
        OnCellsChanged?.Invoke();
        return obj;
    }

    public bool TryMove(StoredObject obj, Vector2Int newOrigin, int newRotation)
    {
        var result = PlacementValidator.CanPlace(Grid, obj.storable, newOrigin, newRotation,
            ignore: obj);

        if (!result.Success)
        {
            Debug.Log($"[InventoryGridMono] TryMove failed: {result}");
            return false;
        }

        Grid.Remove(obj);
        obj.origin = newOrigin;
        obj.rotation = newRotation;
        Grid.Place(obj);

        OnCellsChanged?.Invoke();
        return true;
    }
    public void Remove(StoredObject obj)
    {
        Grid.Remove(obj);
        OnItemRemoved?.Invoke(obj);
        OnCellsChanged?.Invoke();
    }

    internal void RaiseCellsChanged() => OnCellsChanged?.Invoke();

    public InventoryGrid GetSaveData() => Grid;

    public void LoadFrom(InventoryGrid savedGrid)
    {
        Grid = savedGrid;
        Grid.RebuildCells();
        OnCellsChanged?.Invoke();
    }

    public StoredObject GetOccupant(Vector2Int cell) =>
        Grid.InBounds(cell) ? Grid.GetOccupant(cell.x, cell.y) : null;

    public bool IsCellFree(Vector2Int cell) =>
        Grid.InBounds(cell) && Grid.GetCell(cell).state == CellState.Empty;
}