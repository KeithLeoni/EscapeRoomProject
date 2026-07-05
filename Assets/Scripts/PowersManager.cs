using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class ScenePowerManager : MonoBehaviour
{
    [Header("Meta Quest Input")]
    [Tooltip("Input button that activates the power")]
    public InputActionProperty buttonAAction;

    [Header("Current Local Player Role/Power")]
    [Range(1, 3)]
    public int playerNumber = 1;

    [Header("Power States (Bools)")]
    public bool jellyVision = false;
    public bool flyingPower = false;
    public bool scalePower = false;

    // Other Movement Providers
    [Space]
    [Header("Fly Power Settings")]
    [Header("Grounded Move Locomotion Provider/s (Object)")]
    [Tooltip("Listed locomotion Provider will be deactivated in fly mode")]
    public GameObject[] groundedMoveProvider;
    public bool landFlying = false;

    private int traceLayerIndex = -1;

    void Start()
    {
        if (buttonAAction.action != null)
        {
            buttonAAction.action.Enable();
        }
        traceLayerIndex = LayerMask.NameToLayer("JellyTraces");
    }

    void Update()
    {
        if (buttonAAction.action != null && (buttonAAction.action.triggered || buttonAAction.action.WasPressedThisFrame()))
        {
            TogglePowerForPlayer();

            // Locate every active avatar instance in the room

            /**JellyEyesScript[] allEyeScripts = Object.FindObjectsByType<JellyEyesScript>(FindObjectsSortMode.None);

            foreach (JellyEyesScript script in allEyeScripts)
            {
                // 1. Update the eyes everywhere so the mirror camera can see them
                script.SetEyesActive(jellyVision);

                // 2. Direct the local trace request. The script's 'isLocalUser' gate ensures it only applies to YOU.
                script.ForceToggleLocalTraces(jellyVision);
            }**/
        }
    }

    public void TogglePowerTest(int newPlayer) {
        // Disable current power if activated
        jellyVision = false;
        flyingPower = false;
        scalePower = false;
        // reinstate move locomotion if flight was activated
        // Enable other locomotion providers
        for (int i = 0; i < groundedMoveProvider.Length; i++)
        {
            groundedMoveProvider[i].SetActive(true);
        }
        // set player number
        playerNumber = newPlayer;
    
    }

    private void TogglePowerForPlayer()
    {
        switch (playerNumber)
        {
            case 1:
                jellyVision = !jellyVision;
                Debug.Log($"Jelly Vision Vision set to: {jellyVision}");
                break;
            case 2:
                flyingPower = !flyingPower;
                if (flyingPower)
                {
                    // Fly mode has been enabled: disable gravity for other locomotion providers
                    for (int i = 0; i < groundedMoveProvider.Length; i++)
                    {
                        groundedMoveProvider[i].SetActive(false);
                    }
                }
                else
                {
                    landFlying = true;
                }
                Debug.Log($"Flying Power set to: {flyingPower}");
                break;
            case 3:
                scalePower = !scalePower;
                Debug.Log($"Scale Power set to: {scalePower}");
                break;
                /*
            case 3:
                superStrength = !superStrength;
                Debug.Log($"Super Strength set to: {superStrength}");
                break;
            case 4:
                shrinkingPower = !shrinkingPower;
                Debug.Log($"Shrinking Power set to: {shrinkingPower}");
                break;*/
        }
    }
}
