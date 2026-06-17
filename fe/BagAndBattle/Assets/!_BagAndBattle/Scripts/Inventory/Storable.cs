using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(StorableVisual))]
public class Storable : MonoBehaviour
{
    public StorableFootprint footprint;

    private StorableVisual visual;

    public CellState GetCellState(int col, int row)
    {
        return footprint[col, row] ? CellState.Required : CellState.Disable;
    }

    [ContextMenu("Init")]
    public void Init(StorableFootprint footprint)
    {
        int w = footprint.width;
        int h = footprint.height;
        this.footprint = footprint;

        UpdateVisual();
    }

    private void Awake()
    {
        visual = GetComponent<StorableVisual>();
        visual.ShowStorableFootprint(footprint);
    }

    [ContextMenu("UpdateVisual")]
    public void UpdateVisual()
    {
        visual = GetComponent<StorableVisual>();
        visual.ShowStorableFootprint(footprint);
    }
}