using UnityEngine;

public class MrMori : MonoBehaviour, IInteractable
{
    [Header("Tags")]
    [SerializeField] private string name;
    [SerializeField] private string action;

    public void Interact()
    {
        throw new System.NotImplementedException();
    }

    public bool hasTriggered()
    {
        throw new System.NotImplementedException();
    }

    public string promptTag()
    {
        throw new System.NotImplementedException();
    }

    public string promptAction()
    {
        throw new System.NotImplementedException();
    }

}
