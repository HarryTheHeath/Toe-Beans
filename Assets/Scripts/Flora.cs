using UnityEngine;
using com.cyborgAssets.inspectorButtonPro;

public class Flora : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeReference] private SpriteRenderer spriteRenderer;



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
    public void SetRandomSprite()
    {
        if (sprites.Length > 0)
        {
            SetSprite(Random.Range(0, sprites.Length));
        }
    }
}

