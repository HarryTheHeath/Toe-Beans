using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class FootprintStep : MonoBehaviour
{
    [SerializeField] private bool firstStep;
    [SerializeField] private bool finalStep;
    [SerializeField] private GameObject nextPrint;
    [SerializeField] private float triggerRadius = 2f;
    [SerializeField] private Vector3 spherePos;

    private bool _hasTriggered = false;


    [ProButton]
    private void Interact()
    {
        if (!finalStep)
        {
            nextPrint.SetActive(true);
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