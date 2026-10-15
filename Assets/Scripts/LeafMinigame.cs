using UnityEngine;
using UnityEngine.UI;

public class LeafMinigame : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;
    [SerializeField] public Image[] leafButtons;
    private int removedCount = 0;
    private bool printClickable = false;
    

    public void RemoveLeaf()
    {
        removedCount++;
        if (removedCount >= leafButtons.Length)
        {
            Debug.Log("All leafs removed!");
            printClickable = true;
        }
    }

    public void OnClickPrint()
    {
        if (printClickable)
            MiniGameWin();
    }
    
    private void MiniGameWin()
    {
        Debug.Log("MiniGameWin");
        playerObject.TryGetComponent<FirstPersonPlayerController>(out FirstPersonPlayerController player);
        player.Unfreeze();
        gameObject.SetActive(false);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
