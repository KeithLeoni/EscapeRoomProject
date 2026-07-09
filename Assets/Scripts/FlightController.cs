using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

/// <summary>
/// Adds the flylocomotion provider to the scene
/// </summary>
public class FlightController : MonoBehaviour
{
    public GameObject flyPrefab;
    private GameObject _flyObject;

    private void Awake()
    {
        // Extrapolate from the scene XROrigin & move providers
        GameObject locomotion = FindFirstObjectByType<LocomotionMediator>().gameObject;
        // Add component
        _flyObject = Instantiate(flyPrefab);
        // Set XROrigin Locomotion as parent
        _flyObject.transform.SetParent(locomotion.transform);
    }

    // Remove Flying Object
    private void OnDestroy()
    {
        Destroy(_flyObject);
    }

}
