using UnityEngine;

public class StorableVisual : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private float padding = 5f;

    private RectTransform parent;

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
                GameObject go = Instantiate(cellPrefab, parent, false);
                RectTransform rt = (RectTransform)go.transform;

                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0, 1);

                rt.sizeDelta = cellSize;

                float xPos = col * (cellSize.x + padding);
                float yPos = -row * (cellSize.y + padding);
                rt.anchoredPosition = new Vector2(xPos, yPos);

                CellVisual visual = go.GetComponent<CellVisual>();
                if (visual != null)
                {
                    visual.Init();
#pragma warning disable CS0618
                    visual.SetOccupied(footprint[col, row]);
#pragma warning restore CS0618
                }
            }
        }

        float totalHeight = (footprint.height * cellSize.y) + (Mathf.Max(0, footprint.height - 1) * padding);

        parent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
    }

    private void ClearChildren()
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
                Destroy(parent.GetChild(i).gameObject);
            else
                DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }
}