using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class FloraSpawner : MonoBehaviour
{
    public enum AreaShape
    {
        Rectangle,
        Circle
    }

    [Header("Area")]
    [SerializeField] private AreaShape shape = AreaShape.Circle;
    [SerializeField] private Vector2 areaSize = new Vector2(10f, 8f);
    [SerializeField] private float radius = 5f;
    [SerializeField] private float spawnHeight = 0.8f;

    [Header("Decorations")]
    [SerializeField] private Flora[] prefabs;
    [SerializeField] private float density = 2f;

    [Header("Spacing")]
    [SerializeField] private float minSpacing = 0.5f;
    [SerializeField] private int maxPlacementAttempts = 20;

    [Header("Variation")]
    [SerializeField] private bool randomRotation = false;

    [Header("References")]
    [SerializeField] private Transform camRef;

    [ProButton]
    public void Generate()
    {
        if (prefabs == null || prefabs.Length == 0)
            return;

        Clear();

        float area = shape == AreaShape.Circle
            ? Mathf.PI * radius * radius
            : areaSize.x * areaSize.y;

        int count = Mathf.RoundToInt(area * density);

        List<Vector2> placedPositions = new();

        for (int i = 0; i < count; i++)
        {
            Vector2 offset = Vector2.zero;
            bool validPositionFound = false;

            for (int attempt = 0; attempt < maxPlacementAttempts; attempt++)
            {
                if (shape == AreaShape.Circle)
                {
                    offset = Random.insideUnitCircle * radius;
                }
                else
                {
                    offset = new Vector2(
                        Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f),
                        Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f)
                    );
                }

                if (IsPositionValid(offset, placedPositions))
                {
                    validPositionFound = true;
                    break;
                }
            }

            if (!validPositionFound)
                continue;

            placedPositions.Add(offset);

            Vector3 localPosition = new Vector3(
                offset.x,
                spawnHeight,
                offset.y
            );

            Vector3 worldPosition = transform.TransformPoint(localPosition);

            float rotationY = randomRotation
                ? Random.Range(0f, 360f)
                : 0f;

            var instance = Instantiate(
                prefabs[Random.Range(0, prefabs.Length)],
                worldPosition,
                Quaternion.Euler(0f, rotationY, 0f),
                transform
            );

            instance.billboard.cam = camRef;
        }

        Randomise();
    }

    private bool IsPositionValid(
        Vector2 candidate,
        List<Vector2> existingPositions)
    {
        foreach (Vector2 position in existingPositions)
        {
            if ((candidate - position).sqrMagnitude < minSpacing * minSpacing)
                return false;
        }

        return true;
    }

    [ProButton]
    public void Randomise()
    {
        foreach (Flora flora in GetComponentsInChildren<Flora>())
            flora.Randomise();
    }

    [ProButton]
    public void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;

        if (shape == AreaShape.Rectangle)
        {
            Gizmos.DrawWireCube(
                new Vector3(0f, spawnHeight, 0f),
                new Vector3(areaSize.x, 0f, areaSize.y)
            );
        }
        else
        {
            const int segments = 64;

            Vector3 centre = new Vector3(
                0f,
                spawnHeight,
                0f
            );

            Vector3 previousPoint =
                centre + new Vector3(radius, 0f, 0f);

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;

                Vector3 nextPoint = centre + new Vector3(
                    Mathf.Cos(angle) * radius,
                    0f,
                    Mathf.Sin(angle) * radius
                );

                Gizmos.DrawLine(previousPoint, nextPoint);
                previousPoint = nextPoint;
            }
        }

        Gizmos.matrix = Matrix4x4.identity;
    }
}