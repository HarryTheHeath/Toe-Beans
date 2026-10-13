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
    [SerializeField] private GameObject[] breadcrumbs;
    [SerializeField] private float triggerRadius = 2f;
    [SerializeField] private Vector3 spherePos;

    [Header("UI")]
    [SerializeField] private String uiPromptTag;
    [SerializeField] private String uiPromptAction;


    private bool _hasTriggered = false;


    [ProButton]
    public void Interact()
    {
        if (!finalStep)
        {
            nextPrint.SetActive(true);
            foreach (var breadcrumb in breadcrumbs)
                breadcrumb.gameObject.SetActive(false);
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