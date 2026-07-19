using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Ubiq.Messaging;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Component to <see langword="add"/> to XR grabbabble <see langword="object"/> to 
/// enable network synchronization <see langword="and"/> outline upon hovering 
/// </summary>
public class GrabbableElement : MonoBehaviour
{
    // Enable/Disable outline
    private Outline _outlineComponent;
    public XRGrabInteractable grabInteractable;
    // For network synchronization: it is not editable in the inspector
    [System.NonSerialized]
    public bool owner = false;                      // are you currently grabbing/interacting with the object
    NetworkContext context;
    // Needed to keep tracking object while it is "falling" after grab release
    private bool _waitForFall = false;
    private Vector3 _prevPosition = new Vector3(0, -100, 0);         // to determine if object has stopped
    // For more adaptability, check what is the original enable status of the grabbable object 
    // & record it to reest status after grab release
    private bool _defaultGrabbableStatus;
    // Keep track of when user received first grab enter tracking message
    [System.NonSerialized]
    public bool firstMessageArrived = false;

    void Start()
    {
        // Add outline component
        _outlineComponent = gameObject.AddComponent<Outline>();
        _outlineComponent.enabled = false;
        // Set up outline look
        _outlineComponent.OutlineWidth = 4;
        grabInteractable = GetComponent<XRGrabInteractable>();
        // Network
        context = NetworkScene.Register(this);

        if (grabInteractable != null)
        {
            // This is a grabbable object
            _outlineComponent.OutlineColor = Color.white;
            // Set interaction layer to Grabbable Objects
            int grabLayer = InteractionLayerMask.GetMask("Grabbable Objects");
            if (grabLayer == 0)
            {
                Debug.LogWarning("'Grabbable Objects' interaction layer does not exist in project settings!\n Grab interaction may not work.");
            }
            int oldLayerMask = grabInteractable.interactionLayers.value;
            grabInteractable.interactionLayers = oldLayerMask | grabLayer;
            grabInteractable.hoverEntered.AddListener(EnableHighlight);
            grabInteractable.hoverExited.AddListener(DisableHighlight);
            // Network Management
            grabInteractable.selectEntered.AddListener(OnSelect);
            grabInteractable.selectExited.AddListener(OnDeselect);

        }

    }

    private void FixedUpdate()
    {
        if (owner)
        {
            Vector3 currentPosition = transform.position;
            if (currentPosition != _prevPosition)
            {
                // Object moved: user is either grabbing or object is falling
                // Send network message to make update copies' position & rotation
                SendTrackMessage();
                _prevPosition = currentPosition;
            }
            else
            {
                // Object is still
                if (_waitForFall)
                {
                    // Object fall ended; release grab
                    owner = false;
                    _waitForFall = false;
                    _prevPosition = new Vector3(0, -100, 0);
                    Debug.Log("Release");
                    SendReleaseMessage();
                }
            }
        }
    }

    /// <summary>
    /// Enable Outline highlight when hovering  
    /// </summary>
    public void EnableHighlight(HoverEnterEventArgs args)
    {
        // Skip if it is caused by socket snap
        if (args.interactorObject.transform.gameObject.GetComponent<NearFarInteractor>() == null)
        {
            // Not a user-caused interaction
            return;
        }
        _outlineComponent.enabled = true;
    }

    /// <summary>
    /// Disable Outline highlight when exiting hover state 
    /// </summary>
    public void DisableHighlight(HoverExitEventArgs args)
    {
        _outlineComponent.enabled = false;
    }

    // ------------------------------------------------------------------------------------
    // Network sync management

    /// <summary>
    /// When object is grabbed start tracking position
    /// </summary>
    private void OnSelect(SelectEnterEventArgs args)
    {
        // Skip if it is caused by socket snap
        if (args.interactorObject.transform.gameObject.GetComponent<NearFarInteractor>() == null)
        {
            // Not a user-caused interaction
            return;
        }
        // Local avatar becomes owner
        owner = true;
    }

    /// <summary>
    /// When object is released, keep tracking as long as the tracked object is still falling  
    /// </summary>
    private void OnDeselect(SelectExitEventArgs args)
    {
        _waitForFall = true;
    }

    private struct TrackGrabMsg
    {
        public Vector3 position;
        public Quaternion rotation;
        public bool release;
    }

    // Send grab tracking message
    private void SendTrackMessage()
    {
        var message = new TrackGrabMsg();
        message.position = transform.position;
        message.rotation = transform.rotation;
        message.release = false;
        context.SendJson(message);
    }

    // Send grab tracking message to release object
    private void SendReleaseMessage()
    {
        var message = new TrackGrabMsg();
        message.position = transform.position;
        message.rotation = transform.rotation;
        //message.position = transform.localPosition;
        //message.rotation = transform.localRotation;
        message.release = true;
        context.SendJson(message);
    }

    // Network Ubiq: track rotation & position of grabbable objects
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        // Parse message
        var m = message.FromJson<TrackGrabMsg>();

        if (!m.release)
        {
            // Things to do only once
            if (!firstMessageArrived)
            {
                // Someone grabbed object
                owner = false;
                // Store Grabbable component status before starting to track
                _defaultGrabbableStatus = grabInteractable.enabled;
                // Disable Grabbable component
                if (grabInteractable.enabled)
                {
                    grabInteractable.enabled = false;
                }
                // Make rigidbody kinematic
                GetComponent<Rigidbody>().isKinematic = true;
                firstMessageArrived = true;
            }
            // Keep tracking position & rotation
            // For testing in loopback
            gameObject.transform.position = m.position;
            gameObject.transform.rotation = m.rotation;
            //gameObject.transform.localPosition = m.position;
            //gameObject.transform.localRotation = m.rotation;
        }
        else
        {
            // Release grab: reset to previous state
            GetComponent<Rigidbody>().isKinematic = false;
            grabInteractable.enabled = _defaultGrabbableStatus;
            // Reset
            firstMessageArrived = false;
        }
    }
}