using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Support continuos movement/rotation wrt a given axis
/// </summary>
public class XRKnob : XRBaseInteractable
{
    // Objects for rotation
    [Header("Rotation Settings")]
    public Vector3 rotationAxis;
    public GameObject knobObject;
    // Compute delta angle between frames to apply to rotatable object
    private IXRSelectInteractor _controllerObject;
    private Quaternion _prevControllerAngle;
    // Implement object outline
    private Outline _outlineComponent;

    void Start()
    {
        // Add outline component
        _outlineComponent = gameObject.AddComponent<Outline>();
        _outlineComponent.enabled = false;
        // Set up outline look
        _outlineComponent.OutlineWidth = 4;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        selectEntered.AddListener(StartGrab);
        // Outline component management
        hoverEntered.AddListener(enableHighlight);
        hoverExited.AddListener(disableHighlight);
    }

    protected override void OnDisable()
    {
        selectEntered.RemoveListener(StartGrab);
        // Outline component
        hoverEntered.RemoveListener(enableHighlight);
        hoverExited.RemoveListener(disableHighlight);
        base.OnDisable();
    }

    /// <summary>
    /// When an object is grabbed/selected it saves controller rotation
    /// </summary>
    void StartGrab(SelectEnterEventArgs args)
    {
        // Save current rotation of controller
        _controllerObject = args.interactorObject;
        _prevControllerAngle = _controllerObject.GetAttachTransform(this).rotation;
    }

    /// <summary>
    /// Enable Outline highlight when hovering  
    /// </summary>
    void enableHighlight(HoverEnterEventArgs args)
    {
        _outlineComponent.enabled = true;
    }

    /// <summary>
    /// Disable Outline highlight when exiting hover state 
    /// </summary>
    void disableHighlight(HoverExitEventArgs args)
    {
        _outlineComponent.enabled = false;
    }
    
    /// <summary>
    /// While object is selected, rotate along the given axis 
    /// </summary>
    void UpdateRotation()
    {
        // Get new rotation angle of the controller
        Transform newControllerTransform = _controllerObject.GetAttachTransform(this);
        // Compute difference of angles
        Quaternion relativeDiff = Quaternion.Inverse(_prevControllerAngle) * newControllerTransform.rotation;
        float prevEuler = _prevControllerAngle.eulerAngles.x;
        float newEuler = newControllerTransform.rotation.eulerAngles.x;
        // Rotate object
        knobObject.transform.Rotate((newEuler - prevEuler) * rotationAxis);
        // Save new rotation position
        _prevControllerAngle = newControllerTransform.rotation;
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
