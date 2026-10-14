using TMPro;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public CanvasGroup collectibleCG;
    public GameObject onkeyHUD;
    public GameObject froggieHUD;
    public TextMeshProUGUI onkeyTMP;
    public TextMeshProUGUI froggieTMP;

    public int onkeys;
    public int froggies;

    public void IncreaseCount(CollectibleType type)
    {
        if (type == CollectibleType.Onkey)
        {
            onkeys++;
            onkeyTMP.text = onkeys.ToString();
            onkeyHUD.SetActive(true);
        }
        else if (type == CollectibleType.Froggie)
        {
            froggies++;
            froggieTMP.text = froggies.ToString();
            froggieHUD.SetActive(true);
        }
    }

    public enum CollectibleType
    {
        Onkey,
        Froggie
    }
}
