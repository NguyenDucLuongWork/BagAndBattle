using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }

    [Header("References")]
    [Tooltip("Leave empty to use Camera.main.")]
    [SerializeField] private Camera cam;

    [Header("Raycast")]
    [Tooltip("Only colliders on these layers will be hit. Set this to your interactable layer(s).")]
    [SerializeField] private LayerMask raycastLayer = Physics2D.DefaultRaycastLayers;

    [Header("Debug")]
    [SerializeField] private bool logEvents = false;

    private IInteractable hoveredTarget;
    private IInteractable interactingTarget;
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (cam == null) cam = Camera.main;
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
            SubscribeToInput();
        else
            StartCoroutine(SubscribeNextFrame());
    }

    private void OnDisable()
    {
        if (InputManager.Instance == null) return;

        InputManager.Instance.OnClick -= HandleInteractPoint;
        InputManager.Instance.OnHoldBegin -= HandleInteractStart;
        InputManager.Instance.OnHoldEnd -= HandleInteractStop;
        InputManager.Instance.OnDragBegin -= HandleDragStart;
        InputManager.Instance.OnDragEnd -= HandleDragStop;
    }

    private System.Collections.IEnumerator SubscribeNextFrame()
    {
        yield return null;
        SubscribeToInput();
    }

    private void SubscribeToInput()
    {
        InputManager.Instance.OnClick += HandleInteractPoint;
        InputManager.Instance.OnHoldBegin += HandleInteractStart;
        InputManager.Instance.OnHoldEnd -= HandleInteractStop;
        InputManager.Instance.OnHoldEnd += HandleInteractStop;
        InputManager.Instance.OnDragBegin += HandleDragStart;
        InputManager.Instance.OnDragEnd -= HandleDragStop;    
        InputManager.Instance.OnDragEnd += HandleDragStop;
    }

    private void Update()
    {
        if (InputManager.Instance == null) return;
        UpdateHover(InputManager.Instance.CurrentPointerPosition);
    }

    private void UpdateHover(Vector2 screenPos)
    {
        IInteractable hit = RaycastForInteractable(screenPos);

        if (hit == hoveredTarget) return; 

        if (hoveredTarget != null)
        {
            Log($"HoverExit → {(hoveredTarget as UnityEngine.Object)?.name}");
            hoveredTarget.OnHoverExit();
        }

        hoveredTarget = hit;
        if (hoveredTarget != null)
        {
            Log($"HoverEnter → {(hoveredTarget as UnityEngine.Object)?.name}");
            hoveredTarget.OnHoverEnter();
        }
    }

    private void HandleInteractPoint(Vector2 screenPos)
    {
        IInteractable target = RaycastForInteractable(screenPos);
        if (target == null) return;

        Log($"InteractPoint → {(target as UnityEngine.Object)?.name}");
        target.OnInteractStart();
        target.OnInteractStop();
    }

    private void HandleInteractStart(Vector2 screenPos)
    {
        IInteractable target = RaycastForInteractable(screenPos);
        if (target == null) return;

        interactingTarget = target;
        Log($"InteractStart → {(target as UnityEngine.Object)?.name}");
        target.OnInteractStart();
    }

    private void HandleInteractStop(Vector2 screenPos)
    {
        if (interactingTarget == null) return;
        Log($"InteractStop → {(interactingTarget as UnityEngine.Object)?.name}");
        interactingTarget.OnInteractStop();
        interactingTarget = null;
    }
    private void HandleDragStart(Vector2 screenPos)
    {
        IInteractable target = RaycastForInteractable(screenPos);
        if (target == null) return;

        interactingTarget = target;
        Log($"DragStart → {(target as UnityEngine.Object)?.name}");
        target.OnInteractStart();
    }

    private void HandleDragStop(Vector2 screenPos)
    {
        if (interactingTarget == null) return;
        Log($"DragStop → {(interactingTarget as UnityEngine.Object)?.name}");
        interactingTarget.OnInteractStop();
        interactingTarget = null;
    }

    private IInteractable RaycastForInteractable(Vector2 screenPos)
    {
        if (cam == null) return null;

        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, raycastLayer);

        if (hit.collider == null) return null;

        IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
        if (interactable == null || !interactable.IsInteractable) return null;

        return interactable;
    }

    private void Log(string msg)
    {
        if (logEvents) Debug.Log($"[InteractionManager] {msg}");
    }
}