using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class GrateScript : MonoBehaviour
{
    private GameObject _grate;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;
    private void Start()
    {   
        // Get XRGrabInteractable comonentand add a listener 
        _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        _grab.selectEntered.AddListener(XRGrabInteractable_Activated);
    }

    private void OnDestroy()
    {
        _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (_grab != null)
        {
            _grab.selectEntered.RemoveListener(XRGrabInteractable_Activated);
        }
    }

    // Open grate on grab, used for testing the grate opening during development
    private void XRGrabInteractable_Activated(SelectEnterEventArgs eventArgs)
    {
        OpenGrate();
    }

    // Funtion to open the grate
    public void OpenGrate()
    {
        transform.Rotate(90f, 0f, 0f, Space.Self);

        if (_grab != null)
        {
            _grab.enabled = false;
        }
    }

}

