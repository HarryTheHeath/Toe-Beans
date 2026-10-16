using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class Step : MonoBehaviour
{
    [SerializeField] MeshRenderer step;
    [SerializeField] SpriteRenderer[] srs;
    [SerializeField] Material mat1;
    [SerializeField] Material mat2;
    [SerializeField] Material mat3;
    [SerializeField] Sprite sprite1;
    [SerializeField] Sprite sprite2;
    [SerializeField] Sprite sprite3;


    [Header("Type")]
    public StepType stepType;

    [ProButton]
    public void AssignMat()
    {
        if (stepType == StepType.Step1)
        {
            step.material = mat1;
            foreach (SpriteRenderer sr in srs)
                sr.sprite = sprite1;
        }
        else if (stepType == StepType.Step2)
        {
            step.material = mat2;
            foreach (SpriteRenderer sr in srs)
                sr.sprite = sprite2;
        }
        else
        {
            step.material = mat3;
            foreach (SpriteRenderer sr in srs)
                sr.sprite = sprite3;
        }
    }

    [ProButton]
    public void FlipSpites()
    {
        foreach (SpriteRenderer sr in srs)
            sr.flipX = !sr.flipX;
    }

    public enum StepType
    {
        Step1,
        Step2,
        Step3
    }
}