using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class InteractableMono : MonoBehaviour, IInteractable
{

    [Header("State")]
    [Tooltip("Uncheck to temporarily disable all interaction without removing the component.")]
    [SerializeField] private bool isInteractable = true;

    [Header("Events")]
    [Space(4)]
    [Tooltip("Pointer/finger entered this object.")]
    public UnityEvent onHoverEnter;

    [Tooltip("Pointer/finger left this object.")]
    public UnityEvent onHoverExit;

    [Tooltip("Click pressed, hold started, or drag started on this object.")]
    public UnityEvent onInteractStart;

    [Tooltip("Click released, hold released, or drag released on this object.")]
    public UnityEvent onInteractStop;

    [Header("Debug (read-only)")]
    [SerializeField, HideInInspector] private bool isHovered;
    [SerializeField, HideInInspector] private bool isInteracting;

    public bool IsHovered => isHovered;
    public bool IsInteracting => isInteracting;

    public bool IsInteractable => isInteractable && isActiveAndEnabled;

    public void OnHoverEnter()
    {
        if (!IsInteractable) return;
        isHovered = true;
        onHoverEnter?.Invoke();
    }

    public void OnHoverExit()
    {
        isHovered = false;
        onHoverExit?.Invoke();

        if (isInteracting)
            OnInteractStop();
    }

    public void OnInteractStart()
    {
        if (!IsInteractable) return;
        isInteracting = true;
        onInteractStart?.Invoke();
    }

    public void OnInteractStop()
    {
        if (!isInteracting) return;
        isInteracting = false;
        onInteractStop?.Invoke();
    }

    public void SetInteractable(bool value)
    {
        if (!value && isHovered)
        {
            OnHoverExit();
        }
        isInteractable = value;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (GetComponent<Collider2D>() == null)
            Debug.LogWarning($"[InteractableMono] '{name}' needs a Collider2D to be detected by InteractionManager.", this);
    }
#endif
}