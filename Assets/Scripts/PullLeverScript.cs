using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PullLeverScript : MonoBehaviour
{
    
    private Transform t;
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
        PullLeverDown();
    }

    private void PullLeverDown()
    {
        transform.Rotate(160f, 0f, 0f, Space.Self);

        if (grab != null)
        {
            grab.enabled = false;
        }
    }

}
