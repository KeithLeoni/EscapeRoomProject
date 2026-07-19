using Ubiq.Avatars;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class StrengthOnlyGrabPermission : MonoBehaviour
{
    public ScenePowerManager powerManager;
    public XRGrabInteractable grabInteractable;

    // Is null if local avatar does not have size manipulation powers
    private SizeController _localSizeController = null;

    void Start()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        // FOR TESTING ONLY
        _localSizeController = FindLocalSizeController();
    }

    private SizeController FindLocalSizeController()
    {
        AvatarManager avatarManager = FindFirstObjectByType<AvatarManager>();
        // Find local avatar and see if it has super-strenght
        Ubiq.Avatars.Avatar[] avatars = avatarManager.GetComponentsInChildren<Ubiq.Avatars.Avatar>();

        foreach (Ubiq.Avatars.Avatar avatar in avatars)
        {
            if (avatar.GetComponentInChildren<SizeController>() && avatar.IsLocal)
            {
                return avatar.GetComponentInChildren<SizeController>();
            }
        }
        return null;

    }

    void Update()
    {
        if (grabInteractable == null)
            return;

        if (_localSizeController == null)
            return;

        if (_localSizeController.sizeLevel == 1)
        {
            grabInteractable.enabled = true;
        }
        else
        {
            grabInteractable.enabled = false;            
        }
    }

    /// <summary>
    /// Attempts to find a Size Controller component that belongs to the local avatar
    /// </summary>
    public void UpdateLocalSizeController()
    {
        _localSizeController = FindLocalSizeController();
    }
}
