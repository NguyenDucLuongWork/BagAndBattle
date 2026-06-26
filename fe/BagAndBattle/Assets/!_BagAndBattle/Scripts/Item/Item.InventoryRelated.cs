using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public partial class Item
{
    public void Rotate()
    {
        bool rotatable = (storable.RotateStorable());
        if (!rotatable)
        {
            return;
        }

        ApplySize(storableVisual);

        itemVisual.Rotate((RectTransform)this.transform);

    }

    public void ApplySize(StorableVisual storableVisual)
    {
        RectTransform rt = (RectTransform)transform;
        RectTransform sourceRt = (RectTransform)storableVisual.transform;

        rt.sizeDelta = sourceRt.sizeDelta;
    }
} 