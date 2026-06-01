using UnityEngine;
using UnityEngine.InputSystem;

public class ScenePowerManager : MonoBehaviour
{
    [Header("Meta Quest Input")]
    [Tooltip("Map this to XRI RightHand/PrimaryButton (Button A)")]
    public InputActionProperty buttonAAction;

    [Header("Current Local Player Role/Power")]
    [Tooltip("Change this (1-4) in the scene to test different player powers")]
    [Range(1, 4)]
    public int playerNumber = 1;

    [Header("Power States (Bools)")]
    public bool jellyVision = false;
    public bool flyingPower = false;
    public bool superStrength = false;
    public bool shrinkingPower = false;

    // This variable will hold the address of your object on the network, and allow you to send messages
    //private NetworkContext context;

    // public NetworkId Id { get; set; } = new NetworkId(2026);


    void Start()
    {
        // This registers the Component with Ubiq and gets it an address on the network (return value is a NetworkContext)
        //context = NetworkScene.Register(this);

        if (buttonAAction.action != null)
        {
            buttonAAction.action.Enable();
        }
    }

    void Update()
    {
        // When Button A is pressed, toggle the correct bool based on player role
        if (buttonAAction.action != null && buttonAAction.action.WasPressedThisFrame())
        {
            TogglePowerForPlayer();

            PowerSyncMessage message = new PowerSyncMessage();
            message.changedPlayerNumber = playerNumber;

            //context.SendJson(message);
        }
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
                Debug.Log($"Flying Power set to: {flyingPower}");
                break;
            case 3:
                superStrength = !superStrength;
                Debug.Log($"Super Strength set to: {superStrength}");
                break;
            case 4:
                shrinkingPower = !shrinkingPower;
                Debug.Log($"Shrinking Power set to: {shrinkingPower}");
                break;
        }
    }

    // public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    // {
    //     PowerSyncMessage incomingData = message.FromJson<PowerSyncMessage>();
    //     TogglePowerForPlayer(incomingData.changedPlayerNumber);
    // }

    private void OnDestroy()
    {
        if (buttonAAction.action != null)
        {
            buttonAAction.action.Disable();
        }
    }
}

public struct PowerSyncMessage
{
    public int changedPlayerNumber;
}

// using UnityEngine;
// using UnityEngine.InputSystem;
// using Ubiq.Messaging;
// using Ubiq.Rooms; // 1. Added namespace to listen for room/player events

// public class ScenePowerManager : MonoBehaviour
// {
//     [Header("Meta Quest Input")]
//     [Tooltip("Map this to XRI RightHand/PrimaryButton (Button A)")]
//     public InputActionProperty buttonAAction;

//     [Header("Current Local Player Role/Power")]
//     [Tooltip("Change this (1-4) in the scene to test different player powers")]
//     [Range(1, 4)]
//     public int playerNumber = 1;

//     [Header("Power States (Bools)")]
//     public bool jellyVision = false;
//     public bool flyingPower = false;
//     public bool superStrength = false;
//     public bool shrinkingPower = false;

//     private NetworkContext context;
//     private RoomClient roomClient; // 2. Reference to track room status

//     public NetworkId Id { get; set; } = new NetworkId(2026);


//     void Start()
//     {
//         context = NetworkScene.Register(this);

//         // Find Ubiq's RoomClient component in the scene and listen for new players
//         roomClient = Object.FindFirstObjectByType<RoomClient>();
//         if (roomClient != null)
//         {
//             // CHANGED: OnPeerJoined -> OnPeerAdded
//             roomClient.OnPeerAdded.AddListener(OnPeerAdded); 
//         }

//         if (buttonAAction.action != null)
//         {
//             buttonAAction.action.Enable();
//         }
//     }

//     // 4. This triggers on YOUR machine automatically when a new player connects
//     private void OnPeerJoined(IPeer peer)
//     {
//         // If we have our power active when they arrive, broadcast the current true state immediately!
//         BroadcastCurrentState();
//     }

//      // Sends the absolute current state of our power across the network
//     private void BroadcastCurrentState()
//     {
//         PowerSyncMessage message = new PowerSyncMessage
//         {
//             changedPlayerNumber = playerNumber,
//             isCurrentlyActive = GetCurrentPowerState(playerNumber) // Send actual true/false state
//         };

//         context.SendJson(message);
//     }

//     void Update()
//     {
//         if (buttonAAction.action != null && buttonAAction.action.WasPressedThisFrame())
//         {
//             TogglePowerForPlayer(playerNumber);
//             BroadcastCurrentState(); // 5. Extracted the network message to its own function
//         }
//     }

//     // Helper function to quickly grab our specific local power state
//     private bool GetCurrentPowerState(int player)
//     {
//         switch (player)
//         {
//             case 1: return jellyVision;
//             case 2: return flyingPower;
//             case 3: return superStrength;
//             case 4: return shrinkingPower;
//             default: return false;
//         }
//     }

//     // Keep this original toggle method for button presses
//     private void TogglePowerForPlayer(int targetPlayer)
//     {
//         switch (targetPlayer)
//         {
//             case 1:
//                 jellyVision = !jellyVision;
//                 Debug.Log($"Jelly Vision set to: {jellyVision}");
//                 break;
//             case 2:
//                 flyingPower = !flyingPower;
//                 Debug.Log($"Flying Power set to: {flyingPower}");
//                 break;
//             case 3:
//                 superStrength = !superStrength;
//                 Debug.Log($"Super Strength set to: {superStrength}");
//                 break;
//             case 4:
//                 shrinkingPower = !shrinkingPower;
//                 Debug.Log($"Shrinking Power set to: {shrinkingPower}");
//                 break;
//         }
//     }

//     // 6. Overloaded method to force an exact state match when messages arrive from the network
//     private void SetPowerState(int targetPlayer, bool state)
//     {
//         switch (targetPlayer)
//         {
//             case 1: jellyVision = state; break;
//             case 2: flyingPower = state; break;
//             case 3: superStrength = state; break;
//             case 4: shrinkingPower = state; break;
//         }
//         Debug.Log($"Network Synced: Player {targetPlayer} power state forced to {state}");
//     }

//     public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
//     {
//         PowerSyncMessage incomingData = message.FromJson<PowerSyncMessage>();

//         // 7. Changed from blind toggle to enforcing the specific true/false state sent over the wire
//         SetPowerState(incomingData.changedPlayerNumber, incomingData.isCurrentlyActive);
//     }

//     private void OnDestroy()
//     {
//         if (buttonAAction.action != null)
//         {
//             buttonAAction.action.Disable();
//         }

//         // Clean up the listener when the object is destroyed to prevent memory leaks
//         if (roomClient != null)
//         {
//             roomClient.OnPeerJoined.RemoveListener(OnPeerJoined);
//         }
//     }
// }

// // 8. Updated struct to pass the explicit true/false state
// public struct PowerSyncMessage
// {
//     public int changedPlayerNumber;
//     public bool isCurrentlyActive; 
// }