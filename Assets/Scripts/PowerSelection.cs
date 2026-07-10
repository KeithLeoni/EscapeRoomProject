using UnityEngine;
using Ubiq.Rooms;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections.Generic;

/// <summary>
/// This component handles the power selection portion of the game.
/// </summary>
namespace Ubiq.Samples
{
    public class PowerSelection : MonoBehaviour
    {
        [Tooltip("Ubiq main panel object")]
        public SocialMenu mainMenu;
        // Track people in rooom
        private List<string> _peersInRoom = new List<string>();
        private int _maxPlayers = 3;
        public Spawner notepad;
        // All highlighter objects
        public List<GameObject> markers = new List<GameObject>();

        void Start()
        {
            mainMenu.roomClient.OnJoinedRoom.AddListener(OnJoin);
            // When someone joins your room, send message to tell them which spawn point is available
            mainMenu.roomClient.OnPeerAdded.AddListener(AddPeer);
            mainMenu.roomClient.OnPeerRemoved.AddListener(RemovePeer);

            if (notepad == null)
            {
                notepad = FindAnyObjectByType<Spawner>();
            }

            if (markers.Count < _maxPlayers)
            {
                // Find markers
                markers.Clear();
                markers.Add(GameObject.Find("BlueHighlighter").GetNamedChild("BluePen"));
                markers.Add(GameObject.Find("GreenHighlighter").GetNamedChild("GreenPen"));
                markers.Add(GameObject.Find("PinkHighlighter").GetNamedChild("PinkPen"));
            }
            // Highlighters: make them un-grabbable at first
            ToggleMarkers(false);
        }

        /// <summary>
        /// Disable/Enable interaction with highlighters
        /// </summary>
        private void ToggleMarkers(bool activationStatus)
        {
            foreach (var item in markers)
            {
                item.GetComponent<XRGrabInteractable>().enabled = activationStatus;
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
                // Non-shared room, reset peers
                ResetPeers();
                // Reset notepad state
                notepad.DeactivatePowerSelection();
                ToggleMarkers(false);
            }
            else
            {
                // Joined or created a shared room 
                // Is room full?
                bool rejectedJoin = IsRoomFull();
                if (rejectedJoin)
                {
                    // Kick out player
                    mainMenu.roomClient.Join("", false);
                    return;
                }
                // Enter Selection Mode: i.e. activate notepad functionalities
                notepad.ActivatePowerSelection(_maxPlayers);
                ToggleMarkers(true);
            }
        }

        private void AddPeer(IPeer arg0)
        {
            AddPeer(arg0.uuid);
        }
        
        private void RemovePeer (IPeer arg0)
        {
            _peersInRoom.Remove(arg0.uuid);
        }


        // ---------------------------------------------------------------

        /// <summary>
        /// Reset peers list
        /// </summary>
        private void ResetPeers()
        {
            _peersInRoom.Clear();
        }

        /// <summary>
        /// It returns true if there are already the maximum amount of
        /// allowed players in room.
        /// </summary>
        private bool IsRoomFull()
        {
            return _peersInRoom.Count == _maxPlayers;
        }

        /// <summary>
        /// Adds peer to _peersInRoom, but only if the array is not full.
        /// </summary>
        /// <returns>True if peer was added</returns>
        private bool AddPeer(string uuid)
        {
            if (_peersInRoom.Count < _maxPlayers)
            {
                _peersInRoom.Add(uuid);
                return true;
            }
            return false;
        }
        
    }
}