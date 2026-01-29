using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// <summary>

// Bimanual rotation:

// - Object is grabbed with RIGHT hand

// - Object rotates based on LEFT hand lateral movement (X axis)

// - Rotation is applied to the visual child (not the grab root)

// </summary>

[RequireComponent(typeof(XRGrabInteractable))]
public class XRBimanualRotate : MonoBehaviour
{
    [Header("Hand References")]
    [Tooltip("Assign LEFT Hand Geometry transform here")]
    public Transform leftHandTransform;

    [Header("Visual")]
    [Tooltip("Assign the child mesh transform that should visually rotate")]
    public Transform visualTransform;

    [Header("Rotation Settings")]
    [Tooltip("Rotation sensitivity (degrees per meter of hand movement)")]
    public float rotationSensitivity = 300f;

    [Tooltip("Rotation axis (usually Y axis for clockwise / anticlockwise)")]
    public Vector3 rotationAxis = Vector3.up;

    [Tooltip("Ignore tiny hand movements")]
    public float deadZone = 0.002f;

    private XRGrabInteractable grab;
    private Vector3 lastLeftHandPosition;
    private bool trackingLeftHand;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();

        grab.selectEntered.AddListener(OnSelectEntered);
        grab.selectExited.AddListener(OnSelectExited);
    }

    void Update()
    {

        if (!grab.isSelected || !trackingLeftHand)
            return;

        if (leftHandTransform == null || visualTransform == null)
            return;

        Vector3 currentPos = leftHandTransform.position;
        Vector3 delta = currentPos - lastLeftHandPosition;

        float rotationAmount = delta.x * rotationSensitivity;

        if (Mathf.Abs(rotationAmount) > deadZone)
        {

            visualTransform.Rotate(rotationAxis, rotationAmount, Space.Self);

            Debug.Log(
                $"[XRBimanualRotate] Left ΔX={delta.x:F4} → Rotation={rotationAmount:F2}",
                this
            );
        }

        lastLeftHandPosition = currentPos;
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (leftHandTransform != null)
        {
            lastLeftHandPosition = leftHandTransform.position;
            trackingLeftHand = true;
        }

        Debug.Log($"[XRBimanualRotate] Object grabbed: {name}", this);
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        trackingLeftHand = false;
        Debug.Log($"[XRBimanualRotate] Object released: {name}", this);
    }
}

