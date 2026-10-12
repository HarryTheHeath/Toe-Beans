using UnityEngine;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
#endif

// No-op Button attribute so the file compiles without Odin. If you have Odin, define ODIN_INSPECTOR (or SIRENIX_ODIN_INSPECTOR) to use the real attribute.
#if !SIRENIX_ODIN_INSPECTOR && !ODIN_INSPECTOR
namespace Sirenix.OdinInspector
{
    using System;
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class ButtonAttribute : System.Attribute { public ButtonAttribute() { } }
}
#endif

namespace Decals
{
    public enum DecalVariant
    {
        Left,
        LeftScuffed,
        LeftVeryScuffed,
        Right,
        RightScuffed,
        RightVeryScuffed
    }

    [CreateAssetMenu(menuName = "Decals/Animal Decal Set")]
    public class AnimalDecalSet : ScriptableObject
    {
        [Header("Source materials (asset references)")]
        public Material left;
        public Material leftScuffed;
        public Material leftVeryScuffed;
        public Material right;
        public Material rightScuffed;
        public Material rightVeryScuffed;

        [Header("Shared opacity by scuff level (0..1)")]
        [Range(0f, 1f)] public float baseOpacity = 1f;        // Left / Right
        [Range(0f, 1f)] public float scuffedOpacity = 1f;     // LeftScuffed / RightScuffed
        [Range(0f, 1f)] public float veryScuffedOpacity = 1f; // LeftVeryScuffed / RightVeryScuffed

        [Header("Name of the color/opacity property (e.g. _BaseColor, _Color, _Opacity)")]
        public string opacityProperty = "_BaseColor";

        public Material GetSource(DecalVariant v)
        {
            switch (v)
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
    }

    [ExecuteAlways]
    [RequireComponent(typeof(DecalProjector))]
    public class DecalHelper : MonoBehaviour
    {
        [Tooltip("Assign an AnimalDecalSet (serializable per animal)")]
        public AnimalDecalSet animal;

        [Tooltip("Which variant is currently active")]
        public DecalVariant variant = DecalVariant.Left;

        [Tooltip("Optional: assign manually, otherwise the DecalProjector on this GameObject will be used")]
        public DecalProjector decalProjector;

        // material instance assigned to the projector (not saved to assets)
        Material _instanceMaterial;

        void Reset()
        {
            decalProjector = GetComponent<DecalProjector>();
        }

        void OnEnable()
        {
            if (decalProjector == null) decalProjector = GetComponent<DecalProjector>();
            ApplyVariant();
        }

        void OnDisable()
        {
#if UNITY_EDITOR
            if (_instanceMaterial != null)
            {
                DestroyImmediate(_instanceMaterial);
                _instanceMaterial = null;
            }
#else
            if (_instanceMaterial != null)
            {
                Destroy(_instanceMaterial);
                _instanceMaterial = null;
            }
#endif
        }

        void OnValidate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                ApplyVariant();
#endif
        }

        // Public API
        public void SetVariant(DecalVariant v)
        {
            variant = v;
            ApplyVariant();
        }

        // ProButton / Odin methods (compile without Odin thanks to the no-op attribute above)
        [Sirenix.OdinInspector.Button] public void Left() => SetVariant(DecalVariant.Left);
        [Sirenix.OdinInspector.Button] public void LeftScuffed() => SetVariant(DecalVariant.LeftScuffed);
        [Sirenix.OdinInspector.Button] public void LeftVeryScuffed() => SetVariant(DecalVariant.LeftVeryScuffed);
        [Sirenix.OdinInspector.Button] public void Right() => SetVariant(DecalVariant.Right);
        [Sirenix.OdinInspector.Button] public void RightScuffed() => SetVariant(DecalVariant.RightScuffed);
        [Sirenix.OdinInspector.Button] public void RightVeryScuffed() => SetVariant(DecalVariant.RightVeryScuffed);

        // Apply the current variant: instantiate a material, set the scuff-level opacity, assign to DecalProjector.
        public void ApplyVariant()
        {
            if (animal == null) return;

            if (decalProjector == null) decalProjector = GetComponent<DecalProjector>();
            if (decalProjector == null) return;

            Material src = animal.GetSource(variant);
            if (src == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"DecalHelper: source material for {variant} is null on '{name}'.");
#endif
                return;
            }

            // Cleanup previous instance
            if (_instanceMaterial != null)
            {
#if UNITY_EDITOR
                DestroyImmediate(_instanceMaterial);
#else
                Destroy(_instanceMaterial);
#endif
                _instanceMaterial = null;
            }

            // Instantiate material so we don't modify the asset
            _instanceMaterial = Instantiate(src);
            _instanceMaterial.hideFlags = HideFlags.HideAndDontSave;

            // Determine opacity by scuff level (shared across left/right)
            float opacity = GetOpacityForVariant(variant);
            ApplyOpacity(_instanceMaterial, opacity, animal.opacityProperty);

            // Assign to projector
            decalProjector.material = _instanceMaterial;
        }

        float GetOpacityForVariant(DecalVariant v)
        {
            if (animal == null) return 1f;

            switch (v)
            {
                case DecalVariant.Left:
                case DecalVariant.Right:
                    return animal.baseOpacity;
                case DecalVariant.LeftScuffed:
                case DecalVariant.RightScuffed:
                    return animal.scuffedOpacity;
                case DecalVariant.LeftVeryScuffed:
                case DecalVariant.RightVeryScuffed:
                    return animal.veryScuffedOpacity;
                default:
                    return 1f;
            }
        }

        void ApplyOpacity(Material mat, float opacity, string prop)
        {
            if (mat == null) return;

            // Try the provided property name first (color alpha or float)
            if (!string.IsNullOrEmpty(prop) && mat.HasProperty(prop))
            {
                // try color alpha
                try
                {
                    Color c = mat.GetColor(prop);
                    c.a = opacity;
                    mat.SetColor(prop, c);
                    return;
                }
                catch { }
                // try float
                try
                {
                    mat.SetFloat(prop, opacity);
                    return;
                }
                catch { }
            }

            // Common fallbacks
            if (mat.HasProperty("_BaseColor"))
            {
                Color c = mat.GetColor("_BaseColor");
                c.a = opacity;
                mat.SetColor("_BaseColor", c);
                return;
            }

            if (mat.HasProperty("_Color"))
            {
                Color c = mat.GetColor("_Color");
                c.a = opacity;
                mat.SetColor("_Color", c);
                return;
            }

            if (mat.HasProperty("_Opacity"))
            {
                mat.SetFloat("_Opacity", opacity);
                return;
            }

            // Last resort: material.color
            try
            {
                Color c = mat.color;
                c.a = opacity;
                mat.color = c;
                return;
            }
            catch
            {
#if UNITY_EDITOR
                Debug.LogWarning($"DecalHelper: couldn't apply opacity to '{mat.name}'. Set 'opacityProperty' on AnimalDecalSet to the correct property name.");
#endif
            }
        }
    }
}