using UnityEngine;

public class InventoryGridVisual : MonoBehaviour
{
    [Header("Cell Prefab")]
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private float padding = 5f;

    [Header("Cell State Sprites")]
    [SerializeField] private Sprite disabledSprite;
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Sprite occupiedSprite;
    [SerializeField] private Sprite requiredSprite;

    private RectTransform parent;
    private RectTransform cellContainer;
    private RectTransform itemSlotContainer;

    private void Awake()
    {
        parent = transform as RectTransform;
    }

    private void EnsureContainers()
    {
        if (cellContainer == null)
        {
            var cellGo = new GameObject("CellContainer", typeof(RectTransform));
            cellContainer = (RectTransform)cellGo.transform;
            cellContainer.SetParent(parent, false);
            StretchFull(cellContainer);
        }

        if (itemSlotContainer == null)
        {
            var slotGo = new GameObject("ItemSlotContainer", typeof(RectTransform));
            itemSlotContainer = (RectTransform)slotGo.transform;
            itemSlotContainer.SetParent(parent, false);
            StretchFull(itemSlotContainer);
        }
    }

    public void ShowGrid(InventoryGrid grid)
    {
        if (parent == null)
            parent = transform as RectTransform;

        ClearChildren();
        EnsureContainers();

        RectTransform prefabRt = cellPrefab.GetComponent<RectTransform>();
        if (prefabRt == null)
        {
            Debug.LogError("Cell Prefab must have a RectTransform component.");
            return;
        }

        Vector2 cellSize = prefabRt.rect.size;

        float totalHeight = grid.height * cellSize.y + Mathf.Max(0, grid.height - 1) * padding;

        for (int row = 0; row < grid.height; row++)
        {
            for (int col = 0; col < grid.width; col++)
            {
                ref InventoryCell cell = ref grid.GetCell(col, row);

                float xPos = col * (cellSize.x + padding);

                float yPos = totalHeight - cellSize.y - row * (cellSize.y + padding);

                GameObject cellGo = Instantiate(cellPrefab, cellContainer, false);
                RectTransform cellRt = (RectTransform)cellGo.transform;
                SetupRect(cellRt, cellSize, xPos, yPos);
                ApplySprite(cellGo, cell.state);

                GameObject slotGo = itemSlotPrefab != null
                    ? Instantiate(itemSlotPrefab, itemSlotContainer, false)
                    : new GameObject("ItemSlot", typeof(RectTransform));

                if (itemSlotPrefab == null)
                    slotGo.transform.SetParent(itemSlotContainer, false);

                RectTransform slotRt = (RectTransform)slotGo.transform;
                SetupRect(slotRt, cellSize, xPos, yPos);

                cell.rectTransform = slotRt;
            }
        }

        float totalWidth = grid.width * cellSize.x + Mathf.Max(0, grid.width - 1) * padding;

        parent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);
        parent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
        cellContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);
        cellContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
        itemSlotContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);
        itemSlotContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
    }

    public void RefreshGrid(InventoryGrid grid)
    {
        if (cellContainer == null || itemSlotContainer == null)
        {
            ShowGrid(grid);
            return;
        }

        int expected = grid.width * grid.height;
        if (cellContainer.childCount != expected)
        {
            ShowGrid(grid);
            return;
        }

        for (int screenRow = 0; screenRow < grid.height; screenRow++)
        {
            int gridY = grid.height - 1 - screenRow;

            for (int col = 0; col < grid.width; col++)
            {
                int childIndex = screenRow * grid.width + col;
                ref InventoryCell cell = ref grid.GetCell(col, gridY);

                GameObject cellGo = cellContainer.GetChild(childIndex).gameObject;
                ApplySprite(cellGo, cell.state);

                cell.rectTransform = (RectTransform)itemSlotContainer.GetChild(childIndex);
            }
        }
    }

    private void SetupRect(RectTransform rt, Vector2 size, float x, float y)
    {
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 0);
        rt.pivot = new Vector2(0, 0);
        rt.sizeDelta = size;
        rt.anchoredPosition = new Vector2(x, y);
    }

    private void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
    }

    private void ApplySprite(GameObject go, CellState state)
    {
        UnityEngine.UI.Image img = go.GetComponentInChildren<UnityEngine.UI.Image>();
        if (img != null)
        {
            img.sprite = SpriteForState(state);
            return;
        }

        UnityEngine.UI.RawImage rawImg = go.GetComponentInChildren<UnityEngine.UI.RawImage>();
        if (rawImg != null)
        {
            Sprite s = SpriteForState(state);
            rawImg.texture = s != null ? s.texture : null;
            return;
        }

        Debug.LogWarning($"Cell prefab has no Image or RawImage component.", go);
    }

    private Sprite SpriteForState(CellState state) => state switch
    {
        CellState.Disable => disabledSprite,
        CellState.Empty => emptySprite,
        CellState.Occupied => occupiedSprite,
        CellState.Required => requiredSprite,
        _ => null
    };

    private void ClearChildren()
    {
        cellContainer = null;
        itemSlotContainer = null;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
                Destroy(parent.GetChild(i).gameObject);
            else
                DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }
}