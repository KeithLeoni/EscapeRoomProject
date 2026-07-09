using System;
using System.Drawing;
using Ubiq.Avatars;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

public class SizeController : MonoBehaviour
{
    public GameObject sizeManipulationPrefab;
    private GameObject _sizeObject;
    private AvatarManager _avatarManager;

    private void OnEnable()
    {
        // Avatar Manager: wait for prefab change
        _avatarManager = FindFirstObjectByType<AvatarManager>();
        _avatarManager.OnAvatarCreated.AddListener(CreateProvider);
    }

    /// <summary>
    /// Spawn relative size manipulator provider under the XROrigin hierarchy
    /// </summary>
    private void CreateProvider(Ubiq.Avatars.Avatar arg0)
    {
        if (arg0.gameObject.GetComponent<SizeController>() != null && arg0.IsLocal)
        {
            // Extrapolate from the scene XROrigin & move providers
            GameObject locomotion = FindFirstObjectByType<LocomotionMediator>().gameObject;
            // Add component
            _sizeObject = Instantiate(sizeManipulationPrefab);
            // Set XROrigin Locomotion as parent
            _sizeObject.transform.SetParent(locomotion.transform);
            _avatarManager.OnAvatarCreated.RemoveListener(CreateProvider);
            _sizeObject.GetComponent<SizeManipulationProvider>().Initialize(arg0.gameObject.transform);
        }
    }

    // Remove Flying Object
    private void OnDestroy()
    {
        Destroy(_sizeObject);
    }

}