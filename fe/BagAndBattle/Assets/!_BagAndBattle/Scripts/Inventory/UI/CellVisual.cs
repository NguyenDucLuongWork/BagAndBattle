using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CellVisual : MonoBehaviour
{
    public enum CellVisualState
    {
        FootprintEnabled,
        FootprintDisabled
    }

    public Color footprintEnabled;
    public Color footprintDisabled;

    private RawImage rawImage;

    public void Init()
    {
        if(rawImage == null)
        {
            rawImage = GetComponentInChildren<RawImage>();
        }
    }

    // TODO : Update to fit project visual design
    [Obsolete]
    public void UpdateState(CellVisualState state)
    {
        if(state == CellVisualState.FootprintEnabled)
        {
            rawImage.color = footprintEnabled;
            return;
        }
        if (state == CellVisualState.FootprintDisabled)
        {
            rawImage.color = footprintDisabled;
            return;
        }
    }

    // TODO : Update to fit project visual design
    [Obsolete]
    public void SetOccupied(bool occupied)
    {
        Init();
        rawImage.color = occupied
            ? footprintEnabled
            : footprintDisabled;
    }
}