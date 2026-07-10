using UnityEngine;
using Ubiq.Messaging;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using Ubiq.Avatars;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

/// <summary>
/// </summary>
public class StoryBook : MonoBehaviour, IXRHoverFilter
{
    // Audio Setting
    private AudioSource _audioSource;
    public AudioClip teleportationSoundEffect;
    // Interaction Layer for the socket
    private int _paperLayer;
    // How many players confirmed their choice
    private int _confirmedChoices = 0;
    private ScenePowerManager _powerManager;
    private GameObject _objectToDisable;

    void Start()
    {
        _paperLayer = LayerMask.NameToLayer("Paper");
        GetComponent<XRSocketInteractor>().selectEntered.AddListener(ConfirmChoice);
        _powerManager = FindFirstObjectByType<ScenePowerManager>();
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
        // Delay disabling so that object has time to be snapped into place
        _objectToDisable = paper;
        Invoke(nameof(DisableSnappedPaper), 0.5f );
        // Set local power in power manager
        _powerManager.SetPlayerPower(paper.GetComponent<PaperSheet>().selectedPower);
        // Invoke Teleportation
        if (_confirmedChoices == 3)
        {
            Teleport();
        }
    }

    private void DisableSnappedPaper(GameObject paper)
    {
        _objectToDisable.GetComponent<Rigidbody>().isKinematic = true;
        Destroy(_objectToDisable.GetComponent<XRGrabInteractable>());
        _objectToDisable = null;
    }

    private void Teleport()
    {
        // TODO
        // Insert animation, sound effects ect.
        // Teleport all
    }

    // Before snapping add another validation factor
    public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
    {
        bool isValidSelection = interactable.transform.gameObject.GetComponent<PaperSheet>().validityStatus;
        // Stop snapping
        GetComponent<XRSocketInteractor>().allowSelect = isValidSelection;
        Debug.Log(isValidSelection);

        // TODO: ADD CONFLICT OR INVALID STATUS WARNING (GRAPHICALLY)
        return isValidSelection;
    }

    public bool canProcess => isActiveAndEnabled;

}