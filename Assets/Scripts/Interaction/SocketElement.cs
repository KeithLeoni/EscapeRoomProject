using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// This component controls snap socket highlighting  
/// </summary>
public class SocketElement : MonoBehaviour, IXRSelectFilter
{
    // Enable/Disable outline
    private Outline _outlineComponent;
    private XRSocketInteractor _socketInteractor;
    // Tracks if object is currently selected by socket
    private bool _isSelected = false;

    void Start()
    {
        // Add outline component
        _outlineComponent = gameObject.AddComponent<Outline>();
        _outlineComponent.enabled = false;
        // Set up outline look
        _outlineComponent.OutlineWidth = 4;
        _socketInteractor = GetComponent<XRSocketInteractor>();

        if (_socketInteractor != null)
        {
            // This is a socket interactor object
            _outlineComponent.OutlineColor = Color.gold;
            _socketInteractor.hoverEntered.AddListener(EnableHighlight);
            _socketInteractor.hoverExited.AddListener(DisableHighlight);
            _socketInteractor.selectEntered.AddListener(OnSnap);
            _socketInteractor.selectExited.AddListener(OnSelectExit);
        }

    }

    public void EnableHighlight(HoverEnterEventArgs args)
    {
        if (_isSelected)
        {
            // Do not highlight socket
            return;
        }
        _outlineComponent.enabled = true;
    }

    public void DisableHighlight(HoverExitEventArgs args)
    {
        _outlineComponent.enabled = false;
    }

    /// <summary>
    /// When objects enter snapping zone (i.e are selected) this function
    /// disables outline 
    /// </summary>
    public void OnSnap(SelectEnterEventArgs args)
    {
        // Disable socket outline
        _outlineComponent.enabled = false;
        _isSelected = true;
    }

    public void OnSelectExit(SelectExitEventArgs args)
    {
        // Update selected status
        _isSelected = false;
    }

    // Socket Filter: 
    // If Object it is interacting with:
    // - is not being carried by local avatar
    // - is not released
    // reject interaction (objects should snap only when remote avatar releases item)
    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        // Check interactable
        GrabbableElement grabbableElement = interactable.transform.gameObject.GetComponent<GrabbableElement>();
        
        return grabbableElement == null || (!grabbableElement.firstMessageArrived);
    }

    public bool canProcess => isActiveAndEnabled;
}