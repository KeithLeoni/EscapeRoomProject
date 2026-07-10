using UnityEngine;
using Ubiq.Messaging;
using Ubiq.Spawning;
using Ubiq.Rooms;
using System.Collections.Generic;

/// <summary>
/// Spawns specified element
/// </summary>
public class Spawner : MonoBehaviour
{
    public NetworkId NetworkId { get; set; }
    // Prefab to spawn
    public GameObject objectToSpawn;
    [Tooltip("Spawning point relative to the world")]
    public Transform spawnPoint;

    // Setup for validity checking
    // Keep track of current selection
    private List<PaperSheet> _spawnedItems = new List<PaperSheet>();
    private Notepad _notepadObject;

    void Start()
    {
        // Network spawner events
        NetworkSpawnManager.Find(this).OnSpawned.AddListener(OnObjectSpawn);
        NetworkSpawnManager.Find(this).OnDespawned.AddListener(OnObjectDespawn);
        _notepadObject = GetComponent<Notepad>();
    }

    /// <summary>
    /// Allow notepad to start spawning & managing papersheets.
    /// Additionally maximum player count is updated
    /// </summary>
    public void ActivatePowerSelection(int maxPlayers)
    {
        // Spawn first Paper Sheet
        SpawnObject();
    }

    /// <summary>
    /// Reset room
    /// </summary>
    public void DeactivatePowerSelection()
    {
        // Destroy all spawned items
        for (int i = 0; i < _spawnedItems.Count; i++)
        {
            // Destroy GameObject
            Destroy(_spawnedItems[i].gameObject);
        }
        // Empty list
        _spawnedItems.Clear();
    }

    /// <summary>
    /// Spawn <see langword="object"/> with peer scope  
    /// </summary>
    public void SpawnObject()
    {
        GameObject spawnedObject = NetworkSpawnManager.Find(this).SpawnWithPeerScope(objectToSpawn);
        // Change position
        spawnedObject.transform.SetParent(null);
        spawnedObject.transform.position = spawnPoint.position;
    }

    /// <summary>
    /// Make object non-ineractable for local avatar
    /// </summary>
    private void DeactivateObjectLocally(GameObject spawnedObject)
    {
        // Disable all interactable components of this object locally
        Collider[] colliders = spawnedObject.GetComponents<BoxCollider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }

    /// <summary>
    /// Update spawned objects list
    /// </summary>
    private void OnObjectSpawn(GameObject arg0, IRoom arg1, IPeer arg2, NetworkSpawnOrigin arg3)
    {
        // Update list
        _spawnedItems.Add(arg0.GetComponent<PaperSheet>());
        // Check: did local avatar already claimed a paper sheet?
        PaperSheet claimedObject = FindLocalSpawnedObj();
        if (claimedObject != null)
        {
            DeactivateObjectLocally(arg0);
        }
    }

    /// <summary>
    /// Update spawned objects list
    /// </summary>
    private void OnObjectDespawn(GameObject arg0, IRoom arg1, IPeer arg2)
    {
        // Update list
        _spawnedItems.Remove(arg0.GetComponent<PaperSheet>());
    }

    /// <summary>
    /// Returns PaperSheet component owned by avatar with given uuid from the array
    /// of spawned objects in the scene. Returns null if nothing is found
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    private PaperSheet FindSpawnedObjByUuid(string uuid)
    {
        for (int i = 0; i < _spawnedItems.Count; i++)
        {
            if (_spawnedItems[i].owner == uuid)
            {
                return _spawnedItems[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Find Spawned object in scene that belongs to local avatar.
    /// </summary>
    /// <returns>null, if there is no spawned object claimed by the local avatar</returns>
    private PaperSheet FindLocalSpawnedObj()
    {
        for (int i = 0; i < _spawnedItems.Count; i++)
        {
            if (_spawnedItems[i].isLocal)
            {
                return _spawnedItems[i];
            }
        }
        return null;
    }

    // ----------------------------------------
    // Network control

    public void SendUpdateMessage()
    {
        // First Update Local State
        PaperSheet localObj = FindLocalSpawnedObj();
        UpdateLocalState(localObj);
        // Send network messages
        _notepadObject.SendUpdateMessage(localObj.selectedPower, localObj.owner);
    }

    /// <summary>
    /// Checks if power selection has any conflicts (i.e if anyone has chosen the same power),
    /// given the new power selection.
    /// Updates all conflict status and returns true if conflict involves the local avatar's
    /// component
    /// </summary>
    public bool IsSelectionValid(PaperSheet componentToUpdate)
    {
        bool isValid = true;
        foreach (var selection in _spawnedItems)
        {
            // Check if they have the same selection
            if (selection.selectedPower == componentToUpdate.selectedPower
            && selection.owner != componentToUpdate.owner)
            {
                // Update conflict status locally
                selection.validityStatus = false;
                // Update validity status
                if (selection.isLocal)
                {
                    isValid = false;
                }
            }
        }
        // Update validity status for componentToUpdate
        componentToUpdate.validityStatus = isValid;
        return isValid;
    }

    public void ProcessMessage(ScenePowerManager.Power power, string paperOwner)
    {
        // Update status of selected powers
        PaperSheet componentToUpdate = FindSpawnedObjByUuid(paperOwner);
        if (componentToUpdate == null)
        {
            // Error log
            Debug.LogError("PaperSheet component to update not found in local scene!");
            return;
        }
        // Update values
        componentToUpdate.selectedPower = power;
        componentToUpdate.owner = paperOwner;

        UpdateLocalState(componentToUpdate);
    }

    /// <summary>
    /// Checks selection validity and displays local warning
    /// </summary>
    private void UpdateLocalState(PaperSheet componentToUpdate)
    {
        // Start validity checking
        if (!(IsSelectionValid(componentToUpdate)))
        {
            // TODO: Display warning for local copy only
            Debug.Log("CONFLICT DETECTED");
            return;
        }
    }

}
