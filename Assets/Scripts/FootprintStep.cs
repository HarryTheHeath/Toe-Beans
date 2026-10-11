using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class FootprintStep : MonoBehaviour
{
    [SerializeField] private bool firstStep;
    [SerializeField] private bool finalStep;
    [SerializeField] private GameObject nextPrint;

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
}
