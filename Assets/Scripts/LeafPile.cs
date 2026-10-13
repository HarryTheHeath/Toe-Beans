using System.Collections.Generic;
using UnityEngine;
using com.cyborgAssets.inspectorButtonPro;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteAlways]
public class LeafPile : MonoBehaviour
{
    [Header("Sphere")]
    [Tooltip("Center transform of the sphere. If null, this GameObject transform is used.")]
    public Transform sphereCenter;
    [Tooltip("Radius of the sphere surface where leaves sit (world units).")]
    public float radius = 1f;

    [Header("Spawn")]
    [Tooltip("Leaf prefabs (use a flat quad prefab with its plane facing +Z).")]
    public GameObject[] leafPrefabs;
    [Tooltip("Number of leaves to attempt to place. Actual placed may be less due to spacing constraints.")]
    public int targetCount = 300;
    [Tooltip("Minimum spacing between spawned leaves (world units).")]
    public float minSpacing = 0.05f;
    [Tooltip("Maximum attempts per leaf before giving up.")]
    public int maxPlacementAttempts = 30;

    [Header("Pile shape")]
    [Tooltip("Thickness of the pile along the normal (leaves will be offset inward up to this amount).")]
    public float pileThickness = 0.05f;
    [Tooltip("Only spawn on the upper hemisphere (normal.y >= hemisphereMinY). 0 = exact half, 0.1+ = more top-heavy.")]
    [Range(0f, 1f)]
    public float hemisphereMinY = 0f;

    [Header("Orientation")]
    [Tooltip("Random yaw around the surface normal in degrees.")]
    public float maxRandomYaw = 90f;
    [Tooltip("Maximum small tilt applied around tangent axes for more natural leaf variation.")]
    public float maxTiltDegrees = 5f;

    [Header("Core")]
    [Tooltip("Create a dark core sphere beneath the leaves to give depth.")]
    public bool createCore = true;
    [Tooltip("Core material (use a dark material).")]
    public Material coreMaterial;
    [Tooltip("Inset core slightly so leaves overlap it (radius = radius - coreInset).")]
    public float coreInset = 0.02f;

    [Header("Terrain / safety")]
    [Tooltip("Optional terrain. If assigned, placements under terrain height will be skipped.")]
    public Terrain terrain;
    [Tooltip("Small vertical lift above the sampled terrain height to avoid z-fighting.")]
    public float groundHeightOffset = 0.01f;

    [Header("Organization")]
    [Tooltip("Parent for spawned leaves; created as a child if null.")]
    public Transform parent;

    [ProButton]
    public void Generate() => GenerateInternal();

    [ProButton]
    public void Clear() => ClearInternal();

    void Reset()
    {
        if (sphereCenter == null) sphereCenter = transform;
    }

    public void GenerateInternal()
    {
        ClearInternal();

        if (leafPrefabs == null || leafPrefabs.Length == 0)
        {
            Debug.LogWarning("LeafPile: Please assign at least one leaf prefab.");
            return;
        }

        if (sphereCenter == null) sphereCenter = transform;
        if (radius <= 0f)
        {
            Debug.LogWarning("LeafPile: radius must be > 0.");
            return;
        }

        if (parent == null)
        {
            GameObject go = new GameObject("LeafPile");
            go.transform.SetParent(transform, false);
            parent = go.transform;
        }

        List<Vector3> placed = new List<Vector3>();
        int placedCount = 0;
        int attemptsOverall = 0;

        for (int i = 0; i < targetCount; i++)
        {
            bool placedThis = false;

            for (int attempt = 0; attempt < maxPlacementAttempts; attempt++)
            {
                attemptsOverall++;

                // create a point on the sphere's upper hemisphere
                float y = Random.Range(hemisphereMinY, 1f);
                float theta = Random.Range(0f, Mathf.PI * 2f);
                float r = Mathf.Sqrt(Mathf.Max(0f, 1f - y * y));

                Vector3 normal = new Vector3(r * Mathf.Cos(theta), y, r * Mathf.Sin(theta));
                normal.Normalize();

                // world position on sphere surface
                Vector3 worldPosOnSphere = sphereCenter.position + normal * radius;

                // skip if below terrain
                if (terrain != null)
                {
                    float terrainHeight = terrain.SampleHeight(worldPosOnSphere) + terrain.transform.position.y;
                    if (worldPosOnSphere.y < terrainHeight + groundHeightOffset)
                    {
                        continue;
                    }
                }

                // offset inward slightly to create the pile thickness
                float inward = Random.Range(0f, pileThickness);
                Vector3 spawnPos = worldPosOnSphere - normal * inward;

                // spacing check
                bool okay = true;
                float minSq = minSpacing * minSpacing;
                foreach (var p in placed)
                {
                    if ((p - spawnPos).sqrMagnitude < minSq)
                    {
                        okay = false;
                        break;
                    }
                }

                if (!okay) continue;

                GameObject prefab = leafPrefabs[Random.Range(0, leafPrefabs.Length)];
                GameObject inst = null;

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab, this.gameObject.scene);
                    if (inst == null) inst = Instantiate(prefab);
                }
                else
                {
                    inst = Instantiate(prefab);
                }
#else
                inst = Instantiate(prefab);
#endif

                inst.transform.SetParent(parent, true);

                // Correct orientation:
                // flat leaf plane should face tangent to sphere, so its local +Z points along the surface normal
                // random yaw around that normal, not around world axes
                Vector3 dir = normal.normalized;

                // align local +Z to the sphere normal
                Quaternion normalAlign = Quaternion.FromToRotation(Vector3.back, dir);

                // random yaw around that normal
                float yaw = Random.Range(0f, maxRandomYaw);
                Quaternion yawRot = Quaternion.AngleAxis(yaw, dir);

                Quaternion finalRot = yawRot * normalAlign;

                // optional tiny tilt around a tangent axis to make leaves less perfectly uniform
                if (maxTiltDegrees > 0f)
                {
                    Vector3 tangent = Vector3.Cross(dir, Vector3.up);
                    if (tangent.sqrMagnitude < 0.0001f)
                        tangent = Vector3.Cross(dir, Vector3.right);

                    tangent.Normalize();

                    float tilt = Random.Range(-maxTiltDegrees, maxTiltDegrees);
                    finalRot = Quaternion.AngleAxis(tilt, tangent) * finalRot;
                }

                inst.transform.position = spawnPos;
                inst.transform.rotation = finalRot;

                float s = Random.Range(0.8f, 1.2f);
                inst.transform.localScale = new Vector3(s, s, s);

                placed.Add(spawnPos);
                placedCount++;
                placedThis = true;
                break;
            }

