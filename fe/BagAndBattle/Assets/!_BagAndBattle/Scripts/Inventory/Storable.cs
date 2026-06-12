using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Storable : MonoBehaviour
{
    public string id;
    public StorableFootprint footprint;

    [ContextMenu("InitFootprint")]
    public void InitFootprint()
    {
        int w = footprint.width;
        int h = footprint.height;
        footprint = new StorableFootprint(w, h);
    }
}