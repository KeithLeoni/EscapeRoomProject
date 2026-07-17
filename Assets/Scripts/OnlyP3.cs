using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Ubiq.Avatars;

public class StrengthOnlyGrabPermission : MonoBehaviour
{
    public ScenePowerManager powerManager;
    public XRGrabInteractable grabInteractable;

    private SizeController _sizeControllerScript;

    private AvatarManager _avatarManager;

    void Start()
    {

        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        _avatarManager = FindFirstObjectByType<AvatarManager>();

    }

    void Update()
    {

        if (grabInteractable == null)
            return;

        Ubiq.Avatars.Avatar[] avatars = _avatarManager.GetComponentsInChildren<Ubiq.Avatars.Avatar>();

        foreach (Ubiq.Avatars.Avatar avatar in avatars)
        {
            if (avatar.GetComponentInChildren<SizeController>() && avatar.IsLocal)
            {

                _sizeControllerScript = avatar.GetComponentInChildren<SizeController>();
                //_sizeControllerScript.ChangeSize(1);
                Debug.Log("Strenght!" + "size level: "+ _sizeControllerScript.sizeLevel);
                if(_sizeControllerScript.sizeLevel == 1) {
                    Debug.Log("can move " + grabInteractable);
                    grabInteractable.enabled = true;
                }
            }
        }
    }
}