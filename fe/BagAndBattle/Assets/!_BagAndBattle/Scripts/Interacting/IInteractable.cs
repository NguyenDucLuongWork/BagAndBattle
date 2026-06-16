public interface IInteractable
{
    void OnHoverEnter();

    void OnHoverExit();

    void OnInteractStart();

    void OnInteractStop();

    bool IsInteractable { get; }
}