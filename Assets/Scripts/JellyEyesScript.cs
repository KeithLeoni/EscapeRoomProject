using UnityEngine;
using Ubiq.Avatars;
using UnityEngine.InputSystem;

public class JellyEyesScript : MonoBehaviour
{
    [Header("Setup Eyes")]
    public GameObject leftEyeSphere;
    public GameObject rightEyeSphere;

    private Camera _localCamera;
    private int _eyeHiddenLayerIndex = -1;
    private int _traceLayerIndex = -1; // Added back to track the trace layer safely

    [Tooltip("Input Action to activate power")]
    public InputActionProperty triggerAction;
    private bool _activationStatus = false;
    void Start()
    {
        // Get avatar component
        //Ubiq.Avatars.Avatar avatarComponent = GetComponent<Ubiq.Avatars.Avatar>();
        //if (avatarComponent == null) avatarComponent = GetComponentInParent<Ubiq.Avatars.Avatar>();

        //if (avatarComponent != null && avatarComponent.IsLocal)
        //{
            _localCamera = Camera.main;
            if (_localCamera == null) _localCamera = GetComponentInChildren<Camera>();

            _eyeHiddenLayerIndex = LayerMask.NameToLayer("LocalPlayerHidden");
            _traceLayerIndex = LayerMask.NameToLayer("JellyTraces");

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
        //}

        // Start turned off
        SetEyesActive(false);
    }

    void Update()
    {
        if (triggerAction.action != null &&
            triggerAction.action.WasPressedThisFrame())
        {
            _activationStatus = !_activationStatus;
            // Activate power
            ForceToggleLocalTraces(_activationStatus);
            SetEyesActive(_activationStatus);
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
}