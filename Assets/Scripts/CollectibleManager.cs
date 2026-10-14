using TMPro;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public CanvasGroup collectibleCG;
    public GameObject onkeyHUD;
    public GameObject froggieHUD;
    public TextMeshProUGUI onkeyTMP;
    public TextMeshProUGUI froggieTMP;

    public GameObject ammoniteHUD;
    public TextMeshProUGUI ammoniteTMP;

    public GameObject bearHUD;
    public TextMeshProUGUI bearTMP;
    public GameObject deerHUD;
    public TextMeshProUGUI deerTMP;
    public GameObject redPandaHUD;
    public TextMeshProUGUI redPandaTMP;
    public GameObject craneHUD;
    public TextMeshProUGUI craneTMP;



    public int onkeys;
    public int froggies;
    public int ammonite;
    public int bears; 
    public int deers;
    public int redPanda;
    public int cranes;






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
        else if (type == CollectibleType.Fossil)
        {
            ammonite++;
            ammoniteTMP.text = ammonite.ToString();
            ammoniteHUD.SetActive(true);
        }
        else if (type == CollectibleType.BearTrack)
        {
            bears++;
            bearTMP.text = bearTMP.ToString();
            bearHUD.SetActive(true);
        }


    }

    public enum CollectibleType
    {
        Onkey,
        Froggie,
        FroggieRed,
        FroggieGreen,
        FroggieBlue,
        Fossil,
        BearTrack,
        DeerTrack,
        CraneTrack,
        RedPandaTrack
    }
}
