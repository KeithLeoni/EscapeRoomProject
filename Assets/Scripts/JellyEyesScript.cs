using UnityEngine;
using Ubiq.Avatars;
using UnityEngine.InputSystem;

public class JellyEyesScript : MonoBehaviour, GraphicsController
{
    [Header("Setup Eyes")]
    public GameObject leftEyeSphere;
    public GameObject rightEyeSphere;
    // Camera Setup
    private Camera _localCamera;
    private int _eyeHiddenLayerIndex = -1;
    private int _traceLayerIndex = -1; // Added back to track the trace layer safely
    // Input action
    [Tooltip("Input Action to activate power")]
    public InputActionProperty triggerAction;
    // Tracker of power activation
    private bool _activationStatus = false;
    // Avatar Graphics update setup
    private AvatarGraphicsSynchronizer _graphicsSynchronizer;
    private string _uuidAvatar;

    void Start()
    {
        // Find Graphics updater in the scene to change look of remote copies of the avatar
        _graphicsSynchronizer = FindFirstObjectByType<AvatarGraphicsSynchronizer>();
        if (_graphicsSynchronizer == null)
        {
            Debug.LogError("Graphics Synchronizer not found! Avatar Graphics will not be updated.");
        }

        // Do not proceed if it is not your local avatar
        if (!gameObject.GetComponent<Ubiq.Avatars.Avatar>().IsLocal)
        {
            return;
        }
        _uuidAvatar = gameObject.GetComponent<Ubiq.Avatars.Avatar>().Peer.uuid;

        _localCamera = Camera.main;
        if (_localCamera == null) _localCamera = GetComponentInChildren<Camera>();

        _eyeHiddenLayerIndex = LayerMask.NameToLayer("LocalPlayerHidden");
        _traceLayerIndex = LayerMask.NameToLayer("JellyTraces");

        // Hide eyes for the player with this power (local avatar)
        if (_eyeHiddenLayerIndex != -1)
        {
            if (leftEyeSphere != null) leftEyeSphere.layer = _eyeHiddenLayerIndex;
            if (rightEyeSphere != null) rightEyeSphere.layer = _eyeHiddenLayerIndex;

            if (_localCamera != null)
            {
                _localCamera.cullingMask &= ~(1 << _eyeHiddenLayerIndex);
            }
        }

        // Hide traces by default on startup for your local camera view
        if (_traceLayerIndex != -1 && _localCamera != null)
        {
            _localCamera.cullingMask &= ~(1 << _traceLayerIndex);
        }
    }

    void Update()
    {
        if (triggerAction.action != null &&
            triggerAction.action.WasPressedThisFrame())
        {
            _activationStatus = !_activationStatus;
            // Activate power
            ForceToggleLocalTraces(_activationStatus);
            // Update graphics (i.e. eyes) of remote copies
            if (_graphicsSynchronizer != null)
            {
                _graphicsSynchronizer.SendTrackMessage(_uuidAvatar, ScenePowerManager.Power.jellyVision);
            }
        }
    }

    // Direct Push: Called by the manager to change physical eye meshes globally
    public void SetEyesActive(bool state)
    {
        if (leftEyeSphere != null) leftEyeSphere.SetActive(state);
        if (rightEyeSphere != null) rightEyeSphere.SetActive(state);
    }

    public void ForceToggleLocalTraces(bool state)
    {
        if (_localCamera == null) _localCamera = Camera.main;

        if (_localCamera != null)
        {
            if (state)
            {
                _localCamera.cullingMask |= (1 << _traceLayerIndex); // Open your eyes layer view
            }
            else
            {
                _localCamera.cullingMask &= ~(1 << _traceLayerIndex); // Close your eyes layer view
            }

            Debug.Log($"[Traces System] Privately adjusted traces visibility to: {state}");
        }
    }

    void OnEnable()
    {
        triggerAction.action.Enable();
    }

    void OnDestroy()
    {
        // Reset Layer visibility to standard
        ForceToggleLocalTraces(false);
        // Make Eye Layer visible again
        _localCamera.cullingMask |= (1 << _eyeHiddenLayerIndex);
    }

    // Update avatar graphics: this is done for the remote instances
    public void UpdateGraphics()
    {
        // Change JellyEyes viibility
        bool currentState = rightEyeSphere.activeSelf;
        SetEyesActive(!currentState);
    }
}