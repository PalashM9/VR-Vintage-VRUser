using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class GroundFollowingAdvanced : MonoBehaviour
{
    [Header("References")]
    public Transform xrOrigin;
    public Transform head; 

    [Header("Ground Detection")]
    public LayerMask groundLayers = ~0;
    public float raycastDistance = 3.0f;
    public float groundOffset = 0.02f;

    [Header("Height Adjustment")]
    public float followSpeed = 10f;
    public float maxStepHeight = 0.4f;

    [Header("Debug")]
    public bool drawDebugRay = false;

    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {

        var netObj = GetComponent<NetworkObject>();
        if (netObj != null && !netObj.IsOwner)
        {
            enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (xrOrigin == null || head == null) return;

        Vector3 rayOrigin = head.position;
        Vector3 rayDir = Vector3.down;

        if (drawDebugRay)
        {
            Debug.DrawRay(rayOrigin, rayDir * raycastDistance, Color.green);
        }

        if (!Physics.Raycast(
            rayOrigin,
            rayDir,
            out RaycastHit hit,
            raycastDistance,
            groundLayers,
            QueryTriggerInteraction.Ignore))
        {
            return;
        }

        float targetY =
            hit.point.y +
            characterController.height * 0.5f +
            groundOffset;

        float currentY = xrOrigin.position.y;
        float delta = targetY - currentY;

        if (Mathf.Abs(delta) > maxStepHeight)
            return;

        float newY = Mathf.Lerp(
            currentY,
            targetY,
            followSpeed * Time.deltaTime
        );

        Vector3 pos = xrOrigin.position;
        pos.y = newY;
        xrOrigin.position = pos;
    }
}