            if (!placedThis)
                continue;
        }

        if (createCore)
            CreateOrUpdateCore();

#if UNITY_EDITOR
        if (!Application.isPlaying)
            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
#endif

        Debug.Log($"LeafPile: placed {placedCount} leaves on the sphere.");
    }

    void CreateOrUpdateCore()
    {
        if (sphereCenter == null) sphereCenter = transform;

        string coreName = "LeafPile_Core";
        Transform coreT = transform.Find(coreName);

        if (coreT == null)
        {
            GameObject core = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            core.name = coreName;

#if UNITY_EDITOR
            Object.DestroyImmediate(core.GetComponent<Collider>());
#else
            Destroy(core.GetComponent<Collider>());
#endif

            coreT = core.transform;
            coreT.SetParent(transform, false);
        }

        float coreRadius = Mathf.Max(0f, radius - coreInset);
        coreT.localScale = Vector3.one * coreRadius * 2f; // sphere default diameter = 1
        coreT.position = sphereCenter.position;

        if (coreMaterial != null)
        {
            var rend = coreT.GetComponent<Renderer>();
            if (rend != null)
                rend.sharedMaterial = coreMaterial;
        }
    }

    public void ClearInternal()
    {
        Transform tParent = parent;
        if (tParent == null)
            tParent = transform.Find("LeafPile");

        if (tParent != null)
        {
#if UNITY_EDITOR
            for (int i = tParent.childCount - 1; i >= 0; i--)
            {
                var child = tParent.GetChild(i).gameObject;
                Object.DestroyImmediate(child);
            }
#else
            foreach (Transform c in tParent)
                Destroy(c.gameObject);
#endif
        }

        Transform core = transform.Find("LeafPile_Core");
        if (core != null)
        {
#if UNITY_EDITOR
            Object.DestroyImmediate(core.gameObject);
#else
            Destroy(core.gameObject);
#endif
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
#endif
    }

    private void OnDrawGizmosSelected()
    {
        if (sphereCenter == null) sphereCenter = transform;

        Gizmos.color = new Color(0.2f, 0.6f, 0.2f, 0.6f);
        Gizmos.matrix = transform.localToWorldMatrix;

        // hemisphere outline
        Vector3 centre = Vector3.zero;
        const int segments = 48;

        // full circle at base
        Vector3 prev = new Vector3(radius, 0f, 0f) + centre;
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 next = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius) + centre;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }

        // latitude lines
        for (int l = 1; l <= 4; l++)
        {
            float y = Mathf.Lerp(0f, 1f, l / (float)(5));
            float r = Mathf.Sqrt(Mathf.Max(0f, 1f - y * y)) * radius;

            Vector3 p0 = centre + new Vector3(r, y * radius, 0f);
            for (int s = 0; s <= segments; s++)
            {
                float angle = s * Mathf.PI * 2f / segments;
                Vector3 p1 = centre + new Vector3(Mathf.Cos(angle) * r, y * radius, Mathf.Sin(angle) * r);
                Gizmos.DrawLine(p0, p1);
                p0 = p1;
            }
        }

        Gizmos.matrix = Matrix4x4.identity;
    }
}