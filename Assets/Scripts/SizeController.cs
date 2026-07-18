using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

/// <summary>
/// Adds the size manipulation provider to the scene
/// </summary>
public class SizeController : MonoBehaviour, GraphicsController
{
    // Inputs
    [Header("Input")]
    public InputActionProperty growAction;
    public InputActionProperty shrinkAction;

    [Header("Size Level")]
    [Range(-2, 2)]
    public int sizeLevel = 0;
    [Header("Smooth Transition")]
    public float transitionSpeed = 0.8f;
    public Vector3 normalBodyScale { get; private set; }
    public Vector3 targetScale { get; private set; }

    private SizeManipulationProvider _sizeComponent;
    private Transform _avatarBody;
    public bool isLocal { get; private set; } = false;
    public DynamicMoveProvider moveProvider;
    // Expected default speed
    private float _normalSpeed;
    // For Network sync
    AvatarGraphicsSynchronizer _graphicsSynchronizer;
    private string _uuidAvatar;

    private void Start()
    {
        // Get This avatar
        Ubiq.Avatars.Avatar thisAvatar = gameObject.GetComponent<Ubiq.Avatars.Avatar>();
        isLocal = thisAvatar.IsLocal;
        // Do not proceed if this is a remote avatar copy
        if (!isLocal)
        {
            return;
        }
        _uuidAvatar = thisAvatar.Peer.uuid;

        // Find Graphics updater in the scene to change look of remote copies of the avatar
        _graphicsSynchronizer = FindFirstObjectByType<AvatarGraphicsSynchronizer>();

        if (_graphicsSynchronizer == null)
        {
            Debug.LogError("Graphics Synchronizer not found! Avatar Graphics will not be updated.");
        }

        // Find move provider (needed to regulate speed depending on user size)
        if (moveProvider == null)
        {
            moveProvider = FindFirstObjectByType<DynamicMoveProvider>();
            _normalSpeed = moveProvider.moveSpeed;
        }

        // Components needed for correct scaling
        GameObject xrOrigin = FindFirstObjectByType<XROrigin>().gameObject;
        _avatarBody = transform;
        // Initialize scaling parameters
        normalBodyScale = Vector3.one;
        targetScale = normalBodyScale;
        // Add component
        _sizeComponent = xrOrigin.GetComponent<SizeManipulationProvider>();
        _sizeComponent.enabled = true;
    }

    void FixedUpdate()
    {
        // Skip for avatars that do not have the right power
        if (!isLocal)
        {
            return;
        }

        // Only change target when previous one has been reached
        if (targetScale == _avatarBody.localScale)
        {
            if (growAction.action != null &&
                growAction.action.WasPressedThisFrame())
            {
                ChangeSize(1);
            }
            else if (shrinkAction.action != null &&
                shrinkAction.action.WasPressedThisFrame())
            {
                ChangeSize(-1);
            }
            return;
        }

        // Distance from target scale: it is measured only along one axis, as 
        // scaling affects all 3
        float distance = targetScale.x - _avatarBody.localScale.x;
        float sign = distance / (Math.Abs(distance));
        float deltaScale = sign * Time.deltaTime * transitionSpeed;

        if ((sign > 0 && _avatarBody.localScale.x + deltaScale > targetScale.x) ||
        (sign < 0 && _avatarBody.localScale.x + deltaScale < targetScale.x))
        {
            // Snap to Scale
            _avatarBody.localScale = targetScale;
        }
        else
        {
            // grow/shrink further
            _avatarBody.localScale += deltaScale * Vector3.one;
        }
        // Update graphics (i.e. eyes) of remote copies
        if (_graphicsSynchronizer != null)
        {
            _graphicsSynchronizer.SendTrackMessage(_uuidAvatar, ScenePowerManager.Power.sizeManipulationPower, _avatarBody.localScale.x);
        }

    }

    private void ChangeSize(int direction)
    {
        sizeLevel = Mathf.Clamp(sizeLevel + direction, -2, 1);

        Debug.Log("Avatar size level: " + sizeLevel);

        float scaleFactor;

        switch (sizeLevel)
        {
            case -2:
                scaleFactor = 0.2f;
                break;

            case -1:
                scaleFactor = 0.5f;
                break;

            case 1:
                scaleFactor = 1.5f;
                break;

            default:
                scaleFactor = 1f;
                break;

        }

        targetScale = normalBodyScale * scaleFactor;
        // Adjust Velocity
        if (moveProvider != null)
        {
            moveProvider.moveSpeed = _normalSpeed * (1/scaleFactor);
        }
    }

    private void OnEnable()
    {
        if (growAction.action != null)
            growAction.action.Enable();

        if (shrinkAction.action != null)
            shrinkAction.action.Enable();
    }

    private void OnDestroy()
    {
        if (growAction.action != null)
            growAction.action.Disable();

        if (shrinkAction.action != null)
            shrinkAction.action.Disable();

        _sizeComponent.enabled = false;
    }

    // Returns whether or not current scale is equal to the target one
    public bool HasReachedGoal()
    {
        return (targetScale == _avatarBody.localScale);
    }

    // Returns current avatar scale
    public Vector3 GetAvatarScale()
    {
        return _avatarBody.localScale;
    }

    /// <summary>
    /// Propagates Avatar Scaling to remote copies
    /// </summary>
    /// <param name="param"></param>
    public void UpdateGraphics(float param)
    {
        // Update avatar's scale
        transform.localScale = Vector3.one * param;
    }
}
