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

    public bool IsLocked { get; set; } = false;

    [field: SerializeField]
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

        OnCellsChanged += () => inventoryGridVisual.RefreshGrid(grid);
    }

    private void LockRegionOutside(int playableW, int playableH)
    {
        for (int x = 0; x < grid.width; x++)
            for (int y = 0; y < grid.height; y++)
                if (x >= playableW || y >= playableH)
                    grid.GetCell(x, y).state = CellState.Disable;
    }

    public bool TryPlaceOrMove(Storable storable, Vector2Int origin, int rotation, StoredObject existing, out StoredObject result)
    {
        if (IsLocked)
        {
            Debug.Log("[Inventory] Locked! Cannot place or move items during combat.");
            result = null;
            return false;
        }

        var validation = PlacementValidator.CanPlace(grid, storable, origin, rotation, ignore: existing);

        if (!validation.Success)
        {
            //Debug.Log($"[Inventory] TryPlaceOrMove failed: {validation}");
            grid.ReconcileCells();
            OnCellsChanged?.Invoke();
            result = null;
            return false;
        }

        bool isMove = existing != null;

        if (isMove)
            grid.Remove(existing);

        var obj = existing ?? new StoredObject { storable = storable };
        obj.origin = origin;
        obj.rotation = rotation;

        grid.Place(obj);
        result = obj;

        if (!isMove)
            OnItemPlaced?.Invoke(obj);

        OnCellsChanged?.Invoke();
        //Debug.Log("CallingOnCellChanged");
        return true;
    }

    public bool TryPlace(Storable storable, Vector2Int origin, int rotation, out StoredObject placed) =>
        TryPlaceOrMove(storable, origin, rotation, null, out placed);

    public bool TryMove(StoredObject obj, Vector2Int newOrigin, int newRotation) =>
        TryPlaceOrMove(obj.storable, newOrigin, newRotation, obj, out _);

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

    public void SetCellState(Vector2Int cell, CellState state, StoredObject occupant = null)
    {
        if (!grid.InBounds(cell))
        {
            Debug.LogWarning($"[Inventory] SetCellState failed: {cell} is out of bounds.");
            return;
        }

        grid.SetCellState(cell, state, occupant);
        OnCellsChanged?.Invoke();
    }

}