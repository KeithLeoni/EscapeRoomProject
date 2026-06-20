using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OpenDoorScript : MonoBehaviour
{
    private GameObject lever;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private void Start()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grab.selectEntered.AddListener(XRGrabInteractable_Activated);
    }

    private void OnDestroy()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(XRGrabInteractable_Activated);
        }
    }

    private void XRGrabInteractable_Activated(SelectEnterEventArgs eventArgs)
    {
        OpenDoor();
    }

    private void OpenDoor()
    {
        transform.Rotate(0f, -90f, 0f, Space.Self);

        if (grab != null)
        {
            grab.enabled = false;
        }
    }

}
