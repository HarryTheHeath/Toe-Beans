using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class ChangeSpritesChildren : MonoBehaviour
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private SpriteRenderer[] srs;
    [SerializeField] private bool flipSpritesOnAwake;

    private void Awake()
    {
        if (flipSpritesOnAwake)
            RandomFlipSprites();
    }


    [ProButton]
    private void GetSRs()
    {
        foreach (var child in transform)
        {
            if (srs.Length > 0)
                System.Array.Clear(srs, 0, srs.Length);
            srs = GetComponentsInChildren<SpriteRenderer>(true) ;
        }
    }


    [ProButton]
    private void ChangeSpite()
    {
        foreach (var sr in srs)
            if (sr != null)
                sr.sprite = sprite;
    }

    public void ChangeSpiteExternal(Sprite newSprite)
    {
        foreach (var sr in srs)
            if (sr != null)
                sr.sprite = newSprite;
    }

    public void ChangeSpiteExternalArray(Sprite newSprite, SpriteRenderer[] sprites)
    {
        foreach (var sr in sprites)
            if (sr != null)
                sr.sprite = newSprite;
    }

    private void RandomFlipSprites()
    {
        foreach (var sr in srs)
            if (sr != null)
                sr.flipX = Random.value > 0.5f;
    }
}