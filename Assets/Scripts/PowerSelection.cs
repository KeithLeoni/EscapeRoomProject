using UnityEngine;
using Ubiq.Rooms;
using Ubiq.Messaging;

/// <summary>
/// This component handles the power selection portion of the game.
/// </summary>
namespace Ubiq.Samples
{
    public class PowerSelection : MonoBehaviour
    {
        [Tooltip("Ubiq main panel object")]
        public SocialMenu mainMenu;
        public Transform XROrigin;
        // Physical spawn positions for the 3 players in the character selection screen
        public Transform[] spawnPoints;
        public Transform tablePosition;
        // Track people in rooom
        private string[] _peersInRoom = { "", "", "" };
        private CharacterController _characterController;
        // Synchronize player availability with other copies
        NetworkContext context;
        // Keep track of room owner
        private string _roomOwner = "";

        void Start()
        {
            _characterController = XROrigin.gameObject.GetComponent<CharacterController>();
            if (_characterController == null)
            {
                Debug.LogWarning("Character Controller not found in the hierchy");
            }
            // Network
            context = NetworkScene.Register(this);

            mainMenu.roomClient.OnJoinedRoom.AddListener(OnJoin);
            // When someone joins your room, send message to tell them which spawn point is available
            mainMenu.roomClient.OnPeerAdded.AddListener(SendRoomAvailability);
            mainMenu.roomClient.OnPeerRemoved.AddListener(UpdatePeers);
        }

        /// <summary>
        /// When a peer leaves, the room owner updates which spot has been freed
        /// </summary>
        void UpdatePeers(IPeer peer)
        {
            // Update room availability (only the owner)
            if (mainMenu.roomClient.Me.uuid == _roomOwner)
            {
                // Find spot to free
                for (int i = 0; i < _peersInRoom.Length; i++)
                {
                    if (_peersInRoom[i] == peer.uuid)
                    {
                        _peersInRoom[i] = "";
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// When a client joins a room, enter power selection mode
        /// </summary>
        /// <param name="room"></param>
        void OnJoin(IRoom room)
        {
            if (!room.Publish)
            {
                // Non-shared room
                _roomOwner = "";
            }
            else
            {
                // Only enter selection mode if user is the room creator
                // Wait for room owner to be set 
                if (mainMenu.roomClient.Me.uuid == _roomOwner)
                {
                    EnterPowerSelection(0);
                    // Occupy spot
                    _peersInRoom[0] = _roomOwner;

                }
            }
        }

        /// <summary>
        ///  Transport player in chair and activate relevant components for power selection  
        /// </summary>
        void EnterPowerSelection(int spawnIndex)
        {
            // Compute delta difference between current position and target 
            _characterController.Move(spawnPoints[spawnIndex].position - Camera.main.transform.position);
            // Look at table
            XROrigin.LookAt(tablePosition.position);
        }

        /// <summary>
        /// Ubiq message struct for synchronization
        /// </summary>
        private struct TrackMsg
        {
            // An index of a free spawn point in the room
            public int freeSpawnPoint;
            // Pass UUID of room owner
            public string roomOwner;
        }

        // Send room availability
        private void SendRoomAvailability(IPeer peer)
        {
            if (mainMenu.roomClient.Me.uuid == _roomOwner)
            {
                // Only the owner sends the available spots
                var message = new TrackMsg();
                int freeSpot = -1;
                for (int i = 0; i < _peersInRoom.Length; i++)
                {
                    if (_peersInRoom[i] == "")
                    {
                        freeSpot = i;
                        // Update room availability
                        _peersInRoom[i] = peer.uuid;
                        break;
                    }
                }
                message.freeSpawnPoint = freeSpot;
                message.roomOwner = mainMenu.roomClient.Me.uuid;
                context.SendJson(message);
            }
        }

        // Network Ubiq: track rotation & position of grabbable objects
        public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
        {
            // Parse message
            var m = message.FromJson<TrackMsg>();
            // Update room owner string & check for free spots
            if (m.freeSpawnPoint != -1)
            {
                // A spot is available: teleport user to the correct place
                EnterPowerSelection(m.freeSpawnPoint);
                _roomOwner = m.roomOwner;
            }
            else
            {
                // Kick out user
            }
        }

        public void SetSelfAsOwner()
        {
            _roomOwner = mainMenu.roomClient.Me.uuid;
        }
    }
}