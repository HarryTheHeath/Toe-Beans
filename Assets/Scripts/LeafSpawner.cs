using System.Collections.Generic;
using UnityEngine;
using com.cyborgAssets.inspectorButtonPro;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteAlways]
public class LeafSpawner : MonoBehaviour
{
    public enum AreaShape { Rectangle, Circle }

    [Header("Area")]
    public AreaShape shape = AreaShape.Circle;
    public Vector2 areaSize = new Vector2(10f, 8f); // width (x) and depth (y)
    public float radius = 5f;

    [Tooltip("Local Y offset for gizmo center and fallback spawn height when no terrain assigned")]
    public float spawnHeight = 0.8f;

    [Header("Decorations")]
    [Tooltip("Multiple leaf prefabs; one is chosen randomly per spawn.")]
    public GameObject[] leafPrefabs;
    [Tooltip("Leaves per unit² (area * density => number spawned).")]
    public float density = 2f;

    [Header("Spacing")]
    public float minSpacing = 0.5f;
    public int maxPlacementAttempts = 20;

    [Header("Variation / Orientation")]
    [Tooltip("If true, each leaf gets a random yaw around Y axis (while staying flat).")]
    public bool randomRotation = true;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;

    [Header("Determinism")]
    [Tooltip("If true, uses a fixed seed for reproducible placement (useful in editor).")]
    public bool useSeed = false;
    public int seed = 12345;

    [Header("References (optional)")]
    [Tooltip("If assigned, height will be sampled from this terrain. If null, spawnHeight is used.")]
    public Terrain terrain;
    [Tooltip("Optional parent for spawned leaves. If null a child 'Leaves' will be created.")]
    public Transform parent;
    [Tooltip("Slight lift to avoid z-fighting when sampling terrain height")]
    public float heightOffset = 0.02f;

    // Context Menu Methods (visible in component dropdown)
    [ProButton]
    public void Generate() => GenerateInternal();

    [ProButton]
    public void Randomise() => RandomiseInternal();

    [ProButton]
    public void Clear() => ClearInternal();

    public void GenerateInternal()
    {
        if (leafPrefabs == null || leafPrefabs.Length == 0)
        {
            Debug.LogWarning("LeafSpawner: No leaf prefabs assigned.");
            return;
        }

        ClearInternal();

        float area = (shape == AreaShape.Circle) ? Mathf.PI * radius * radius : areaSize.x * areaSize.y;
        int targetCount = Mathf.RoundToInt(area * density);
        if (targetCount <= 0) return;

        List<Vector2> placedPositions = new List<Vector2>();
        System.Random rng = useSeed ? new System.Random(seed) : null;

        // ensure parent exists
        if (parent == null)
        {
            GameObject go = new GameObject("Leaves");
            go.transform.SetParent(this.transform, false);
            parent = go.transform;
        }

        var terrainData = terrain != null ? terrain.terrainData : null;
        Vector3 terrainPos = terrain != null ? terrain.transform.position : Vector3.zero;

        for (int i = 0; i < targetCount; i++)
        {
            Vector2 offset = Vector2.zero;
            bool valid = false;

            for (int attempt = 0; attempt < maxPlacementAttempts; attempt++)
            {
                if (shape == AreaShape.Circle)
                {
                    if (rng != null)
                    {
                        float u = (float)rng.NextDouble();
                        float r = Mathf.Sqrt(u) * radius;
                        float theta = (float)rng.NextDouble() * Mathf.PI * 2f;
                        offset = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * r;
                    }
                    else
                    {
                        offset = Random.insideUnitCircle * radius;
                    }
                }
                else // Rectangle
                {
                    if (rng != null)
                    {
                        float halfX = areaSize.x * 0.5f;
                        float halfY = areaSize.y * 0.5f;
                        float rx = (float)rng.NextDouble() * (halfX * 2f) - halfX;
                        float ry = (float)rng.NextDouble() * (halfY * 2f) - halfY;
                        offset = new Vector2(rx, ry);
                    }
                    else
                    {
                        offset = new Vector2(
                            Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f),
                            Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f)
                        );
                    }
                }

                if (IsPositionValid(offset, placedPositions))
                {
                    valid = true;
                    break;
                }
            }

