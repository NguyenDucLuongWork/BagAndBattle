using System;
using UnityEngine;

[Serializable]
public class StoredObject
{
    public Storable storable;
    public Vector2Int origin;
    public int rotation; // 0/1/2/3 = 0/90/180/270 

    public StorableFootprint EffectiveFootprint =>
        storable.footprint.Rotated(rotation);
}