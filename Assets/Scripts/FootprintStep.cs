using System;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class FootprintStep : MonoBehaviour, IInteractable
{
    [SerializeField] private bool firstStep;
    [SerializeField] private bool finalStep;
    [SerializeField] private GameObject nextPrint;
    // White part of the print. Link in final footprint.
    [SerializeField] private GameObject whitePrint;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject[] breadcrumbs;
    //[SerializeField] private float triggerRadius = 2f;
    [SerializeField] private Vector3 spherePos;
    [SerializeField] private LadyLyla lyla;

    [Header("UI")]
    [SerializeField] private String uiPromptTag;
    [SerializeField] private String uiPromptAction;
    
    
    // Leaf minigame
    public CanvasGroup leafMinigame;


    private bool _hasTriggered = false;
    private bool _finished;


    [ProButton]
    public void Interact()
    {
        if (_finished) return;
        if (!finalStep)
        {
            if (firstStep)
                lyla.PlayPrints();

            nextPrint.SetActive(true);
            foreach (var breadcrumb in breadcrumbs)
                breadcrumb.gameObject.SetActive(false);
        }
        else
        {
            lyla.PlayLastPrintReaction();
            leafMinigame.gameObject.SetActive(true);
            playerObject.TryGetComponent<FirstPersonPlayerController>(out FirstPersonPlayerController player);
            player.Freeze(false);
            _finished = true;
        }
        _hasTriggered = true;
    }


    private void Awake()
    {
        if (!firstStep)
            gameObject.SetActive(false);
    }

    private void Start()
    {
        if (finalStep)
            whitePrint.SetActive(true);
    }
        public bool hasTriggered()
    {
        return _hasTriggered;
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