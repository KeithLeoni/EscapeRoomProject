using UnityEngine;
using UnityEngine.InputSystem;

public class SizeManipulationProvider : MonoBehaviour
{
    private Transform xrOrigin;
    private SizeController _controller;

    void Start()
    {
        // Need to find xr origin like this for smooth transition
        xrOrigin = transform;
    }

    void OnEnable()
    {
        _controller = FindFirstObjectByType<SizeController>();        
    }

    void FixedUpdate()
    {
        if (xrOrigin == null || !_controller.isLocal)
        {
            return;
        }

        // Only change target when previous one has been reached
        if (_controller.HasReachedGoal())
        {
            return;
        }

        xrOrigin.localScale = _controller.GetAvatarScale();

    }
}