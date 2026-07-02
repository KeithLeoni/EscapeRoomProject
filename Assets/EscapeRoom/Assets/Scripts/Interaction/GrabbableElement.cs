using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Ubiq.Messaging;

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
    [Header("Socket Element component (if it exists)")]
    [Tooltip("Override Ubiq Grab Behaviour. Grab tracking can be automatically triggered when an object interacts with a Socket interactor.")]
    public SocketElement socketElement;                 // object containing socket element that interacts with grabbable object (if it exists)

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
            // Was the object already realesed?
            if (_waitForFall)
            {
                if (_prevPosition != transform.localPosition)
                {
                    // Object is still falling
                    _prevPosition = transform.localPosition;
                    // Send network message to make update copies' position & rotation
                    SendTrackMessage();
                }
                else
                {
                    // Object fall ended; release grab
                    owner = false;
                    _waitForFall = false;
                    _prevPosition = new Vector3(0, -100, 0);
                    SendReleaseMessage();
                }
            }
            else
            {
                // Send network message to make update copies' position & rotation
                SendTrackMessage();
            }

        }
    }

    /// <summary>
    /// Enable Outline highlight when hovering  
    /// </summary>
    public void EnableHighlight(HoverEnterEventArgs args)
    {
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
    private void OnSelect(SelectEnterEventArgs args)
    {
        // Only track if Grab was not triggered due to Socket interaction
        if (socketElement != null && socketElement.socketTracker.IsSocketActivated())
        {
            return;
        }
        // Local avatar becomes owner
        owner = true;
    }

    private void OnDeselect(SelectExitEventArgs args)
    {
        _waitForFall = true;
    }

    private struct TrackGrabMsg
    {
        // Cannot use this to test on local loopback
        public Vector3 position;
        public Quaternion rotation;
        public bool release;
    }

    // Send grab tracking message
    private void SendTrackMessage()
    {
        var message = new TrackGrabMsg();
        message.position = transform.localPosition;
        message.rotation = transform.localRotation;
        message.release = false;
        context.SendJson(message);
    }

    // Send grab tracking message to release object
    private void SendReleaseMessage()
    {
        var message = new TrackGrabMsg();
        message.position = transform.localPosition;
        message.rotation = transform.localRotation;
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
            // Someone grabbed object
            owner = false;
            // Disable Grabbable component
            if (grabInteractable.enabled)
            {
                grabInteractable.enabled = false;
                // Make rigidbody kinematic
                GetComponent<Rigidbody>().isKinematic = true;
            }
            // Keep tracking position & rotation
            gameObject.transform.localPosition = m.position;
            gameObject.transform.localRotation = m.rotation;
        }
        else
        {
            // Release grab
            GetComponent<Rigidbody>().isKinematic = false;
            grabInteractable.enabled = true;
        }
    }
}