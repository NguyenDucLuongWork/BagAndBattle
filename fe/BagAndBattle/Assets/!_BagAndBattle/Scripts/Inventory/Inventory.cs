using LgTyLib.Core;
using System;
using UnityEngine;

[RequireComponent(typeof(InventoryGridVisual))]
public class Inventory : BaseSingleton<Inventory>
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
    public InventoryGrid grid { get; private set; }

    public InventoryGridVisual inventoryGridVisual { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        grid = new InventoryGrid(startWidth, startHeight);

        if (startWithLockedRegion)
            LockRegionOutside(unlockedWidth, unlockedHeight);

        inventoryGridVisual = GetComponent<InventoryGridVisual>();
        inventoryGridVisual.ShowGrid(grid);

    }

    private void LockRegionOutside(int playableW, int playableH)
    {
        for (int x = 0; x < grid.width; x++)
            for (int y = 0; y < grid.height; y++)
                if (x >= playableW || y >= playableH)
                    grid.GetCell(x, y).state = CellState.Disable;
    }

    public StoredObject TryPlace(Storable storable, Vector2Int origin, int rotation)
    {
        var result = PlacementValidator.CanPlace(grid, storable, origin, rotation);
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

        grid.Place(obj);
        OnItemPlaced?.Invoke(obj);
        OnCellsChanged?.Invoke();
        return obj;
    }

    public bool TryMove(StoredObject obj, Vector2Int newOrigin, int newRotation)
    {
        var result = PlacementValidator.CanPlace(grid, obj.storable, newOrigin, newRotation,
            ignore: obj);

        if (!result.Success)
        {
            Debug.Log($"[InventoryGridMono] TryMove failed: {result}");
            return false;
        }

        grid.Remove(obj);
        obj.origin = newOrigin;
        obj.rotation = newRotation;
        grid.Place(obj);

        OnCellsChanged?.Invoke();
        return true;
    }
    public void Remove(StoredObject obj)
    {
        grid.Remove(obj);
        OnItemRemoved?.Invoke(obj);
        OnCellsChanged?.Invoke();
    }

    internal void RaiseCellsChanged() => OnCellsChanged?.Invoke();

    public InventoryGrid GetSaveData() => grid;

    public void LoadFrom(InventoryGrid savedGrid)
    {
        grid = savedGrid;
        grid.RebuildCells();
        OnCellsChanged?.Invoke();
    }

    public StoredObject GetOccupant(Vector2Int cell) =>
        grid.InBounds(cell) ? grid.GetOccupant(cell.x, cell.y) : null;

    public bool IsCellFree(Vector2Int cell) =>
        grid.InBounds(cell) && grid.GetCell(cell).state == CellState.Empty;
}