using System.Collections.Generic;
using UnityEngine;

public static class PlacementValidator
{

    public enum FailReason
    {
        None,
        OutOfBounds,
        CellLocked,
        CellOccupied,
    }

    public readonly struct PlacementResult
    {
        public readonly bool Success;
        public readonly FailReason Reason;
        public readonly Vector2Int FailCell;

        public static readonly PlacementResult Ok =
            new PlacementResult(true, FailReason.None, default);

        public PlacementResult(bool success, FailReason reason, Vector2Int failCell)
        {
            Success = success;
            Reason = reason;
            FailCell = failCell;
        }

        public override string ToString() =>
            Success ? "OK" : $"Failed ({Reason} @ {FailCell})";
    }

    public static PlacementResult CanPlace(
        InventoryGrid grid,
        Storable storable,
        Vector2Int origin,
        int rotation,
        StoredObject ignore = null)
    {
        var footprint = storable.Footprint.Rotated(rotation);

        foreach (var offset in footprint.OccupiedCells())
        {
            int x = origin.x + offset.x;
            int y = origin.y + offset.y;

            if (!grid.InBounds(x, y))
                return new PlacementResult(false, FailReason.OutOfBounds,
                    new Vector2Int(x, y));

            var cell = grid.GetCell(x, y);

            if (cell.state == CellState.Disable)
                return new PlacementResult(false, FailReason.CellLocked,
                    new Vector2Int(x, y));

            if (cell.state == CellState.Occupied && cell.occupant != ignore)
                return new PlacementResult(false, FailReason.CellOccupied,
                    new Vector2Int(x, y));
        }

        return PlacementResult.Ok;
    }

    public static bool IsValid(
        InventoryGrid grid,
        Storable storable,
        Vector2Int origin,
        int rotation,
        StoredObject ignore = null)
        => CanPlace(grid, storable, origin, rotation, ignore).Success;

    public static IEnumerable<CellPreview> GetPreviewCells(
        InventoryGrid grid,
        Storable storable,
        Vector2Int origin,
        int rotation,
        StoredObject ignore = null)
    {
        var footprint = storable.Footprint.Rotated(rotation);
        foreach (var offset in footprint.OccupiedCells())
        {
            int x = origin.x + offset.x;
            int y = origin.y + offset.y;

            bool valid;
            if (!grid.InBounds(x, y))
                valid = false;
            else
            {
                var cell = grid.GetCell(x, y);
                valid = cell.state == CellState.Empty
                     || (cell.state == CellState.Occupied && cell.occupant == ignore);
            }

            yield return new CellPreview(new Vector2Int(x, y), valid);
        }
    }

    public readonly struct CellPreview
    {
        public readonly Vector2Int Cell;
        public readonly bool IsValid;
        public CellPreview(Vector2Int cell, bool isValid) { Cell = cell; IsValid = isValid; }
    }

    public static Vector2Int SnapToGrid(
        InventoryGrid grid,
        Storable storable,
        int rotation,
        Vector2 worldPos,
        Vector2 cellSize,
        Vector2 gridOriginWorld)
    {
        var fp = storable.Footprint.Rotated(rotation);

        float rawX = (worldPos.x - gridOriginWorld.x) / cellSize.x;
        float rawY = (worldPos.y - gridOriginWorld.y) / cellSize.y;

        int snappedX = Mathf.RoundToInt(rawX - fp.width * 0.5f);
        int snappedY = Mathf.RoundToInt(rawY - fp.height * 0.5f);

        snappedX = Mathf.Clamp(snappedX, 0, grid.width - fp.width);
        snappedY = Mathf.Clamp(snappedY, 0, grid.height - fp.height);

        return new Vector2Int(snappedX, snappedY);
    }
}