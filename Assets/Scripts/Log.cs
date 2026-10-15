using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class Log : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] circleEnds;
    [SerializeField] MeshRenderer log;
    [SerializeField] Material logMatBirch;
    [SerializeField] Material logMatPine;
    [SerializeField] Sprite logEndBirch;
    [SerializeField] Sprite logEndPine;

    [Header("Type")]
    public WoodType woodType;

    [ProButton]
    public void AssignMat()
    {
        if (woodType == WoodType.Birch)
        {
            log.material = logMatBirch;
            foreach (SpriteRenderer circle in circleEnds)
                circle.sprite = logEndBirch;
        }
        else
        {
            log.material = logMatPine;
            foreach (SpriteRenderer circle in circleEnds)
                circle.sprite = logEndPine;
        }
    }

    public enum WoodType
    {
        Birch,
        Pine
    }

}
