using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class LocalScaleRandomiser : MonoBehaviour
{
    [Header("Behavior")]
    [SerializeField] private bool randomizeOnStart = true;

    [SerializeField] private Vector2 uniformRange = new Vector2(0.8f, 1.2f);
    [SerializeField] private Vector3 originalLocalScale = Vector3.one;


    [ProButton]
    public void RandomizeGameObject()
    {
        transform.localScale = GetRandomisedScale(transform.localScale);
    }

    [ProButton]
    public void ResetToOriginal()
    {
        transform.localScale = originalLocalScale;
    }

    [ProButton]
    public void RandomizeChildren()
    {
        foreach (Transform child in transform)
            child.localScale = GetRandomisedScale(child.localScale);
 
    }
           

    [ProButton]
    public void ResetChildren()
    {
        foreach (var child in transform)
            transform.localScale = originalLocalScale;
    }

    private Vector3 GetRandomisedScale(Vector3 transform)
    {
        float newValue = Random.Range(uniformRange.x, uniformRange.y);
        Vector3 newScale = new Vector3 (newValue, newValue, newValue);
        return newScale;
    }
}