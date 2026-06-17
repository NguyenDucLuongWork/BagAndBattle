using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isPressed = false;

    [Tooltip("Event fires continuously every frame while holding")]
    public UnityEvent onButtonHold;

    [Tooltip("Event fires once when the button is first pressed")]
    public UnityEvent onButtonDown;

    [Tooltip("Event fires once when the button is released")]
    public UnityEvent onButtonUp;

    void Update()
    {
        if (isPressed)
        {
            // Fires every frame the button is being held down
            onButtonHold?.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        onButtonDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        onButtonUp?.Invoke();
    }
}