using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/* Chest Behaviour:
 * - tracks contact with key
 * - plays lid opening animation
 * */
public class Chest : MonoBehaviour
{
    // It is an interactable component, that interacts with key to open
    private GameObject lockObject;
    private GameObject chestLidObject;
    // Chest opening variables
    private Vector3 rotationPoint;
    private Quaternion finalRotationTarget;
    private float angularSpeed = -100f;
    // Used as fail safe in case of lag
    private float prevDiff = 240f;
    private bool isOpening;
    [Header("Key object the lock interacts with")]
    [Space]
    public GameObject keyObject;

    void Start()
    {
        // Get Lock
        lockObject = gameObject.GetNamedChild("Lock");
        chestLidObject = gameObject.GetNamedChild("Chest_Lid");
        rotationPoint = gameObject.transform.position;
        rotationPoint.y += 0.421f;
        rotationPoint.z -= 0.263f;
        finalRotationTarget = Quaternion.Euler(240f, 0, 0);
        isOpening = false;
    }

    void Update()
    {
        if (isOpening)
        {
            // Animate chest opening
            float angleDiff = Quaternion.Angle(chestLidObject.transform.rotation, finalRotationTarget);
            // Snap into position
            // WARNING: lagging can cause jumps in rotation & stop point can be missed, therefore it needs a failsafe
            if (angleDiff < 5f || prevDiff < angleDiff)
            {
                chestLidObject.transform.rotation = finalRotationTarget;
                isOpening = false;
                return;
            }
            chestLidObject.transform.RotateAround(rotationPoint, Vector3.right, angularSpeed * Time.deltaTime);
            prevDiff = angleDiff;

        }
    }

    private void keyFall()
    {
        keyObject.GetComponent<XRGrabInteractable>().enabled = false;
        Debug.Log("DISABLE");
    }

    private void lockFall()
    {
        lockObject.GetComponent<SphereCollider>().enabled = false;
        lockObject.GetComponent<MeshCollider>().enabled = true;
        lockObject.GetComponent<Rigidbody>().isKinematic = false;
        // Play animation of chest opening
        isOpening = true;
    }

    public void onSnap()
    {
        // On snap the lock falls to the ground
        // Key has to be kinematic and not selectable
        keyObject.GetComponent<XRGrabInteractable>().hoverEntered.RemoveAllListeners();
        keyObject.GetComponent<XRGrabInteractable>().hoverExited.RemoveAllListeners();
        keyObject.GetComponent<Outline>().enabled = false;

        // For networking sync, we have to wait until grabbed key is released, before disabling grab component
        GrabbableElement keyGrabTracker = keyObject.GetComponent<GrabbableElement>();
        while (!keyGrabTracker.owner && !keyGrabTracker.grabInteractable.enabled) { 
        }

        Invoke(nameof(keyFall), 0.5f);
        // Activate mesh collider and deactivate sphere collider
        Invoke(nameof(lockFall), 1.0f);
    }


}