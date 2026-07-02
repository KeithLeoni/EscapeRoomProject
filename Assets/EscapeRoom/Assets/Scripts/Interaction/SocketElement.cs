using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// This component <see langword="is"/> used <see langword="for"/> socket snap interaction.
/// It includes outline tracking.  
/// </summary>
public class SocketElement : MonoBehaviour
{
    // Input method to call once snap interaction happens
    [Tooltip("Method to call when socket snap is triggered.")]
    public UnityEvent onSocketSnap;
    // Override Grab tracking behaviour for room synchronization
    public SocketStatusTracker socketTracker;
    // Enable/Disable outline
    private Outline _outlineComponent;
    private XRSocketInteractor _socketInteractor;

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
            _socketInteractor.hoverEntered.AddListener(enableHighlight);
            _socketInteractor.hoverExited.AddListener(disableHighlight);
            _socketInteractor.selectEntered.AddListener(onSnap);
            _socketInteractor.selectExited.AddListener(onDeselect);
            // Add socket tracker status
            socketTracker = gameObject.AddComponent<SocketStatusTracker>();
        }

    }

    public void enableHighlight(HoverEnterEventArgs args)
    {
        _outlineComponent.enabled = true;
    }

    public void disableHighlight(HoverExitEventArgs args)
    {
        _outlineComponent.enabled = false;
    }

    /// <summary>
    /// When objects enter snapping zone (i.e are selected) <see langword="this"/> function
    /// updates socket status <see langword="and"/> optionally calls the given onSocketSnap method  
    /// </summary>
    public void onSnap(SelectEnterEventArgs args)
    {
        // Update socket tracker
        socketTracker.setSocketStatus(true);
        // Disable socket
        _socketInteractor.hoverEntered.RemoveAllListeners();
        _socketInteractor.hoverExited.RemoveAllListeners();
        _outlineComponent.enabled = false;
        onSocketSnap?.Invoke();
    }

    public void onDeselect(SelectExitEventArgs args)
    {
        // Update socket tracker
        socketTracker.setSocketStatus(false);
    }

}