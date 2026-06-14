using UnityEngine;

public class StorableFootprintDebugger : MonoBehaviour
{
    [Header("Debug Colors")]
    public Color occupiedColor = Color.white;
    public Color emptyColor = Color.black;

    public StorableVisual visual;

    [ContextMenu("Test_Visual")]
    public void TestVisual()
    {
        visual.ShowStorableFootprint(GetComponent<Storable>());
    }

    [ContextMenu("Test_Rotate90")]
    public void Test_Rotate90()
    {
        GetComponent<Storable>().footprint.RotateSelf();
        TestVisual();
    }

    [ContextMenu("Test_Rotate180")]
    public void Test_Rotate180()
    {
        GetComponent<Storable>().footprint.RotateSelf(2);
        TestVisual();
    }

    [ContextMenu("Test_Rotate270")]
    public void Test_Rotate270()
    {
        GetComponent<Storable>().footprint.RotateSelf(3);
        TestVisual();
    }
}