using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class GrateScript : MonoBehaviour
{
    private GameObject grate;
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
        OpenGrate();
    }

    public void OpenGrate()
    {
        transform.Rotate(90f, 0f, 0f, Space.Self);

        if (grab != null)
        {
            grab.enabled = false;
        }
    }

}

