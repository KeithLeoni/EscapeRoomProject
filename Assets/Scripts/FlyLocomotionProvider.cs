using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

/// <summary>
/// Create an empty <see langword="object"/> <see langword="and"/> <see langword="add"/> <see langword="this"/>
/// component to create a locomotion flying system. Add the <see langword="object"/> under the Locomotion <see langword="object"/>
/// <see langword="in"/> the XR Rig  
/// </summary>
public class FlyLocomotionProvider : MonoBehaviour
{
    [Header("Scene power management object")]
    [Tooltip("Object used to handle power activation")]
    public ScenePowerManager scenePowerManager;
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
    private float _minHeight = 2;          // Minimum height accepted when in flying mode
    public float forwardSpeed = 3f;         // Forward/Backward speed
    public float verticalSpeed = 2.5f;      // Up/Down speed
    public float strafeSpeed = 2f;          // Left/Right speed

    private Camera _mainCamera;
    private CharacterController _characterController;

    private void Awake()
    {
        // Setup camera and initialize flyEnabled variable
        _mainCamera = Camera.main;
        _characterController = GetComponentInParent<CharacterController>();
        if (_characterController == null)
        {
            Debug.LogWarning("Character Controller not found in the hierarchy");
        }

    }

    private void OnEnable()
    {
        // Enable actions
        moveAction.action.Enable();
        ascendAction.action.Enable();
        descendAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        ascendAction.action.Disable();
        descendAction.action.Disable();
    }

    private void Update()
    {
        if (scenePowerManager.flyingPower)
        {
            Vector3 move = Vector3.zero;
            // Get head/camera forward/right for movement direction
            Transform head = _mainCamera.transform;
            Vector3 forward = head.forward;
            Vector3 right = head.right;
            Vector3 up = head.up;
            float verticalInput = 0;

            if (_mainCamera.transform.position.y >= _minHeight)
            {
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
                if (_mainCamera.transform.position.y + (verticalMove.y * Time.deltaTime) >= _minHeight)
                {
                    move += verticalMove;
                }
            }
            else
            {
                // If flight mode has just been enabled float upwards to minimum height
                move += up * verticalSpeed;
            }

            // Apply movement in world space and multiply by delta time
            move *= Time.deltaTime;
            _characterController.Move(move);     
        }
        else if (scenePowerManager.landFlying)
        {
            // Apply downward force until player lands
            if (!_characterController.isGrounded)
            {
                _characterController.Move(-Vector3.up * 9.8f * Time.deltaTime);
            }
            else
            {
                scenePowerManager.landFlying = false;
                // Enable other locomotion providers & force player to the ground
                for (int i = 0; i < scenePowerManager.groundedMoveProvider.Length; i++)
                {
                    scenePowerManager.groundedMoveProvider[i].SetActive(true);
                }
            }
        }
    }
}
