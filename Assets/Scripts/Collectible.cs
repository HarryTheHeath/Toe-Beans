using System;
using UnityEngine;

public class Collectible : MonoBehaviour, IInteractable
{

    [Header("UI")]
    [SerializeField] private String uiPromptTag;
    [SerializeField] private String uiPromptAction;
    [SerializeField] private CollectibleManager manager;
    [SerializeField] private SFX sfx;
    public CollectibleManager.CollectibleType type;
    public FirstPersonPlayerController player;

    private bool _hasTriggered;

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
        sfx.PlayAudio();
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
