using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Ubiq.Messaging;
using Unity.XR.CoreUtils;
using UnityEngine.InputSystem;

/// <summary>
/// Track inner wheel rotation for network synchronization 
/// </summary>
public class Cypher : MonoBehaviour
{
    // Controller Input 
    [Header("Input Actions for moving the cypher wheel")]
    public InputActionProperty wheelForwardAction;
    public InputActionProperty wheelBackwardsAction;
    private int _currentSector = 0;
    private GameObject _wheelObject;
    private XRGrabInteractable _cypherInteractable;
    private BoxCollider _cypherCollider;
    // Track network
    [System.NonSerialized]
    public bool owner;                      // are you currently grabbing/interacting with the object
    private NetworkContext _context;

    // Script to handle cat dialogue
    private CatSpeech _catSpeechScript;
    private bool _talked = false;

    void Start()
    {
        // Enable Inner grab interactable only if object is being grabbed
        _cypherInteractable = GetComponent<XRGrabInteractable>();
        // Get external collider
        _cypherCollider = gameObject.GetNamedChild("Outer").GetComponent<BoxCollider>();
        _wheelObject = gameObject.GetNamedChild("Inner");

        // Toggle mesh colliders depending on whether the cypher is being grabbed
        _cypherInteractable.selectEntered.AddListener(OnSelect);
        _cypherInteractable.selectExited.AddListener(OnDeselect);
        _cypherInteractable.enabled = false;
        owner = false;

        // Network
        _context = NetworkScene.Register(this);

        // Cat dialogue script
        _catSpeechScript = FindFirstObjectByType<CatSpeech>();
    }

    // Enable Inputs
    void OnEnable()
    {
        wheelForwardAction.action.Enable();
        wheelBackwardsAction.action.Enable();
    }

    void Update()
    {
        int sectorDisplacement = 0;
        // Is chypher grabbed?
        if (owner)
        {
            // Check input
            if (wheelForwardAction.action.WasPressedThisFrame())
            {
                if (!wheelBackwardsAction.action.WasPressedThisFrame())
                {
                    sectorDisplacement += 1;
                }
            }
            else if (wheelBackwardsAction.action.WasPressedThisFrame())
            {
                if (!wheelForwardAction.action.WasPressedThisFrame())
                {
                    sectorDisplacement -= 1;
                }
            }

        }

        if (sectorDisplacement != 0)
        {
            _currentSector += sectorDisplacement;
            // Rotate Cypher 
            _wheelObject.transform.Rotate(0, 0, sectorDisplacement * 36f, Space.Self);
            SendTrackMessage(sectorDisplacement);
        }

    }


    void OnSelect(SelectEnterEventArgs args)
    {
        // Deactivate collider of grabbable object to prevent conflicts
        _cypherCollider.enabled = false;
        if (!_talked)
        {
            _catSpeechScript.Say("I have never seen that, but I think they are instructions to a new device he just bought… he spends too much money on silly stuff if you ask me… One thing I can tell you is that he just changed the vial number 05.", 0);
            _talked = true;
        }
        // For tracking 
        owner = true;
    }

    void OnDeselect(SelectExitEventArgs args)
    {
        // Reactivate collider
        _cypherCollider.enabled = true;
        owner = false;
    }


    // ----------------------------------------------------------------------------------


    // Send rotation tracking message
    private struct TrackRotMsg
    {
        public int offset;
    }

    private void SendTrackMessage(int offset)
    {
        var message = new TrackRotMsg();
        message.offset = offset;
        _context.SendJson(message);
    }

    // Network Ubiq: track rotation
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        // Parse message
        var m = message.FromJson<TrackRotMsg>();
        owner = false;
        // Keep tracking rotation of the hinge
        int offset = m.offset;
        _wheelObject.transform.Rotate(0, 0, offset * 36f, Space.Self);

    }
}