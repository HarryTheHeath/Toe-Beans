using UnityEngine;
using com.cyborgAssets.inspectorButtonPro;

public class Flora : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public Billboard billboard;

    [Header("Random Scale")]
    [SerializeField] private float minScale = 0.075f;
    [SerializeField] private float maxScale = 0.125f;


    private void Awake()
    {
        SetSR();
    }

    [ProButton]
    public void SetSR()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }


    [ProButton]
    public void SetSprite(int index)
    {
        if (index >= 0 && index < sprites.Length)
        {
            spriteRenderer.sprite = sprites[index];
        }
    }

    [ProButton]
    public void Randomise()
    {
        SetSR();
        SetRandomScale();
        SetRandomSprite();
    }


    [ProButton]
    public void SetRandomSprite()
    {
        if (sprites.Length > 0)
        {
            SetSprite(Random.Range(0, sprites.Length));
        }
    }

    [ProButton]
    public void SetRandomScale()
    {
        float scale = Random.Range(minScale, maxScale);
        transform.localScale = Vector3.one * scale;
    }

    public void ResetSize()
    {
        transform.localScale = Vector3.one * 0.1f;
    }
}