            if (!valid) continue;

            placedPositions.Add(offset);

            // Use the same local->world mapping as your original script:
            // local (offset.x, spawnHeight, offset.y) transformed by this.transform
            Vector3 localPos = new Vector3(offset.x, spawnHeight, offset.y);
            Vector3 worldPosXZ = transform.TransformPoint(localPos);

            Vector3 spawnPos;
            if (terrain != null && terrainData != null)
            {
                float sampleH = terrain.SampleHeight(worldPosXZ) + terrainPos.y + heightOffset;
                spawnPos = new Vector3(worldPosXZ.x, sampleH, worldPosXZ.z);
            }
            else
            {
                spawnPos = new Vector3(worldPosXZ.x, transform.position.y + spawnHeight, worldPosXZ.z);
            }

            // Orientation: always flat. Random yaw only on Y axis.
            float yaw = 0f;
            if (randomRotation)
            {
                yaw = (rng != null) ? (float)rng.NextDouble() * 360f : Random.Range(0f, 360f);
            }

            Quaternion rot = Quaternion.Euler(90f, yaw, 0f); // X=90 to lie flat, Y=yaw, Z=0

            // choose a random prefab
            GameObject chosen = leafPrefabs[rng != null ? rng.Next(leafPrefabs.Length) : Random.Range(0, leafPrefabs.Length)];

            GameObject inst;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                // Keep prefab connection when instantiating in editor if possible
                inst = (GameObject)PrefabUtility.InstantiatePrefab(chosen, this.gameObject.scene);
                if (inst == null) inst = Instantiate(chosen);
                inst.transform.SetParent(parent, true);
                inst.transform.position = spawnPos;
                inst.transform.rotation = rot;
            }
            else
            {
                inst = Instantiate(chosen, spawnPos, rot, parent);
            }
#else
            inst = Instantiate(chosen, spawnPos, rot, parent);
#endif
            // apply scale variance
            float s = (rng != null) ? Mathf.Lerp(minScale, maxScale, (float)rng.NextDouble()) : Random.Range(minScale, maxScale);
            inst.transform.localScale = new Vector3(s, s, s);
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
#endif
    }

    private bool IsPositionValid(Vector2 candidate, List<Vector2> existing)
    {
        float minSq = minSpacing * minSpacing;
        foreach (var p in existing)
        {
            if ((candidate - p).sqrMagnitude < minSq) return false;
        }
        return true;
    }

    private void RandomiseInternal()
    {
        Transform iterateParent = parent != null ? parent : transform;
        foreach (Transform t in iterateParent)
        {
            var flora = t.GetComponent("Flora");
            if (flora != null)
            {
                var method = flora.GetType().GetMethod("Randomise");
                if (method != null) method.Invoke(flora, null);
            }
            else if (randomRotation)
            {
                float yaw = Random.Range(0f, 360f);
                t.rotation = Quaternion.Euler(90f, yaw, 0f);
            }
        }
    }

    private void ClearInternal()
    {
        if (parent == null)
        {
            Transform maybe = transform.Find("Leaves");
            if (maybe != null) parent = maybe;
        }

        if (parent == null)
        {
            Debug.Log("LeafSpawner: no parent found to clear.");
            return;
        }

#if UNITY_EDITOR
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            var child = parent.GetChild(i).gameObject;
            DestroyImmediate(child);
        }
#else
        foreach (Transform t in parent)
            Destroy(t.gameObject);
#endif

#if UNITY_EDITOR
        if (!Application.isPlaying)
            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
#endif
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;

        Vector3 centre = new Vector3(0f, spawnHeight, 0f);

        if (shape == AreaShape.Rectangle)
        {
            Gizmos.DrawWireCube(centre, new Vector3(areaSize.x, 0f, areaSize.y));
        }
        else
        {
            const int segments = 64;
            Vector3 previousPoint = centre + new Vector3(radius, 0f, 0f);
            for (int i = 1; i <= segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;
                Vector3 nextPoint = centre + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
                Gizmos.DrawLine(previousPoint, nextPoint);
                previousPoint = nextPoint;
            }
        }

        Gizmos.matrix = Matrix4x4.identity;
    }
}