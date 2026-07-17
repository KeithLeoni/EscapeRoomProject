using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using TMPro;
using Ubiq.Messaging;
using Unity.XR.CoreUtils;
using System.Collections.Generic;
using Ubiq.Avatars;
using System.Linq;

/// <summary>
/// </summary>
public class StoryBook : MonoBehaviour
{
    // Audio Setting
    private AudioSource _audioSource;
    public AudioClip teleportationSoundEffect;
    // How many players confirmed their choice
    private int _confirmedChoices = 0;
    // If local user has confirmed their choice
    private bool _hasConfirmed = false;
    private ScenePowerManager _powerManager;
    private ScenePowerManager.Power _finalChoice;
    private GameObject _objectToDisable;
    private GameObject _rejectionSymbol;
    private GameObject _acceptionSymbol;
    // Text colors
    private TextMeshPro _instructionsText;
    private NetworkContext _context;
    private CharacterController _characterController;
    public Transform dest;
    // Test variables
    private bool _isChooser = true;
    private List<string> setOrderAvatars = new List<string>();
    private List<ScenePowerManager.Power> setOrderPowers = new List<ScenePowerManager.Power>();
    private AvatarManager _avatarManager;

    void Start()
    {
        _context = NetworkScene.Register(this);
        //GetComponent<XRSocketInteractor>().selectEntered.AddListener(ConfirmChoice);
        //GetComponent<XRSocketInteractor>().hoverEntered.AddListener(OnHoverEnter);
        //GetComponent<XRSocketInteractor>().hoverExited.AddListener(OnHoverExit);
        _powerManager = FindFirstObjectByType<ScenePowerManager>();
        Canvas[] canvasElements = gameObject.GetComponentsInChildren<Canvas>(true);
        foreach (var item in canvasElements)
        {
            if (item.gameObject.name == "AcceptSymbol")
            {
                _acceptionSymbol = item.gameObject;
            }
            else
            {
                _rejectionSymbol = item.gameObject;
            }
        }
        // Find instructions
        _instructionsText = gameObject.GetComponentInChildren<TextMeshPro>(true);

        _avatarManager = FindFirstObjectByType<AvatarManager>();
    }

