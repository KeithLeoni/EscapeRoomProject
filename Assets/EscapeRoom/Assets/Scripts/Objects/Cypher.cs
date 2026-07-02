using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Ubiq.Messaging;

public class Cypher : MonoBehaviour
{
    private XRGrabInteractable cypherInteractable;
    private XRKnob knobInteractable;
    // Track network
    [System.NonSerialized]
    public bool owner = false;                      // are you currently grabbing/interacting with the object
    NetworkContext context;

    void Start()
    {
        // Enable Knob interactable only if object is being grabbed
        cypherInteractable = GetComponent<XRGrabInteractable>();
        knobInteractable = gameObject.GetComponentInChildren<XRKnob>();
        cypherInteractable.selectEntered.AddListener(onSelect);
        cypherInteractable.selectExited.AddListener(onDeselect);
        knobInteractable.enabled = false;
        // Network
        context = NetworkScene.Register(this);
        knobInteractable.selectEntered.AddListener(StartTracking);
        knobInteractable.selectExited.AddListener(StopTracking);
    }


    void onSelect(SelectEnterEventArgs args)
    {
        // activate knob
        knobInteractable.enabled = true;
    }

    void onDeselect(SelectExitEventArgs args)
    {
        knobInteractable.enabled = false;
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

    private void StartTracking(SelectEnterEventArgs args) {
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
        context.SendJson(message);
    }
    // Send tracking message to release object
    private void SendReleaseMessage()
    {
        var message = new TrackRotMsg();
        message.rotation = transform.localRotation;
        message.release = true;
        context.SendJson(message);
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
            if (knobInteractable.enabled)
            {
                knobInteractable.enabled = false;
            }

            // Keep tracking rotation
            knobInteractable.knobObject.transform.localRotation = m.rotation;
        }
        else
        {
            // Release grab
            knobInteractable.enabled = true;
        }
    }
}