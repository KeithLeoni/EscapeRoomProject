using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ubiq.Messaging;

public class PullLeverScript : MonoBehaviour
{
    
    private Transform t;
    private GameObject lever;
    private GameObject doorPivot;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;
    private OpenDoorScript _doorScript;
    // For synch.
    NetworkContext context;

    private void Start()
    {
        doorPivot = GameObject.Find("DoorPivot");
        _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        _grab.selectEntered.AddListener(XRGrabInteractable_Activated);

        _doorScript = doorPivot.GetComponent<OpenDoorScript>();

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

    private void XRGrabInteractable_Activated(SelectEnterEventArgs eventArgs)
    {
        PullLeverDown();
    }

    private void PullLeverDown()
    {
        transform.Rotate(160f, 0f, 0f, Space.Self);

        if (_grab != null)
        {
            _grab.enabled = false;
        }

        if (doorPivot != null)
        {

            if (_doorScript != null)
            {
                _doorScript.OpenDoor();
            }
        }
    }
    
     private struct TrackLever
    {
        public bool grabEnabled;
    }

    // Send message to track lever
    private void SendTrackTrackLeverMessage()
    {
        var message = new TrackLever();
        message.grabEnabled = _grab.enabled;
        context.SendJson(message);
    }

    // Process message to track lever
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var m = message.FromJson<TrackLever>();
        _grab.enabled = m.grabEnabled;
        _doorScript.doorAudioSource.Play();
    }

}
