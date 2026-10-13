public interface IInteractable
{
    void Interact();
    bool hasTriggered();
    string promptTag();
    string promptAction();
}