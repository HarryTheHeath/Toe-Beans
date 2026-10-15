using UnityEngine;
using UnityEngine.UI;

public class LeafMinigame : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;
    [SerializeField] public Image[] leafButtons;
    private int removedCount = 0;
    private bool printClickable = false;
    public Image bearPrint;
    public CollectibleManager collectibleManager;
    public CollectibleManager.CollectibleType type;



    public void RemoveLeaf()
    {
        removedCount++;
        if (removedCount >= leafButtons.Length)
        {
            Debug.Log("All leafs removed!");
            bearPrint.raycastTarget = true;
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
        collectibleManager.IncreaseCount(type);
    }
}