using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryGrid
{
    public int width;
    public int height;

    public InventoryCell[,] cells;

    public List<StoredObject> stored = new List<StoredObject>();

    public InventoryGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
        BuildCells();
    }

    public void BuildCells()
    {
        cells = new InventoryCell[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                cells[x, y] = new InventoryCell { state = CellState.Empty };
    }

    public void RebuildCells()
    {
        BuildCells();
        foreach (var obj in stored)
            foreach (var offset in obj.EffectiveFootprint.OccupiedCells())
            {
                var cell = cells[obj.origin.x + offset.x, obj.origin.y + offset.y];
                cell.state = CellState.Occupied;
                cell.occupant = obj;
            }
    }

    public bool InBounds(int x, int y) =>
    x >= 0 && x < width && y >= 0 && y < height;

    public bool InBounds(Vector2Int v) => InBounds(v.x, v.y);

    public InventoryCell GetCell(int x, int y) => cells[x, y];
    public InventoryCell GetCell(Vector2Int v) => cells[v.x, v.y];

    public StoredObject GetOccupant(int x, int y) => cells[x, y].occupant;

    public void Place(StoredObject obj)
    {
        foreach (var offset in obj.EffectiveFootprint.OccupiedCells())
        {
            var cell = cells[obj.origin.x + offset.x, obj.origin.y + offset.y];
            cell.state = CellState.Occupied;
            cell.occupant = obj;
        }
        stored.Add(obj);
    }

    public void Remove(StoredObject obj)
    {
        foreach (var offset in obj.EffectiveFootprint.OccupiedCells())
        {
            var cell = cells[obj.origin.x + offset.x, obj.origin.y + offset.y];
            cell.state = CellState.Empty;
            cell.occupant = null;
        }
        stored.Remove(obj);
    }

    public void UnlockRow(int y)
    {
        if (y < 0 || y >= height) return;
        for (int x = 0; x < width; x++)
            if (cells[x, y].state == CellState.Locked)
                cells[x, y].state = CellState.Empty;
    }

    public void UnlockColumn(int x)
    {
        if (x < 0 || x >= width) return;
        for (int y = 0; y < height; y++)
            if (cells[x, y].state == CellState.Locked)
                cells[x, y].state = CellState.Empty;
    }

    public void Resize(int newWidth, int newHeight)
    {
        var newCells = new InventoryCell[newWidth, newHeight];
        for (int x = 0; x < newWidth; x++)
            for (int y = 0; y < newHeight; y++)
                newCells[x, y] = (x < width && y < height)
                    ? cells[x, y]
                    : new InventoryCell { state = CellState.Locked };

        cells = newCells;
        width = newWidth;
        height = newHeight;
    }
}