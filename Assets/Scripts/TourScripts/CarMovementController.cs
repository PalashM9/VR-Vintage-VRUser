using UnityEngine;
using UnityEngine.InputSystem;

public class CarMovementController : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference moveAction;   

    public InputActionReference turnAction;   

    [Header("Car Settings")]
    public float maxSpeed = 6f;
    public float acceleration = 6f;

    [Tooltip("Lower = smoother turning")]
    public float turnSpeed = 35f;   

    [Header("Seat / XR")]
    public Transform seatPoint;
    public Transform xrOrigin;
    public MonoBehaviour hmdNavigation;
    public CharacterController characterController;

    [Header("Axis Override")]
    public Vector3 localForwardAxis = Vector3.right;
    public Vector3 worldUpAxis = Vector3.up;

    [Header("Exit Settings")]
    public Transform exitPoint;     

    public KeyCode exitKey = KeyCode.E;

    private Rigidbody rb;
    private bool canDrive = false;
    private float currentSpeed = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void EnableDriving()
    {
        canDrive = true;

        moveAction.action.Enable();
        turnAction.action.Enable();

        Debug.Log("Entered vehicle");
    }

    public void ExitVehicle()
    {
        canDrive = false;
        currentSpeed = 0f;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        moveAction.action.Disable();
        turnAction.action.Disable();

        if (hmdNavigation != null)
            hmdNavigation.enabled = true;

        if (characterController != null)
            characterController.enabled = true;

        if (exitPoint != null)
        {
            xrOrigin.position = exitPoint.position;
        }

        Debug.Log("Exited vehicle");
    }

    void FixedUpdate()
    {
        if (!canDrive) return;

        Vector2 move = moveAction.action.ReadValue<Vector2>();
        Vector2 turn = turnAction.action.ReadValue<Vector2>();

        float targetSpeed = move.y * maxSpeed;
        currentSpeed = Mathf.Lerp(
            currentSpeed,
            targetSpeed,
            acceleration * Time.fixedDeltaTime
        );

        Vector3 forwardWorld =
            transform.TransformDirection(localForwardAxis).normalized;

        rb.MovePosition(
            rb.position + forwardWorld * currentSpeed * Time.fixedDeltaTime
        );

        float steer = Mathf.Clamp(turn.x, -1f, 1f);

        Quaternion turnRotation =
            Quaternion.AngleAxis(
                steer * turnSpeed * Time.fixedDeltaTime,
                worldUpAxis
            );

        rb.MoveRotation(turnRotation * rb.rotation);
    }

    void LateUpdate()
    {
        if (!canDrive) return;

        Camera cam = xrOrigin.GetComponentInChildren<Camera>();
        Vector3 offset = xrOrigin.position - cam.transform.position;
        xrOrigin.position = seatPoint.position + offset;

        if (Input.GetKeyDown(exitKey))
        {
            ExitVehicle();
        }
    }
}

