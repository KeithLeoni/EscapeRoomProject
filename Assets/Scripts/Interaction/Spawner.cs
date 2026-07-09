using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Ubiq.Messaging;
using Ubiq.Spawning;

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
    public NetworkSpawnManager networkSpawner;
    [Tooltip("Maximum number of objects each user is allowed to spawn")]
    public int maxSpawningCount = 3;
    // Setup for validity checking
    private NetworkContext _context;
    public ScenePowerManager.Power[] selectedPowers = { ScenePowerManager.Power.nothing, ScenePowerManager.Power.nothing, ScenePowerManager.Power.nothing };

    void Start()
    {
        _context = NetworkScene.Register(this);
    }

    /// <summary>
    /// Spawn <see langword="object"/> with peer scope  
    /// </summary>
    void SpawnObject(SelectEnterEventArgs args)
    {
        // Check spwning limit
        if (maxSpawningCount > 0)
        {
            GameObject spawnedObject = NetworkSpawnManager.Find(this).SpawnWithPeerScope(objectToSpawn);
            // Change position
            spawnedObject.transform.SetParent(null);
            spawnedObject.transform.position = spawnPoint.position;
            maxSpawningCount -= 1;
        }
    }

    // ----------------------------------------
    // Network control
    
    // Message to synchronize selected powers across all copies
    private struct UpdateMsg
    {
        public ScenePowerManager.Power power;
        public string paperOwner;
    }
    private void SendMessage(ScenePowerManager.Power selectedPower, string owner)
    {
        var message = new UpdateMsg();
        message.power = selectedPower;
        message.paperOwner = owner;
        _context.SendJson(message);
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        // Parse message
        var m = message.FromJson<UpdateMsg>();
        // Update status
        //selectedPowers
    }

}
