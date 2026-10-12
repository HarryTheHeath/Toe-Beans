using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Transform cam;

    void Start()
    {
        if (cam == null)
            cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(cam);
    }
}