using com.cyborgAssets.inspectorButtonPro;
using System;
using UnityEngine;

public class Shrooms : MonoBehaviour
{
    [SerializeField] private ChangeSpritesChildren spriteList;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Sprite[] highlightSprites;
    [SerializeField] private SpriteRenderer[] highlightSpritesSr;
    public ShroomTypes shroomType;


    [ProButton]
    private void ChangeShroom()
    {
        Sprite shroomSprite;
        Sprite highlightSprite;

        switch (shroomType)
        {
            case ShroomTypes.FlyAgariac:
                shroomSprite = sprites[0];
                highlightSprite = highlightSprites[0];
                break;
            case ShroomTypes.Chantarelle:
                shroomSprite = sprites[1];
                highlightSprite = highlightSprites[1];
                break;
            case ShroomTypes.PennyBun:
                shroomSprite = sprites[2];
                highlightSprite = highlightSprites[2];
                break;
            default:
                shroomSprite = sprites[0];
                highlightSprite = highlightSprites[0];
                break;
        }

        spriteList.ChangeSpiteExternal(shroomSprite);
        spriteList.ChangeSpiteExternalArray(highlightSprite, highlightSpritesSr);
    }

    public enum ShroomTypes
    {
        FlyAgariac,
        Chantarelle,
        PennyBun
    }
}