using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Chest behaviour that plays opening lid animation when key is put in lock   
/// </summary>
public class Chest : MonoBehaviour
{
    // It is an interactable component, that interacts with key to open
    private GameObject _lockObject;
    private GameObject _chestLidObject;
    // Chest opening variables
    private Vector3 _rotationPoint;
    private Quaternion _finalRotationTarget;
    private float _angularSpeed = -100f;
    // Used as fail safe in case of lag
    private float _prevDiff = 240f;
    private bool _isOpening;
    [Header("Key object the lock interacts with")]
    [Space]
    public GameObject keyObject;

    void Start()
    {
        // Get Lock
        _lockObject = gameObject.GetNamedChild("Lock");
        _chestLidObject = gameObject.GetNamedChild("Chest_Lid");
        _rotationPoint = gameObject.GetNamedChild("PivotPoint").transform.position;
        _finalRotationTarget = Quaternion.Euler(240f, 0, 0);
        _isOpening = false;
    }

    void Update()
    {
        if (_isOpening)
        {
            // Animate chest opening
            float angleDiff = Quaternion.Angle(_chestLidObject.transform.rotation, _finalRotationTarget);
            // Snap into position
            // WARNING: lagging can cause jumps in rotation & stop point can be missed, therefore it needs a failsafe
            if (angleDiff < 5f || _prevDiff < angleDiff)
            {
                _chestLidObject.transform.rotation = _finalRotationTarget;
                _isOpening = false;
                return;
            }
            _chestLidObject.transform.RotateAround(_rotationPoint, Vector3.right, _angularSpeed * Time.deltaTime);
            _prevDiff = angleDiff;

        }
    }

    /// <summary>
    /// Make key <see langword="object"/> ungrabbable 
    /// </summary>
    private void KeyFall()
    {
        keyObject.GetComponent<XRGrabInteractable>().enabled = false;
    }

    /// <summary>
    /// Disable lock, make it fall and play chest opening animation
    /// </summary>
    private void LockFall()
    {
        _lockObject.GetComponent<SphereCollider>().enabled = false;
        _lockObject.GetComponent<MeshCollider>().enabled = true;
        _lockObject.GetComponent<Rigidbody>().isKinematic = false;
        // Play animation of chest opening
        _isOpening = true;
    }

    /// <summary>
    /// When key snaps to lock, start lock & key falling  
    /// </summary>
    public void OnSnap()
    {
        // On snap the lock falls to the ground
        // Key has to be kinematic and not selectable
        keyObject.GetComponent<XRGrabInteractable>().hoverEntered.RemoveAllListeners();
        keyObject.GetComponent<XRGrabInteractable>().hoverExited.RemoveAllListeners();
        keyObject.GetComponent<Outline>().enabled = false;

        // For networking sync, we have to wait until grabbed key is released, before disabling grab component
        GrabbableElement keyGrabTracker = keyObject.GetComponent<GrabbableElement>();
        // Fix grab/snap bug for network synchronization: delay key disabling for the copies
        while (!keyGrabTracker.owner && !keyGrabTracker.grabInteractable.enabled)
        {
        }

        Invoke(nameof(KeyFall), 0.5f);
        // Activate mesh collider and deactivate sphere collider
        Invoke(nameof(LockFall), 1.0f);
    }
}