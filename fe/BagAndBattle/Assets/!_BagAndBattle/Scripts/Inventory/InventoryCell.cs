using System;
using UnityEngine;

[Serializable]
public class InventoryCell
{
    public CellState state;

    public Vector2Int coord;

    public StoredObject occupant;

    public RectTransform rectTransform;
}