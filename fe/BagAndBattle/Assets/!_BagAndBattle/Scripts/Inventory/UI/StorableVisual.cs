using UnityEngine;
using UnityEngine.UI;

public class StorableVisual : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private float cellSpacing = 5f; 
    [SerializeField] private float outerPadding = 5f; 

    [Header("Cell State Sprites")]
    [SerializeField] private Sprite disabledSprite;
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Sprite occupiedSprite;
    [SerializeField] private Sprite requiredSprite;

    private RectTransform parent;

    [SerializeField] private RectTransform itemRoot;

    private void Awake()
    {
        parent = transform as RectTransform;
    }

    public void ShowStorableFootprint(Storable storable)
    {
        ShowStorableFootprint(storable.footprint);
    }

    public void ShowStorableFootprint(StorableFootprint footprint)
    {
        if (parent == null)
            parent = transform as RectTransform;

        ClearChildren();

        RectTransform prefabRt = cellPrefab.GetComponent<RectTransform>();
        if (prefabRt == null)
        {
            Debug.LogError("Cell Prefab must have a RectTransform component.");
            return;
        }

        Vector2 cellSize = prefabRt.rect.size;

        for (int row = 0; row < footprint.height; row++)
        {
            for (int col = 0; col < footprint.width; col++)
            {
                CellState state = footprint[col, row] ? CellState.Required : CellState.Disable;

                GameObject go = Instantiate(cellPrefab, parent, false);
                RectTransform rt = (RectTransform)go.transform;

                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0, 1);
                rt.sizeDelta = cellSize;

                // Offset cells by outerPadding so they don't hug the edge
                float xPos = outerPadding + col * (cellSize.x + cellSpacing);
                float yPos = -(outerPadding + row * (cellSize.y + cellSpacing));
                rt.anchoredPosition = new Vector2(xPos, yPos);

                ApplySprite(go, state);
            }
        }

        UpdateParentSize(footprint);
    }

    public void RefreshFootprint(StorableFootprint footprint)
    {
        int expected = footprint.width * footprint.height;
        if (parent.childCount != expected)
        {
            ShowStorableFootprint(footprint);
            return;
        }

        for (int row = 0; row < footprint.height; row++)
        {
            for (int col = 0; col < footprint.width; col++)
            {
                int childIndex = row * footprint.width + col;
                CellState state = footprint[col, row] ? CellState.Required : CellState.Disable;
                ApplySprite(parent.GetChild(childIndex).gameObject, state);
            }
        }

        UpdateParentSize(footprint);
    }

    private void ApplySprite(GameObject go, CellState state)
    {
        Image img = go.GetComponentInChildren<Image>();
        if (img != null)
        {
            img.sprite = SpriteForState(state);
            return;
        }

        RawImage rawImg = go.GetComponentInChildren<RawImage>();
        if (rawImg != null)
        {
            Sprite s = SpriteForState(state);
            rawImg.texture = s != null ? s.texture : null;
            return;
        }

        Debug.LogWarning("Cell prefab has no Image or RawImage component.", go);
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
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            if (!parent.GetChild(i).GetComponent<CellVisual>())
                continue;
            if (Application.isPlaying)
                Destroy(parent.GetChild(i).gameObject);
            else
                DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }

    private void UpdateParentSize(StorableFootprint footprint)
    {
        RectTransform prefabRt = cellPrefab.GetComponent<RectTransform>();
        if (prefabRt == null) return;

        Vector2 cellSize = prefabRt.rect.size;

        float totalWidth = outerPadding * 2 + footprint.width * cellSize.x + Mathf.Max(0, footprint.width - 1) * cellSpacing;
        float totalHeight = outerPadding * 2 + footprint.height * cellSize.y + Mathf.Max(0, footprint.height - 1) * cellSpacing;

        parent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);
        parent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);

        if (itemRoot != null)
        {
            itemRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);
            itemRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
        }
    }
}