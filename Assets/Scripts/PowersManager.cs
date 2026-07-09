using Ubiq.Avatars;
using UnityEngine;

public class ScenePowerManager : MonoBehaviour
{
    // Public enum to better define available powers
    public enum Power
    {
        jellyVision, flyingPower, sizeManipulationPower, nothing
    }
    // Keeps track of which power the player has
    // Until we have the power selection feature, this variable can be modified in the Inspector
    private Power _playerPower = Power.nothing;

    // Avatar Power prefabs
    public GameObject flyingAvatarPrefab;
    public GameObject jellyAvatarPrefab;
    public GameObject sizeManipulationAvatarPrefab;
    // Avatar manager
    private AvatarManager _avatarManager;

    void Start()
    {
        // Find avatar manager
        _avatarManager = FindFirstObjectByType<AvatarManager>();
    }

    /// <summary>
    /// Sets player power with the given parameter. This method is necessary for
    /// the power selection feature to work
    /// </summary>
    /// <param name="power"></param>
    public void SetPlayerPower(Power power)
    {
        _playerPower = power;
        // Change avatar prefab
        switch (_playerPower)
        {
            case Power.jellyVision:
                _avatarManager.avatarPrefab = jellyAvatarPrefab;
                break;
            case Power.flyingPower:
                _avatarManager.avatarPrefab = flyingAvatarPrefab;
                break;
            case Power.sizeManipulationPower:
                _avatarManager.avatarPrefab = sizeManipulationAvatarPrefab;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Returns the power the player has currently
    /// </summary>
    public Power GetCurrentPower()
    {
        return _playerPower;
    }
}