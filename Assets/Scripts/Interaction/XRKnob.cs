using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System;

/// <summary>
/// Support continuos movement/rotation.
/// </summary>
public class XRKnob : XRBaseInteractable
{
    public GameObject knobObject;
    // Compute delta angle between frames to apply to rotatable object
    private IXRSelectInteractor _controllerObject;
    private Vector3 _prevControllerAngle;
    private Vector3 _defaultRotation;

    // Countdown to an angle difference of 36 degrees before snapping to next _sectionAtStart
    private float _snapCountDown = 0f; 
    // Implement object outline
    private Outline _outlineComponent;

    void Start()
    {
        // Add outline component
        _outlineComponent = gameObject.AddComponent<Outline>();
        _outlineComponent.enabled = false;
        // Set up outline look
        _outlineComponent.OutlineWidth = 4;
        _defaultRotation = knobObject.transform.localEulerAngles;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        selectEntered.AddListener(StartGrab);
        // Outline component management
        hoverEntered.AddListener(EnableHighlight);
        hoverExited.AddListener(DisableHighlight);
    }

    protected override void OnDisable()
    {
        selectEntered.RemoveListener(StartGrab);
        // Outline component
        hoverEntered.RemoveListener(EnableHighlight);
        hoverExited.RemoveListener(DisableHighlight);
        base.OnDisable();
    }

    /// <summary>
    /// When an object is grabbed/selected it saves controller rotation
    /// </summary>
    void StartGrab(SelectEnterEventArgs args)
    {
        // Save current rotation of controller
        _controllerObject = args.interactorObject;
        // Reset Countdown
        _snapCountDown = 0f;
        _prevControllerAngle = ProjectOnKnob(_controllerObject.GetAttachTransform(this).rotation.eulerAngles);//_controllerObject.GetAttachTransform(this).right;
    }

    /// <summary>
    /// Enable Outline highlight when hovering  
    /// </summary>
    void EnableHighlight(HoverEnterEventArgs args)
    {
        _outlineComponent.enabled = true;
    }

    /// <summary>
    /// Given an integer it returns the corresponding number in the circular range [0, 9]
    /// </summary>
    /// <param name="number">initial integer to adapt to circular range </param>
    /// <returns>positive integer from 0 to 9 </returns>
    private int ClampCircularRange(int number)
    {
        // numbers from -9 to 9
        int range = number % 10;
        return range >= 0 ? range : (10 + range);
    } 
    
    /// <summary>
    /// Returns the projection of the given angle on the knob object surface plane
    /// </summary>
    private Vector3 ProjectOnKnob(Vector3 angle)
    {
        return Vector3.ProjectOnPlane(angle, knobObject.transform.forward);
    }

    /// <summary>
    /// Disable Outline highlight when exiting hover state 
    /// </summary>
    void DisableHighlight(HoverExitEventArgs args)
    {
        _outlineComponent.enabled = false;
    }

    public int GetCurrentSector()
    {
        float currentZ = knobObject.transform.localRotation.eulerAngles.z - _defaultRotation.z;
        return ClampCircularRange(Mathf.RoundToInt(currentZ / 36f));
    }
    
    /// <summary>
    /// While object is selected, rotate along the given axis 
    /// </summary>
    void UpdateRotation()
    {
        // Get new rotation angle of the controller
        Vector3 newControllerProjection = ProjectOnKnob(_controllerObject.GetAttachTransform(this).rotation.eulerAngles);//_controllerObject.GetAttachTransform(this).right;
        // Compute difference of angles
        float rollAngle = -Vector3.SignedAngle(_prevControllerAngle, newControllerProjection, knobObject.transform.forward);
        _snapCountDown += rollAngle;
        // Consider the offset of the initial rotation
        int currentSector = GetCurrentSector();
        int snapTo = currentSector;
        // Check Countdown
        if (Math.Abs(_snapCountDown) >= 36f)
        {
            // Snap to next/previous notch
            if (_snapCountDown >= 0)
            {
                // Snap to next
                snapTo = ClampCircularRange(snapTo + 1);
            }
            else
            {
                snapTo = ClampCircularRange(snapTo - 1);
            }
            // Reset
            _snapCountDown = 0f;
            knobObject.transform.localRotation = Quaternion.Euler(_defaultRotation.x, _defaultRotation.y, _defaultRotation.z + (snapTo * 36f));
        }
        // Save new rotation position
        _prevControllerAngle = newControllerProjection;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);
        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            if (isSelected)
            {
                UpdateRotation();
            }
        }
    }
}
