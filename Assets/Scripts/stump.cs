using UnityEngine;
using System;
using TMPro;

public class stump : MonoBehaviour, IInteractable
{
    
    [SerializeField] private GameObject playerObject;
    [SerializeField] private Camera stumpCamera;
    [SerializeField] private Camera playerCamera;
    
    [Header("UI")]
    [SerializeField] private string uiPromptTag;
    [SerializeField] private string uiPromptAction;
    

    private bool sitting = false;

    public void Interact()
    {
        
        playerObject.TryGetComponent<FirstPersonPlayerController>(out FirstPersonPlayerController player);
        if (sitting)
        {
            player.unfreeze();
            sitting = false;
            stumpCamera.enabled = false;
            playerCamera.enabled = true;
            uiPromptTag = "stump";
            uiPromptAction = "sit";
        }
        else
        {
            // freeze player
            player.freeze();
            // activate new camera looking from the stump
            stumpCamera.enabled = true;
            playerCamera.enabled = false;
            sitting = true;
            // change prompt to "get up"
            player.promptTagTMP.text = "";
            player.promptActionTMP.text = "get up";
            player.promptObject.alpha = 1;
        }
        
    }

    public bool hasTriggered()
    {
        return false;
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
