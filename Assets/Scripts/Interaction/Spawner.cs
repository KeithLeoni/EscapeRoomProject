using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Ubiq.Spawning;

/// <summary>
/// Spawns specified element when player "grabs" it 
/// </summary>
public class Spawner : XRBaseInteractable
{
    // Prefab to spawn
    public GameObject objectToSpawn;
    [Tooltip("Spawning point relative to the world")]
    public Transform spawnPoint;
    public NetworkSpawnManager networkSpawner;
    [Tooltip("Maximum number of objects each user is allowed to spawn")]
    public int maxSpawningCount = 3;
    private Outline _outlineComponent;

    void Start()
    {
        // Add outline component
        _outlineComponent = gameObject.AddComponent<Outline>();
        _outlineComponent.enabled = false;
        // Set up outline look
        _outlineComponent.OutlineWidth = 4;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        selectEntered.AddListener(SpawnObject);
        // Outline component management
        hoverEntered.AddListener(EnableHighlight);
        hoverExited.AddListener(DisableHighlight);
    }

    protected override void OnDisable()
    {
        selectEntered.RemoveListener(SpawnObject);
        // Outline component
        hoverEntered.RemoveListener(EnableHighlight);
        hoverExited.RemoveListener(DisableHighlight);
        base.OnDisable();
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

    /// <summary>
    /// Enable Outline highlight when hovering  
    /// </summary>
    void EnableHighlight(HoverEnterEventArgs args)
    {
        _outlineComponent.enabled = true;
    }

    /// <summary>
    /// Disable Outline highlight when exiting hover state 
    /// </summary>
    void DisableHighlight(HoverExitEventArgs args)
    {
        _outlineComponent.enabled = false;
    }
}
