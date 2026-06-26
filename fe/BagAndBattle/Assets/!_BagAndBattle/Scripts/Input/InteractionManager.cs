using LgTyLib.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionManager : BaseSingleton<InteractionManager>
{
    public static string ItemHolderTag = "ItemHolder";

    private static readonly List<RaycastResult> raycastResultsCache = new List<RaycastResult>();

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        //InputManager.Instance.OnClick += HandleClick;
    }

    private void OnDisable()
    {
        //if (InputManager.Instance != null)
        //    InputManager.Instance.OnClick -= HandleClick;
    }

    private void HandleClick(Vector2 screenPosition)
    {
        bool overItemHolder = IsPointerOverItemHolder(screenPosition);
        Debug.Log($"[InteractionManager] Click at {screenPosition} | Over ItemHolder: {overItemHolder}");
    }

    public bool IsPointerOverItemHolder()
    {
        return IsPointerOverItemHolder(InputManager.Instance.CurrentPointerPosition);
    }

    public bool IsPointerOverItemHolder(Vector2 screenPosition)
    {
        if (EventSystem.current == null) return false;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        raycastResultsCache.Clear();
        EventSystem.current.RaycastAll(pointerData, raycastResultsCache);

        for (int i = 0; i < raycastResultsCache.Count; i++)
        {
            if (raycastResultsCache[i].gameObject.CompareTag(ItemHolderTag))
                return true;
        }

        return false;
    }
}