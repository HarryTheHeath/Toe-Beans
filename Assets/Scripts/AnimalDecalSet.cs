using UnityEngine;

public class AnimalDecalSet : MonoBehaviour
{
    [Header("Materials (source assets)")]
    public Material left;
    public Material leftScuffed;
    public Material leftVeryScuffed;

    public Material right;
    public Material rightScuffed;
    public Material rightVeryScuffed;

    [Header("Shared opacity per side (0..1)")]
    [Range(0f, 1f)] public float leftOpacity = 1f;
    [Range(0f, 1f)] public float rightOpacity = 1f;

    [Header("Which material color/property to use for opacity")]
    // Common names: "_BaseColor" (URP), "_Color", "_Opacity", etc. If your decal shader exposes one of these, set it here.
    public string opacityProperty = "_BaseColor";

    public Material GetSourceMaterial(DecalVariant variant)
    {
        switch (variant)
        {
            case DecalVariant.Left: return left;
            case DecalVariant.LeftScuffed: return leftScuffed;
            case DecalVariant.LeftVeryScuffed: return leftVeryScuffed;
            case DecalVariant.Right: return right;
            case DecalVariant.RightScuffed: return rightScuffed;
            case DecalVariant.RightVeryScuffed: return rightVeryScuffed;
            default: return null;
        }
    }

    public enum DecalVariant
    {
        Left,
        LeftScuffed,
        LeftVeryScuffed,
        Right,
        RightScuffed,
        RightVeryScuffed
    }

}
