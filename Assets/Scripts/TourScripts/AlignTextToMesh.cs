using UnityEngine;
using TMPro;

[ExecuteAlways]
public class AlignTextToMesh : MonoBehaviour
{
    [Tooltip("The mesh the text should align to")]
    public MeshRenderer targetMesh;

    [Tooltip("Small offset so text doesn't z-fight")]
    public float surfaceOffset = 0.001f;

    [Tooltip("Scale factor relative to mesh size")]
    public float scaleFactor = 0.9f;

    void Update()
    {
        if (!targetMesh) return;

        Bounds b = targetMesh.localBounds;

        // Center text on the mesh
        transform.localPosition = b.center + Vector3.forward * surfaceOffset;

        // Match mesh orientation
        transform.localRotation = Quaternion.identity;

        // Scale text to fit mesh
        float size = Mathf.Min(b.size.x, b.size.y);
        transform.localScale = Vector3.one * size * scaleFactor;
    }
}
