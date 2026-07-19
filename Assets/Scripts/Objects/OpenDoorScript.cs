using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ubiq.Messaging;



public class OpenDoorScript : MonoBehaviour
{
    public AudioSource doorAudioSource;
    
    private GameObject _lever;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;
    private CatSpeech _catSpeechScript;
    

    private void Start()
    {
        doorAudioSource = GetComponent<AudioSource>();
        Debug.Log(doorAudioSource);

        _catSpeechScript = FindFirstObjectByType<CatSpeech>();

    }

    private void XRGrabInteractable_Activated(SelectEnterEventArgs eventArgs)
    {
        OpenDoor();
    }

    public void OpenDoor()
    {
        transform.Rotate(0f, -90f, 0f, Space.Self);
        Debug.Log(transform + "Rotated");

        doorAudioSource.Play();
        Debug.Log("Sound played");

        _catSpeechScript.SayLocal("You did it! You opened the door, I think you’ll find what you need to come back to your world there … But you’ll need to think about how you got here in the first place…", 0);
        Debug.Log("Said");
    }



}
