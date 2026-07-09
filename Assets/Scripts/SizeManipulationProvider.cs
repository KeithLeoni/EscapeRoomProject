using Ubiq.Avatars;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

public class SizeManipulationProvider : MonoBehaviour
{
    [Header("Input")]
    public InputActionProperty growAction;    // U / A
    public InputActionProperty shrinkAction;  // J / B

    [Header("Avatar Visual")]
    public Transform targetToScale;

    [Header("Camera")]
    public Transform cameraOffset;

    [Header("Size Level")]
    [Range(-2, 2)]
    public int sizeLevel = 0;

    [Header("Smooth Transition")]
    public float transitionSpeed = 3f;

    private Vector3 _normalBodyScale;
    private Vector3 _normalBodyPosition;
    private Vector3 _normalCameraOffsetPosition;
    private Transform _xrOrigin;

    void Start()
    {
        if (cameraOffset == null)
        {
            GameObject cameraOffsetObj = GameObject.Find("Camera Offset");
            if (cameraOffsetObj != null)
                cameraOffset = cameraOffsetObj.transform;
        }

        if (cameraOffset != null)
            _normalCameraOffsetPosition = cameraOffset.localPosition;
        _normalBodyScale = Vector3.one;
        // Try
        //_xrOrigin = GetComponentInParent<XROrigin>().gameObject.transform;  
    }

    public void Initialize(Transform localAvatar)
    {
        targetToScale = localAvatar.transform;
        _normalBodyPosition = targetToScale.localPosition;      
    }

    void OnEnable()
    {
        if (growAction.action != null)
            growAction.action.Enable();

        if (shrinkAction.action != null)
            shrinkAction.action.Enable();

    }

    void Update()
    {
        if (targetToScale == null)
        {
            return;
        }

        if (growAction.action != null && growAction.action.WasPressedThisFrame())
        {
            ChangeSize(1);
        }

        if (shrinkAction.action != null && shrinkAction.action.WasPressedThisFrame())
        {
            ChangeSize(-1);
        }

        ApplyScale();
    }

    private void ChangeSize(int direction)
    {
        sizeLevel += direction;
        sizeLevel = Mathf.Clamp(sizeLevel, -2, 2);

        Debug.Log("Avatar size level: " + sizeLevel);
    }

    private void ApplyScale()
    {
        Vector3 targetScale = _normalBodyScale;
        Vector3 targetBodyPosition = _normalBodyPosition;
        Vector3 targetCameraPosition = _normalCameraOffsetPosition;

        switch (sizeLevel)
        {
            case -2:
                targetScale = new Vector3(0.2f, 0.2f, 0.2f);
                targetBodyPosition = _normalBodyPosition + new Vector3(0f, -0.6f, 0f);
                targetCameraPosition = _normalCameraOffsetPosition + new Vector3(0f, -0.6f, 0f);
                break;

            case -1:
                targetScale = new Vector3(0.5f, 0.5f, 0.5f);
                targetBodyPosition = _normalBodyPosition + new Vector3(0f, -0.25f, 0f);
                targetCameraPosition = _normalCameraOffsetPosition + new Vector3(0f, -0.25f, 0f);
                break;

            case 0:
                targetScale = _normalBodyScale;
                targetBodyPosition = _normalBodyPosition;
                targetCameraPosition = _normalCameraOffsetPosition;
                break;

            case 1:
                targetScale = new Vector3(2f, 2f, 2f);
                targetBodyPosition = _normalBodyPosition + new Vector3(0f, 0.5f, 0f);
                targetCameraPosition = _normalCameraOffsetPosition + new Vector3(0f, 0.5f, 0f);
                break;

            case 2:
                targetScale = new Vector3(3.5f, 3.5f, 3.5f);
                targetBodyPosition = _normalBodyPosition + new Vector3(0f, 1.2f, 0f);
                targetCameraPosition = _normalCameraOffsetPosition + new Vector3(0f, 1.2f, 0f);
                break;
        }
        /*
                _xrOrigin.localScale = Vector3.Lerp(
                    targetToScale.localScale,
                    targetScale,
                    Time.deltaTime * transitionSpeed
                );*/

        targetToScale.localScale = Vector3.Lerp(
            targetToScale.localScale,
            targetScale,
            Time.deltaTime * transitionSpeed
        );

        targetToScale.localPosition = Vector3.Lerp(
            targetToScale.localPosition,
            targetBodyPosition,
            Time.deltaTime * transitionSpeed
        );

        if (cameraOffset != null)
        {
            cameraOffset.localPosition = Vector3.Lerp(
                cameraOffset.localPosition,
                targetCameraPosition,
                Time.deltaTime * transitionSpeed
            );
        }
        
    }

    void OnDisable()
    {
        // Reset scale
        sizeLevel = 0;
        ApplyScale();
        Debug.Log("Scale");
    }
    // comment needed to not have input problems when using the testing menu
    private void OnDestroy()
    {
        /*
        if (growAction.action != null)
            growAction.action.Disable();

        if (shrinkAction.action != null)
            shrinkAction.action.Disable();
            */
    }
}