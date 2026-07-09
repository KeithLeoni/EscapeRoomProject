using UnityEngine;
using Ubiq.Messaging;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using Ubiq.Avatars;
using Ubiq.Spawning;

/// <summary>
/// Paper Sheet behaviour:
/// - Can interact with highlighter elements
/// - Can be grabbed
/// </summary>
public class PaperSheet : MonoBehaviour, INetworkSpawnable
{
    public NetworkId NetworkId { get; set; }
    private NetworkContext _context;
    // Only who is the owner of the object can pick it up
    private string _owner = "";
    public ScenePowerManager.Power selectedPower { get; private set; } = ScenePowerManager.Power.nothing;
    private AudioSource _audioSource;

    void Start()
    {
        // Object Physics ----------------------------------
        // Set initially to kinematic
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        // When object is first picked up, the user who did it becomes its owner forever
        gameObject.GetComponent<XRGrabInteractable>().selectEntered.AddListener(SetOwner);
        // Modify Grab Behaviour: Once the page is "torn" from the notepad it is a physical object
        gameObject.GetComponent<XRGrabInteractable>().selectExited.AddListener(EnableGravity);
        // Network
        _context = NetworkScene.Register(this);
        _audioSource = GetComponent<AudioSource>();
        // Initially disable interactable for highlighters
        Collider[] colliders = GetComponents<BoxCollider>();
        foreach (var collider in colliders)
        {
            if (collider.isTrigger)
            {
                // Found right collider
                collider.enabled = false;
                break;
            }
        }
    }

    void SetOwner(SelectEnterEventArgs args)
    {
        // Enable collider        
        Collider[] colliders = GetComponents<BoxCollider>();
        foreach (var collider in colliders)
        {
            if (collider.isTrigger)
            {
                // Found right collider
                collider.enabled = true;
                break;
            }
        }

        // Find Local Avatar
        GameObject avatarManager = FindFirstObjectByType<AvatarManager>().gameObject;
        Ubiq.Avatars.Avatar[] avatars = avatarManager.GetComponentsInChildren<Ubiq.Avatars.Avatar>();
        foreach (var avatar in avatars)
        {
            if (avatar.IsLocal)
            {
                _owner = avatar.Peer.uuid;
                break;
            }
        }
        // Play page ripping sound
        _audioSource.Play();
        // Communicate to all remote copies that no other user can interact with this piece of paper
        // TO DO: ADD USER'S NAME ON PAPER SO IT IS LESS CONFUSING
        SendMessage();
        gameObject.GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(SetOwner);
    }
    
    // -----------------------------------------------------------
    
    // Send grab tracking message to release object

    private struct TrackOwnerMsg
    {
        public string owner;
    }
    private void SendMessage()
    {
        var message = new TrackOwnerMsg();
        message.owner = _owner;
        _context.SendJson(message);
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        // Parse message
        var m = message.FromJson<TrackOwnerMsg>();
        // Remove grab element for all other copies
        Collider[] colliders = GetComponents<BoxCollider>();
        foreach (var collider in colliders)
        {
            Destroy(collider);
        }
        // Set owner
        _owner = m.owner;
    }

    void EnableGravity(SelectExitEventArgs args)
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<XRGrabInteractable>().selectExited.RemoveListener(EnableGravity);
    }

    // Define Drawing collider behaviour
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.EndsWith("Highlighter"))
        {
            // Check which highlighter it is
            if (other.gameObject.name.StartsWith("Blue"))
            {
                // Assign right material
                // Register selected power
                selectedPower = ScenePowerManager.Power.flyingPower;
                Debug.Log("Fly Selected");

            } else if (other.gameObject.name.StartsWith("Pink"))
            {
                selectedPower = ScenePowerManager.Power.sizeManipulationPower;
                Debug.Log("Size Selected");
            } else if (other.gameObject.name.StartsWith("Green"))
            {
                selectedPower = ScenePowerManager.Power.jellyVision;
                Debug.Log("Jelly Selected");
            }
        }
    }
}