using UnityEngine;
using Ubiq.Messaging;
using Unity.XR.CoreUtils;
using System.Collections.Generic;
using Ubiq.Avatars;
using Ubiq.Samples;

/// <summary>
/// </summary>
public class StoryBook : MonoBehaviour
{
    // Audio Setting
    private AudioSource _audioSource;

    // Start experience confirmation variables
    public AudioClip teleportationSoundEffect;
    // How many players confirmed their choice
    private int _confirmedChoices = 0;
    // If local user has confirmed their choice
    private bool _hasConfirmed = false;
    private ScenePowerManager _powerManager;
    private PowerSelection _powerSelection;
    private ScenePowerManager.Power _finalChoice;
    private bool _isChooser = true;
    private List<string> _setOrderAvatars = new List<string>();
    private List<ScenePowerManager.Power> _setOrderPowers = new List<ScenePowerManager.Power>();
    private AvatarManager _avatarManager;

    // Teleport
    private NetworkContext _context;
    private CharacterController _characterController;
    public Transform dest;

    // Activation of furniture for super strengh
    private List<StrengthOnlyGrabPermission> _movableFurniture = new List<StrengthOnlyGrabPermission>();

    void Start()
    {
        _context = NetworkScene.Register(this);
        _powerManager = FindFirstObjectByType<ScenePowerManager>();
        _avatarManager = FindFirstObjectByType<AvatarManager>();
        _powerSelection = FindFirstObjectByType<PowerSelection>();

        // Find all movable furniture
        _movableFurniture.AddRange(FindObjectsByType<StrengthOnlyGrabPermission>(FindObjectsSortMode.None));
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
            if (_isChooser)
            {
                // This user will decide the random oder
                Ubiq.Avatars.Avatar localAvatar = null;
                Ubiq.Avatars.Avatar[] avatar = _avatarManager.GetComponentsInChildren<Ubiq.Avatars.Avatar>();
                foreach (var item in avatar)
                {
                    _setOrderAvatars.Add(item.Peer.uuid);
                    if (item.IsLocal)
                    {
                        localAvatar = item;
                    }
                }
                //List<ScenePowerManager.Power> powersAvailable = new List<ScenePowerManager.Power>();
                _setOrderPowers.Add(ScenePowerManager.Power.jellyVision);
                _setOrderPowers.Add(ScenePowerManager.Power.flyingPower);
                _setOrderPowers.Add(ScenePowerManager.Power.sizeManipulationPower);

                // Assign your own power
                _finalChoice = FindYourPower(localAvatar.Peer.uuid, _setOrderPowers, _setOrderAvatars);
                SendUpdateMessage(true, _setOrderAvatars, _setOrderPowers);
            }
            else
            {
                SendUpdateMessage(false, _setOrderAvatars, _setOrderPowers);
            }

            if (_confirmedChoices == _powerSelection.maxPlayers)
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

        // Change movable furniture status if needed
        foreach (var item in _movableFurniture)
        {
            item.UpdateLocalSizeController();
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
        if (m.isRandomChoice)
        {
            // Set power according to chooser
            // Find your avatar UUID
            Ubiq.Avatars.Avatar localAvatar = null;
            Ubiq.Avatars.Avatar[] avatar = _avatarManager.GetComponentsInChildren<Ubiq.Avatars.Avatar>();
            foreach (var item in avatar)
            {
                _setOrderAvatars.Add(item.Peer.uuid);
                if (item.IsLocal)
                {
                    localAvatar = item;
                }
            }
            _finalChoice = FindYourPower(localAvatar.Peer.uuid, m.setOrderPowers, m.setOrderAvatars);
            _isChooser = false;
        }

        // Invoke Teleportation
        if (_confirmedChoices == _powerSelection.maxPlayers)
        {
            Teleport();
        }
    }

}