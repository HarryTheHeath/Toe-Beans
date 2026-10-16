using System;
using UnityEngine;

public class Collectible : MonoBehaviour, IInteractable
{

    [Header("UI")]
    [SerializeField] private String uiPromptTag;
    [SerializeField] private String uiPromptAction;
    [SerializeField] private CollectibleManager manager;
    public CollectibleManager.CollectibleType type;
    public FirstPersonPlayerController player;

    private bool _hasTriggered;
    [SerializeField] private LadyLyla lyla;

    public bool hasTriggered()
    {
        return _hasTriggered;
    }

    public void Interact()
    {
        if (_hasTriggered)
            return;

        _hasTriggered = true;
        manager.IncreaseCount(type);
        lyla.PlayTake();
        gameObject.SetActive(false);
        player.ClearInteractable(this);
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
