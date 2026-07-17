using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using TMPro;
using Ubiq.Messaging;
using Unity.XR.CoreUtils;

/// <summary>
/// </summary>
public class StoryBook : MonoBehaviour
{
    // Audio Setting
    private AudioSource _audioSource;
    public AudioClip teleportationSoundEffect;
    // Interaction Layer for the socket
    private int _paperLayer;
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

    void Start()
    {
        _context = NetworkScene.Register(this);
        _paperLayer = LayerMask.NameToLayer("Paper");
        GetComponent<XRSocketInteractor>().selectEntered.AddListener(ConfirmChoice);
        GetComponent<XRSocketInteractor>().hoverEntered.AddListener(OnHoverEnter);
        GetComponent<XRSocketInteractor>().hoverExited.AddListener(OnHoverExit);
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
    }

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
    }

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
    }

    public void SendUpdateMessage()
    {   // Propagate Update on network
        var message = new ConfirmMsg();
        _context.SendJson(message);
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        // Parse message
        var m = message.FromJson<ConfirmMsg>();
        // A usre confirmed their selection
        _confirmedChoices += 1;
        // Update Text if user has confirmed their choice
        if (_hasConfirmed)
        {
            _instructionsText.text = "Wait for your friends.\n" + _confirmedChoices + "/3";
        }

        // Invoke Teleportation
        if (_confirmedChoices == 2)
        {
            Teleport();
        }
    }

}