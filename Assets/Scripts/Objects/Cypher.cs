using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Ubiq.Messaging;

/// <summary>
/// Track inner wheel rotation for network synchronization 
/// </summary>
public class Cypher : MonoBehaviour
{
    private XRGrabInteractable _cypherInteractable;
    private XRKnob _knobInteractable;
    // Track network
    [System.NonSerialized]
    public bool owner = false;                      // are you currently grabbing/interacting with the object
    private NetworkContext _context;

    void Start()
    {
        // Enable Knob interactable only if object is being grabbed
        _cypherInteractable = GetComponent<XRGrabInteractable>();
        _knobInteractable = gameObject.GetComponentInChildren<XRKnob>();
        _cypherInteractable.selectEntered.AddListener(OnSelect);
        _cypherInteractable.selectExited.AddListener(OnDeselect);
        _knobInteractable.enabled = false;
        // Network
        _context = NetworkScene.Register(this);
        _knobInteractable.selectEntered.AddListener(StartTracking);
        _knobInteractable.selectExited.AddListener(StopTracking);
    }


    void OnSelect(SelectEnterEventArgs args)
    {
        // Deactivate box collider of grabbable object to prevent conflicts
        gameObject.GetComponent<BoxCollider>().enabled = false;
        // Activate knob interactable
        _knobInteractable.enabled = true;
    }

    void OnDeselect(SelectExitEventArgs args)
    {
        _knobInteractable.enabled = false;
        // Reactivate collider
        gameObject.GetComponent<BoxCollider>().enabled = true;
    }


    // ----------------------------------------------------------------------------------

    private void FixedUpdate()
    {
        if (owner)
        {
            // Send network message to make update copies' position & rotation
            SendTrackMessage();
        }
    }

    // Send rotation tracking message
    private struct TrackRotMsg
    {
        public Quaternion rotation;
        public bool release;
    }

    private void StartTracking(SelectEnterEventArgs args)
    {
        owner = true;
    }

    private void StopTracking(SelectExitEventArgs args)
    {
        owner = false;
        // Send release message
        SendReleaseMessage();
    }

    private void SendTrackMessage()
    {
        var message = new TrackRotMsg();
        message.rotation = transform.localRotation;
        message.release = false;
        _context.SendJson(message);
    }
    // Send tracking message to release object
    private void SendReleaseMessage()
    {
        var message = new TrackRotMsg();
        message.rotation = transform.localRotation;
        message.release = true;
        _context.SendJson(message);
    }

    // Network Ubiq: track rotation
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        // Parse message
        var m = message.FromJson<TrackRotMsg>();

        if (!m.release)
        {
            // Someone grabbed rotatable knob object
            owner = false;
            // Disable Knob component
            if (_knobInteractable.enabled)
            {
                _knobInteractable.enabled = false;
            }
            // Keep tracking rotation
            _knobInteractable.knobObject.transform.localRotation = m.rotation;
        }
        else
        {
            // Release grab
            _knobInteractable.enabled = true;
        }
    }
}