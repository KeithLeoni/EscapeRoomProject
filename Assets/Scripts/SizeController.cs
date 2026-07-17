using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

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
    // For Network sync
    AvatarGraphicsSynchronizer _graphicsSynchronizer;
    private string _uuidAvatar;

    private void Start()
    {
        if (!transform.gameObject.GetComponent<Ubiq.Avatars.Avatar>().IsLocal)
        {
            return;
        }

        // Find Graphics updater in the scene to change look of remote copies of the avatar
        _graphicsSynchronizer = FindFirstObjectByType<AvatarGraphicsSynchronizer>();

        if (_graphicsSynchronizer == null)
        {
            Debug.LogError("Graphics Synchronizer not found! Avatar Graphics will not be updated.");
        }
        _uuidAvatar = gameObject.GetComponent<Ubiq.Avatars.Avatar>().Peer.uuid;

        isLocal = true;
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

        switch (sizeLevel)
        {
            case -2:
                targetScale = normalBodyScale * 0.2f;
                break;

            case -1:
                targetScale = normalBodyScale * 0.5f;
                break;

            case 0:
                targetScale = normalBodyScale;
                break;

            case 1:
                targetScale = normalBodyScale * 2f;
                break;

            case 2:
                targetScale = normalBodyScale * 2.5f;
                break;
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

    public void UpdateGraphics(float param)
    {
        // Update avatar's scale
        transform.localScale = Vector3.one * param;
    }
}
