using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

/// <summary>
/// Component to control flying mechanics
/// </summary>
public class FlyLocomotionProvider : MonoBehaviour
{
    // Controller Input 
    [Header("Input Actions")]
    [Tooltip("Input Action for movement for 2d movement")]
    public InputActionProperty moveAction;
    [Tooltip("Input Action for vertical movement (Ascend)")]
    public InputActionProperty ascendAction;
    [Tooltip("Input Action for vertical movement (Descend)")]
    public InputActionProperty descendAction;

    // Physics settings
    [Header("Settings")]
    public float forwardSpeed = 3f;         // Forward/Backward speed
    public float verticalSpeed = 2.5f;      // Up/Down speed
    public float strafeSpeed = 2f;          // Left/Right speed

    // Component needed for computations
    private Camera _mainCamera;
    private CharacterController _characterController;
    private GameObject _groundedMoveProvider;

    private void Awake()
    {
        // Extrapolate from the scene XROrigin & move providers
        GameObject xrOrigin = FindFirstObjectByType<XROrigin>().gameObject;

        // Setup camera and initialize flyEnabled variable
        _mainCamera = Camera.main;
        _characterController = xrOrigin.GetComponent<CharacterController>();
        if (_characterController == null)
        {
            Debug.LogError("Character Controller not found in the hierarchy");
        }

        // Find move providers
        _groundedMoveProvider = xrOrigin.GetComponentInChildren<DynamicMoveProvider>().gameObject;
        _groundedMoveProvider.SetActive(false);
    }

    private void OnEnable()
    {
        // Enable actions
        moveAction.action.Enable();
        ascendAction.action.Enable();
        descendAction.action.Enable();
    }

    // Temporarly commented to avoid input disabling conflicts with avatarsizecontroller
    // when usin the testing menu
    private void OnDisable()
    {
        _groundedMoveProvider.SetActive(true);
        /*
        ascendAction.action.Disable();
        descendAction.action.Disable();
        */
    }

    private void FixedUpdate()
    {
        Vector3 move = Vector3.zero;
        // Get head/camera forward/right for movement direction
        Transform head = _mainCamera.transform;
        Vector3 forward = head.forward;
        Vector3 right = head.right;
        Vector3 up = head.up;
        float verticalInput = 0;

        // Read joystick input
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        // Handle hold interaction
        if (ascendAction.action.IsPressed())
        {
            // Annul vertical movement if both buttons are pressed at the same time
            if (!descendAction.action.IsPressed())
            {
                verticalInput = 1;
            }
        }
        else if (descendAction.action.IsPressed())
        {
            if (!ascendAction.action.IsPressed())
            {
                verticalInput = -1;
            }
        }

        // Flatten forward/right to avoid unwanted vertical tilt
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // Calculate movement vector
        move = forward * moveInput.y * forwardSpeed + right * moveInput.x * strafeSpeed;
        // Add vertical movement: check that vertical displacement does not put camera below minHeight
        Vector3 verticalMove = up * verticalInput * verticalSpeed;
        move += verticalMove;
        // Apply movement in world space and multiply by delta time
        move *= Time.deltaTime;
        _characterController.Move(move);

    }
}
