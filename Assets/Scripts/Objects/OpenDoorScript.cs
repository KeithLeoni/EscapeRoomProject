using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OpenDoorScript : MonoBehaviour
{
    private GameObject _lever;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;
    private AudioSource _doorAudioSource;
    private CatSpeech _catSpeechScript;

    private void Start()
    {
        _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        _grab.selectEntered.AddListener(XRGrabInteractable_Activated);
        _doorAudioSource = GetComponent<AudioSource>();

        _catSpeechScript = FindFirstObjectByType<CatSpeech>();
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
        OpenDoor();
    }

    public void OpenDoor()
    {
        transform.Rotate(0f, -90f, 0f, Space.Self);

        _doorAudioSource.Play();


        if (_grab != null)
        {
            _grab.enabled = false;
        }

        _catSpeechScript.Say("You did it! You opened the door, I think you’ll find what you need to come back to your world there … But you’ll need to think about how you got here in the first place…", 0);
    }

}
