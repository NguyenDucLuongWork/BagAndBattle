using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class StorableFootprint
{
    public bool[] cells;
    public int width, height;

    public bool this[int col, int row] => cells[row * width + col];

    public StorableFootprint(int width, int height)
    {
        this.width = width;
        this.height = height;
        this.cells = new bool[width * height];
    }

    public StorableFootprint(int width, int height, bool[] cells)
    {
        this.width = width;
        this.height = height;
        this.cells = cells;
    }

    public StorableFootprint Rotate90()
    {
        var r = new bool[width * height];
        for (int row = 0; row < height; row++)
            for (int col = 0; col < width; col++)
                r[col * height + (height - 1 - row)] = cells[row * width + col];

        return new StorableFootprint(height, width, r);
    }

    public StorableFootprint Rotated(int steps)
    {
        var fp = this;
        steps = ((steps % 4) + 4) % 4;
        for (int i = 0; i < steps; i++) fp = fp.Rotate90();
        return fp;
    }

    public IEnumerable<Vector2Int> OccupiedCells()
    {
        for (int row = 0; row < height; row++)
            for (int col = 0; col < width; col++)
                if (this[col, row])
                    yield return new Vector2Int(col, height - 1 - row);
    }

    public void RotateSelf(int steps = 1)
    {
        var rotated = Rotated(steps);
        width = rotated.width;
        height = rotated.height;
        cells = rotated.cells;
    }
}