using System;
using UnityEngine;

public class Collectable : MonoBehaviour, IInteractable
{

    [Header("UI")]
    [SerializeField] private String uiPromptTag;
    [SerializeField] private String uiPromptAction;

    private bool _hasTriggered;

    public bool hasTriggered()
    {
        return _hasTriggered;
    }

    public void Interact()
    {
        gameObject.SetActive(false);
        _hasTriggered = true;
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
