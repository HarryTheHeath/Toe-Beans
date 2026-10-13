using UnityEngine;
using UnityEngine.UI;

public class LeafMinigame : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;
    public Image[] leafButtons;
    private int removedCount = 0;

    public void RemoveLeaf()
    {
        removedCount++;
        if (removedCount >= leafButtons.Length)
        {
            MiniGameWin();
        }
    }
    
    private void MiniGameWin()
    {
        Debug.Log("MiniGameWin");
        playerObject.TryGetComponent<FirstPersonPlayerController>(out FirstPersonPlayerController player);
        player.unfreeze();
    }
    
    // TODO: add leafbuttons to the array and count number of removed leafs.
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
