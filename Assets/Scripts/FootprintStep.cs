using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class FootprintStep : MonoBehaviour
{
    [SerializeField] private bool firstStep;
    [SerializeField] private bool finalStep;
    [SerializeField] private GameObject nextPrint;
    [SerializeField] private GameObject[] breadcrumbs;
    [SerializeField] private float triggerRadius = 2f;
    [SerializeField] private Vector3 spherePos;

    private bool _hasTriggered = false;


    [ProButton]
    private void Interact()
    {
        if (!finalStep)
        {
            nextPrint.SetActive(true);
            foreach (var breadcrumb in breadcrumbs)
                breadcrumb.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Woohoo");
        }
    }

    private void Awake()
    {
        if (!firstStep)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            Interact();
    }
}