    // For player testing
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<XROrigin>() != null)
        {
            // Teleport + random power assignment
            if (_hasConfirmed)
            {
                return;
            }
            _hasConfirmed = true;
            _confirmedChoices += 1;
            Debug.Log("ENTERED" + _confirmedChoices);
            if (_isChooser)
            {
                // This user will decide the random oder
                Ubiq.Avatars.Avatar localAvatar = null;
                Ubiq.Avatars.Avatar[] avatar = _avatarManager.GetComponentsInChildren<Ubiq.Avatars.Avatar>();
                foreach (var item in avatar)
                {
                    setOrderAvatars.Add(item.Peer.uuid);
                    if (item.IsLocal)
                    {
                        localAvatar = item;
                    }
                }
                //List<ScenePowerManager.Power> powersAvailable = new List<ScenePowerManager.Power>();
                setOrderPowers.Add(ScenePowerManager.Power.jellyVision);
                setOrderPowers.Add(ScenePowerManager.Power.flyingPower);
                setOrderPowers.Add(ScenePowerManager.Power.sizeManipulationPower);

                // Assign your own power
                _finalChoice = FindYourPower(localAvatar.Peer.uuid, setOrderPowers, setOrderAvatars);
                Debug.Log(_finalChoice);
                SendUpdateMessage(true, setOrderAvatars, setOrderPowers);
            }
            else
            {
                SendUpdateMessage(false, setOrderAvatars, setOrderPowers);                
            }

            if (_confirmedChoices == 3)
            {
                Teleport();
            }
        }
    }

    private ScenePowerManager.Power FindYourPower(string yourUUID, List<ScenePowerManager.Power> powers, List<string> avatars)
    {
        // Find uuid avatar position in list
        int index = -1;
        for (int i = 0; i < avatars.Count; i++)
        {
            if (avatars[i] == yourUUID)
            {
                index = i;
                break;
            }
        }
        return powers[index];

    }
    /*
        private void OnHoverEnter(HoverEnterEventArgs arg0)
        {
            bool isValidSelection = arg0.interactableObject.transform.gameObject.GetComponent<PaperSheet>().validityStatus;
            // Stop snapping
            GetComponent<XRSocketInteractor>().allowSelect = isValidSelection;
            if (!isValidSelection)
            {
                ToggleWarning(true);
            }
            else
            {
                _acceptionSymbol.SetActive(true);
            }
        }

        private void OnHoverExit(HoverExitEventArgs arg0)
        {
            // Reset
            ToggleWarning(false);
            _acceptionSymbol.SetActive(false);
        }

        // When you snap the piece of paper: make piece of paper non-grabbable & kinematic
        // Also make piece of paper non-interactable for the socket
        public void ConfirmChoice(SelectEnterEventArgs args)
        {
            // Make paper un-grabbabble
            GameObject paper = args.interactableObject.transform.gameObject;
            if (!paper.GetComponent<PaperSheet>().validityStatus)
            {
                return;
            }
            // Update choice confirmation
            _confirmedChoices += 1;
            _hasConfirmed = true;
            // Sync this variable
            SendUpdateMessage();

            // Update text
            _instructionsText.text = "Wait for your friends.\n" + _confirmedChoices + "/3";
            _instructionsText.color = new Color(20, 172, 60);
            _acceptionSymbol.SetActive(false);

            // Delay disabling so that object has time to be snapped into place
            _objectToDisable = paper;
            Invoke(nameof(DisableSnappedPaper), 0.5f);
            _finalChoice = paper.GetComponent<PaperSheet>().selectedPower;
            // Invoke Teleportation
            if (_confirmedChoices == 2)
            {
                Teleport();
            }
        }

        private void DisableSnappedPaper()
        {
            _objectToDisable.GetComponent<Rigidbody>().isKinematic = true;
            _objectToDisable.GetComponent<XRGrabInteractable>().enabled = false;
        }*/

    private void Teleport()
    {
        // TODO
        // Set local power in power manager
        _powerManager.SetPlayerPower(_finalChoice);
        XROrigin xrOrigin = GameObject.FindFirstObjectByType<XROrigin>();
        _characterController = xrOrigin.GetComponent<CharacterController>();
        if (_characterController != null) _characterController.enabled = false;

        xrOrigin.transform.position = dest.position;
        xrOrigin.transform.rotation = dest.rotation;

        if (_characterController != null) _characterController.enabled = true;
    }

    private void ToggleWarning(bool visibility)
    {
        _rejectionSymbol.SetActive(visibility);
        // Control Text
        if (!visibility)
        {
            _instructionsText.color = new Color(255, 0, 225);
            // Reset text
            _instructionsText.text = "When you are ready, place your drawing here!";
        }
        else
        {
            _instructionsText.color = new Color(162, 0, 0);
            _instructionsText.text = "You have to pick something and you cannot pick the same powers as your friends :<";
        }

    }

    /// <summary>
    /// Reset all variables to pre-selection status
    /// </summary>
    public void ResetStatus()
    {
        _hasConfirmed = false;
        _confirmedChoices = 0;
    }

    // ----------------------------------------
    // Network control

    // Message to synchronize who confirmed their selection
    private struct ConfirmMsg
    {
        public bool isRandomChoice;
        public List<string> setOrderAvatars;
        public List<ScenePowerManager.Power> setOrderPowers;
    }

    public void SendUpdateMessage(bool isRandomChoice,
        List<string> setOrderAvatars,
        List<ScenePowerManager.Power> setOrderPowers)
    {   // Propagate Update on network
        var message = new ConfirmMsg();
        message.isRandomChoice = isRandomChoice;
        message.setOrderAvatars = setOrderAvatars;
        message.setOrderPowers = setOrderPowers;
        _context.SendJson(message);
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        // Parse message
        var m = message.FromJson<ConfirmMsg>();
        // A user confirmed their selection
        _confirmedChoices += 1;
        Debug.Log(_confirmedChoices);
        if (m.isRandomChoice)
        {
            // Set power according to chooser
            // Find your avatar UUID
            Ubiq.Avatars.Avatar localAvatar = null;
            Ubiq.Avatars.Avatar[] avatar = _avatarManager.GetComponentsInChildren<Ubiq.Avatars.Avatar>();
            foreach (var item in avatar)
            {
                setOrderAvatars.Add(item.Peer.uuid);
                if (item.IsLocal)
                {
                    localAvatar = item;
                }
            }
            _finalChoice = FindYourPower(localAvatar.Peer.uuid, m.setOrderPowers, m.setOrderAvatars);
            _isChooser = false;
        }

        /*
        // Update Text if user has confirmed their choice
        if (_hasConfirmed)
        {
            _instructionsText.text = "Wait for your friends.\n" + _confirmedChoices + "/3";
        }*/

        // Invoke Teleportation
        if (_confirmedChoices == 3)
        {
            Teleport();
        }
    }

}