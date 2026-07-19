using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ubiq.Messaging;

public class PullLeverScript : MonoBehaviour
{
    private GameObject _doorPivot;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;
    private OpenDoorScript _doorScript;
    // For synch.
    NetworkContext context;

    private void Start()
    {
        // Get the DoorPivot
        _doorPivot = GameObject.Find("DoorPivot");

        // Add listener on the XRGrabInteractable of the lever 
        _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        _grab.selectEntered.AddListener(XRGrabInteractable_Activated);

        // Get door script
        _doorScript = _doorPivot.GetComponent<OpenDoorScript>();

        // Get context
        context = NetworkScene.Register(this);
    }

    private void OnDestroy()
    {
        _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (_grab != null)
        {
            _grab.selectEntered.RemoveListener(XRGrabInteractable_Activated);
        }
    }

    // When the lever is grabbed it calls the function PullLeverDown
    private void XRGrabInteractable_Activated(SelectEnterEventArgs eventArgs)
    {
        PullLeverDown();
    }

    // This function calls the funtion to open the door and pulls the lever down
    private void PullLeverDown()
    {
        // Pull leevr down
        transform.Rotate(160f, 0f, 0f, Space.Self);

        // Disable the grab of the lever (door can be opened once) 
        if (_grab != null)
        {
            _grab.enabled = false;
        }

        if (_doorPivot != null)
        {
            if (_doorScript != null)
            {
                // Call the function in the _doorScript to open the door
                _doorScript.OpenDoor();
            }
        }
        // Synch for other players
        SendTrackLeverMessage();
    }
    
    // Struct to track lever grab
    private struct TrackLever
    {
        public bool grabEnabled;
    }

    // Send message to track lever
    private void SendTrackLeverMessage()
    {
        var message = new TrackLever();
        message.grabEnabled = _grab.enabled;
        context.SendJson(message);
    }

    // Process message to track lever and door
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var m = message.FromJson<TrackLever>();
        _grab.enabled = m.grabEnabled;
        Debug.Log(_doorScript);
        _doorScript.OpenDoor();
    }

}