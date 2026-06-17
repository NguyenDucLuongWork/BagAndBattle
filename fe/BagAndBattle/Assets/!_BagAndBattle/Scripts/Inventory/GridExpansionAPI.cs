using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Inventory))]
public class GridExpansionAPI : MonoBehaviour
{
    [Header("Expansion limits")]
    [SerializeField] private int maxWidth = 10;
    [SerializeField] private int maxHeight = 10;

    public UnityEvent<int, int> OnGridExpanded = new UnityEvent<int, int>();

    private Inventory mono;

    private void Awake()
    {
        mono = GetComponent<Inventory>();
        if (mono == null)
            Debug.LogError("[GridExpansionAPI] Requires Inventory on the same GameObject.");
    }

    public bool UnlockRow(int y)
    {
        if (!IsRowUnlockable(y)) return false;
        mono.grid.UnlockRow(y);
        mono.RaiseCellsChanged();
        OnGridExpanded.Invoke(mono.grid.width, mono.grid.height);
        return true;
    }

    public bool UnlockColumn(int x)
    {
        if (!IsColumnUnlockable(x)) return false;
        mono.grid.UnlockColumn(x);
        mono.RaiseCellsChanged();
        OnGridExpanded.Invoke(mono.grid.width, mono.grid.height);
        return true;
    }

    public bool AddRow()
    {
        var grid = mono.grid;
        if (grid.height >= maxHeight) return false;

        int newRow = grid.height;
        grid.Resize(grid.width, grid.height + 1);
        grid.UnlockRow(newRow);

        mono.RaiseCellsChanged();
        OnGridExpanded.Invoke(grid.width, grid.height);
        return true;
    }

    public bool AddColumn()
    {
        var grid = mono.grid;
        if (grid.width >= maxWidth) return false;

        int newCol = grid.width;
        grid.Resize(grid.width + 1, grid.height);
        grid.UnlockColumn(newCol);

        mono.RaiseCellsChanged();
        OnGridExpanded.Invoke(grid.width, grid.height);
        return true;
    }

    public bool ResizeLocked(int newWidth, int newHeight)
    {
        if (newWidth > maxWidth || newHeight > maxHeight) return false;

        mono.grid.Resize(newWidth, newHeight);
        mono.RaiseCellsChanged();
        OnGridExpanded.Invoke(newWidth, newHeight);
        return true;
    }

    public bool CanExpandWidth => mono.grid.width < maxWidth;
    public bool CanExpandHeight => mono.grid.height < maxHeight;

    private bool IsRowUnlockable(int y) =>
        y >= 0 && y < mono.grid.height;

    private bool IsColumnUnlockable(int x) =>
        x >= 0 && x < mono.grid.width;
}

