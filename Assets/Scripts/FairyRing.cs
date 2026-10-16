using com.cyborgAssets.inspectorButtonPro;
using System.Collections;
using UnityEngine;

public class FairyRing : MonoBehaviour, IInteractable
{
    [SerializeField] private ChangeSpritesChildren spriteList;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Sprite[] highlightSprites;
    [SerializeField] private SpriteRenderer[] highlightSpritesSr;
    public bool toggleHighlightOnStart;
    public ShroomTypes shroomType;
    public bool isStartTeleport;
    public bool teleporting;
    public float waitTime;
    public FirstPersonPlayerController firstPersonPlayerController;
    public Transform startTeleport;
    public LadyLyla lyla;

    [Header("UI")]
    [SerializeField] private string myName;
    [SerializeField] private string action;



    private void Start()
    {
        if (toggleHighlightOnStart)
            ToggleHighlights(false);
    }

    public void ToggleHighlights(bool toggle)
    {
        foreach (var sr in highlightSpritesSr)
            sr.enabled = toggle;
    }


    public IEnumerator Teleport()
    {
        teleporting = true;
        ToggleHighlights(true);
        firstPersonPlayerController.Freeze(true);
        lyla.PlayReaction();
        yield return new WaitForSeconds(waitTime);
        firstPersonPlayerController.TeleportTo(startTeleport, true);
        teleporting = false;
        ToggleHighlights(false);
    }

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

    public void Interact()
    {
        if (!teleporting && !isStartTeleport)
            StartCoroutine(Teleport());
    }

    public bool hasTriggered()
    {
        return teleporting;
    }

    public string promptTag()
    {
        if (isStartTeleport) return "";
        else 
            return myName;
    }

    public string promptAction()
    {
        if (isStartTeleport) return "";
        else
            return action;
    }

    public enum ShroomTypes
    {
        FlyAgariac,
        Chantarelle,
        PennyBun
    }
}