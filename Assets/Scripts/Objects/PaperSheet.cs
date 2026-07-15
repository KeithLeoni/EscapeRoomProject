using UnityEngine;
using Ubiq.Messaging;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using Ubiq.Avatars;
using Ubiq.Spawning;
using TMPro;
using Unity.XR.CoreUtils;

/// <summary>
/// Paper Sheet behaviour:
/// - Can interact with highlighter elements
/// - Can be grabbed
/// </summary>
public class PaperSheet : MonoBehaviour, INetworkSpawnable
{
    public NetworkId NetworkId { get; set; }
    // Only who is the owner of the object can pick it up
    public string owner = "";
    public ScenePowerManager.Power selectedPower = ScenePowerManager.Power.nothing;
    // Is current selection Valid?
    public bool validityStatus = false;
    public bool isLocal { get; private set; } = false;
    public Spawner notepad;

    // Audio Setting
    private AudioSource _audioSource;
    public AudioClip rippingSoundEffect;
    public AudioClip writingSoundEffect1;
    public AudioClip writingSoundEffect2;
    public AudioClip writingSoundEffect3;

    // Marker drawing settings
    private bool _hasEntered = false;

    void Start()
    {
        // Object Physics ----------------------------------
        // Set initially to kinematic
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        // When object is first picked up, the user who did it becomes its owner forever
        gameObject.GetComponent<XRGrabInteractable>().selectEntered.AddListener(SetOwner);
        // Modify Grab Behaviour: Once the page is "torn" from the notepad it is a physical object
        gameObject.GetComponent<XRGrabInteractable>().selectExited.AddListener(EnableGravity);
        // Initially disable drawing interaction
        ToggleDrawingInteraction(false);
        _audioSource = GetComponent<AudioSource>();

        // Network ------------------------------------------
        if (notepad == null)
        {
            notepad = FindAnyObjectByType<Spawner>();
        }
    }

    /// <summary>
    /// Enabl/Disable interactable components for drawing
    /// </summary>
    /// <param name="activationStatus"></param>
    private void ToggleDrawingInteraction(bool activationStatus)
    {
        // Find collider        
        Collider[] colliders = GetComponents<BoxCollider>();
        foreach (var collider in colliders)
        {
            if (collider.isTrigger)
            {
                // Found right collider
                collider.enabled = activationStatus;
                break;
            }
        }

    }

    private void SetOwner(SelectEnterEventArgs args)
    {
        // Enable drawing interaction
        ToggleDrawingInteraction(true);

        // Find Local Avatar
        GameObject avatarManager = FindFirstObjectByType<AvatarManager>().gameObject;
        Ubiq.Avatars.Avatar[] avatars = avatarManager.GetComponentsInChildren<Ubiq.Avatars.Avatar>();
        // Save avatar's name
        string name = "";
        foreach (var avatar in avatars)
        {
            if (avatar.IsLocal)
            {
                owner = avatar.Peer.uuid;
                name = avatarManager.GetComponent<AvatarManager>().RoomClient.Me["ubiq.displayname"];
                break;
            }
        }

        // Play page ripping sound
        _audioSource.PlayOneShot(rippingSoundEffect);
        isLocal = true;
        // Spawn next object
        notepad.SpawnObject();

        // Set Text
        GameObject textObject = gameObject.GetNamedChild("Text");
        TextMeshPro text = textObject.GetComponent<TextMeshPro>();
        Debug.Log(text);
        text.text = name + "'s Hero";
        text.enabled = true;

        gameObject.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(SetOwner);
    }

    // -----------------------------------------------------------

    void EnableGravity(SelectExitEventArgs args)
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<XRGrabInteractable>().selectExited.RemoveListener(EnableGravity);
    }

    // Define Drawing collider behaviour
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.EndsWith("Pen") && !_hasEntered)
        {
            // Check that marker is being grabbed
            XRGrabInteractable interactable = other.gameObject.GetComponent<XRGrabInteractable>();
            if (interactable == null || !interactable.isSelected)
            {
                // Marker is not being grabbed
                return;
            }
            _hasEntered = true;
            // Check which highlighter it is
            if (other.gameObject.name.StartsWith("Blue"))
            {
                // Play sound effect
                _audioSource.PlayOneShot(writingSoundEffect1);
                // Assign right material
                // Register selected power
                selectedPower = ScenePowerManager.Power.flyingPower;
                Debug.Log("Fly Selected");

            }
            else if (other.gameObject.name.StartsWith("Pink"))
            {
                // Play sound effect
                _audioSource.PlayOneShot(writingSoundEffect2);
                selectedPower = ScenePowerManager.Power.sizeManipulationPower;
                Debug.Log("Size Selected");
            }
            else if (other.gameObject.name.StartsWith("Green"))
            {
                // Play sound effect
                _audioSource.PlayOneShot(writingSoundEffect3);
                selectedPower = ScenePowerManager.Power.jellyVision;
                Debug.Log("Jelly Selected");
            }
        }
        // Communicate with notepad to coordinate remote copies
        notepad.SendUpdateMessage();
    }

    void OnTriggerExit(Collider other)
    {
        // Reset variable
        _hasEntered = false;
    }
}