using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryGrid
{
    public int width;
    public int height;

    [field: SerializeField]
    public InventoryCell[] cells { get; private set; }

    public List<StoredObject> stored = new List<StoredObject>();

    public InventoryGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
        BuildCells();
    }

    private int Index(int x, int y) => y * width + x;
    private int Index(Vector2Int v) => Index(v.x, v.y);

    public bool InBounds(int x, int y) =>
        x >= 0 && x < width && y >= 0 && y < height;

    public bool InBounds(Vector2Int v) => InBounds(v.x, v.y);

    public ref InventoryCell GetCell(int x, int y)
    {
        if (!InBounds(x, y))
            throw new ArgumentOutOfRangeException($"Cell ({x},{y}) is out of bounds ({width}x{height})");
        return ref cells[Index(x, y)];
    }

    public ref InventoryCell GetCell(Vector2Int v) => ref GetCell(v.x, v.y);

    public StoredObject GetOccupant(int x, int y) => GetCell(x, y).occupant;

    public void BuildCells()
    {
        cells = new InventoryCell[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cells[Index(x, y)] = new InventoryCell
                {
                    state = CellState.Empty,
                    coord = new Vector2Int(x, y)
                };
            }
        }
    }

    public void RebuildCells()
    {
        BuildCells();
        foreach (var obj in stored)
            MarkOccupied(obj);
    }

    public bool TryPlace(StoredObject obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        foreach (var offset in obj.EffectiveFootprint.OccupiedCells())
        {
            int cx = obj.origin.x + offset.x;
            int cy = obj.origin.y + offset.y;
            if (!InBounds(cx, cy) || cells[Index(cx, cy)].state != CellState.Empty)
                return false;
        }

        MarkOccupied(obj);
        stored.Add(obj);
        return true;
    }

    public void Place(StoredObject obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));
        MarkOccupied(obj);
        stored.Add(obj);
    }

    public void Remove(StoredObject obj)
    {
        if (obj == null || !stored.Contains(obj)) return;

        foreach (var offset in obj.EffectiveFootprint.OccupiedCells())
        {
            int cx = obj.origin.x + offset.x;
            int cy = obj.origin.y + offset.y;
            if (!InBounds(cx, cy)) continue;
            ref var cell = ref cells[Index(cx, cy)];
            cell.state = CellState.Empty;
            cell.occupant = null;
        }
        stored.Remove(obj);
    }


    public void UnlockRow(int y)
    {
        if (!InBounds(0, y)) return;
        for (int x = 0; x < width; x++)
            if (cells[Index(x, y)].state == CellState.Disable)
                cells[Index(x, y)].state = CellState.Empty;
    }

    public void UnlockColumn(int x)
    {
        if (!InBounds(x, 0)) return;
        for (int y = 0; y < height; y++)
            if (cells[Index(x, y)].state == CellState.Disable)
                cells[Index(x, y)].state = CellState.Empty;
    }

    public void Resize(int newWidth, int newHeight)
    {
        var newCells = new InventoryCell[newWidth * newHeight];

        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                newCells[y * newWidth + x] = new InventoryCell
                {
                    state = CellState.Disable,
                    coord = new Vector2Int(x, y)
                };
            }
        }

        for (int x = 0; x < Mathf.Min(width, newWidth); x++)
            for (int y = 0; y < Mathf.Min(height, newHeight); y++)
                newCells[y * newWidth + x] = cells[Index(x, y)];

        cells = newCells;
        width = newWidth;
        height = newHeight;
    }

    public void SetCellState(Vector2Int coord, CellState state, StoredObject occupant = null)
    {
        if (!InBounds(coord))
            throw new ArgumentOutOfRangeException(nameof(coord), $"Cell {coord} is out of bounds ({width}x{height})");

        ref var cell = ref cells[Index(coord)];
        cell.state = state;
        cell.occupant = occupant;
    }

    private void MarkOccupied(StoredObject obj)
    {
        foreach (var offset in obj.EffectiveFootprint.OccupiedCells())
        {
            int cx = obj.origin.x + offset.x;
            int cy = obj.origin.y + offset.y;
            ref var cell = ref cells[Index(cx, cy)];
            cell.state = CellState.Occupied;
            cell.occupant = obj;
        }
    }
}