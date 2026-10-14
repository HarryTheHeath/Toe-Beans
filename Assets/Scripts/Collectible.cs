using System;
using UnityEngine;

public class Collectible : MonoBehaviour, IInteractable
{

    [Header("UI")]
    [SerializeField] private String uiPromptTag;
    [SerializeField] private String uiPromptAction;
    [SerializeField] private CollectibleManager manager;
    public CollectibleManager.CollectibleType type;

    private bool _hasTriggered;

    public bool hasTriggered()
    {
        return _hasTriggered;
    }

    public void Interact()
    {
        _hasTriggered = true;
        manager.IncreaseCount(type);
        gameObject.SetActive(false);
    }

    public string promptAction()
    {
        return uiPromptAction;
    }

    public string promptTag()
    {
        return uiPromptTag;
    }
}
