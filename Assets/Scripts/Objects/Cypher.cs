using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Ubiq.Messaging;
using Unity.XR.CoreUtils;

/// <summary>
/// Track inner wheel rotation for network synchronization 
/// </summary>
public class Cypher : MonoBehaviour
{
    private XRGrabInteractable _cypherInteractable;
    private MeshCollider _wheelCollider;
    private BoxCollider _cypherCollider;
    private XRGrabInteractable _wheelInteractable;
    // Track network
    [System.NonSerialized]
    public bool owner = false;                      // are you currently grabbing/interacting with the object
    private NetworkContext _context;
    // only track state while grabbing
    private int _prevSector = 0;

    // Script to handle cat dialogue
    private CatSpeech _catSpeechScript;
    private bool _talked = false;

    void Start()
    {
        // Enable Inner grab interactable only if object is being grabbed
        _cypherInteractable = GetComponent<XRGrabInteractable>();
        // Get external collider
        _cypherCollider = gameObject.GetNamedChild("Outer").GetComponent<BoxCollider>();
        GameObject hingeJointObj = gameObject.GetNamedChild("HingeJoint");
        _wheelCollider = hingeJointObj.GetComponentInChildren<MeshCollider>();

        // Toggle mesh colliders depending on whether the cypher is being grabbed
        _cypherInteractable.selectEntered.AddListener(OnSelect);
        _cypherInteractable.selectExited.AddListener(OnDeselect);
        // Disable wheel collider
        _wheelCollider.enabled = false;

        // Network
        _context = NetworkScene.Register(this);
        _wheelInteractable = hingeJointObj.GetComponent<XRGrabInteractable>();
        _wheelInteractable.selectEntered.AddListener(StartTracking);
        _wheelInteractable.selectExited.AddListener(StopTracking);

        // Cat dialogue script
        _catSpeechScript = FindFirstObjectByType<CatSpeech>();
    }


    void OnSelect(SelectEnterEventArgs args)
    {
        // Deactivate collider of grabbable object to prevent conflicts
        _cypherCollider.enabled = false;
        // Activate knob interactable
        _wheelCollider.enabled = true;

        if (!_talked)
        {
            _catSpeechScript.Say("I have never seen that, but I think they are instructions to a new device he just bought… he spends too much money on silly stuff if you ask me…", 0);
        }
    }

    void OnDeselect(SelectExitEventArgs args)
    {
        _wheelCollider.enabled = false;
        // Reactivate collider
        _cypherCollider.enabled = true;
    }


    // ----------------------------------------------------------------------------------
    // This tracking only works for the inner wheel, the whole chypher is tracked by the 
    // Grabbable Element component
    private void FixedUpdate()
    {
        if (owner)
        {
            // Optimize network message sending for the inner wheel:
            // send messages only when the wheel is grabbed
            if (_wheelCollider.enabled)
            {
                // User is grabbing inner wheel
                // Send network message to make update copies' rotation
                SendTrackMessage();              
            }
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
            // Disable inner wheel component (not really requires)
            if (_wheelInteractable.enabled)
            {
                _wheelInteractable.enabled = false;
            }
            // Keep tracking rotation of the hinge
            _wheelInteractable.gameObject.transform.localRotation = m.rotation;
        }
        else
        {
            // Release grab
            _wheelInteractable.enabled = true;
        }
    }
}