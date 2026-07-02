using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Scroll : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // As soon as it is grabbed stop animation
        gameObject.GetComponent<XRGrabInteractable>().selectExited.AddListener(onDeselect);
    }

    public void onDeselect(SelectExitEventArgs args)
    {
        // Disable kinematic
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
        // Disable call
        gameObject.GetComponent<XRGrabInteractable>().selectExited.RemoveAllListeners();
    }


}
