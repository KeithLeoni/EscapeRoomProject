using UnityEngine;

public class CauldronScript : MonoBehaviour
{
    // Potion ingredients
    private bool _potionGreen, _potionRed, _ivyLeaf = false;

    // Prefab of the magic ink that will spawn 
    public GameObject magicInkPrefab;
    // Spawning height of the magicInkPrefab
    [SerializeField] private float _spawnHeightOffset = 0.2f; // y offset
    private GameObject _magicInk;
    // Variable to prevent ink from spawingin more than once
    private bool _hasSpawned = false; 

    // Check if all ingredients have been added
    void Update()
    {
        // When all ingredients have been added, spawn the magic ink on top of the cauldron
        if (_potionGreen && _potionRed && _ivyLeaf && !_hasSpawned)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0f, _spawnHeightOffset, 0f);
            _magicInk = Instantiate(magicInkPrefab, spawnPosition, Quaternion.identity);

            // Set to true to avoid infinite spawns
            _hasSpawned = true;
        }
    }

    // When an object collides with the cauldron, the script checks the object name
    // If the object is one of the ingredients of the potion, it gets "added" to the potion (in practice deleted from scene)
    void OnCollisionEnter(Collision col) {
        if(col.gameObject.name == "PuzzlePotionGreen")
            {   
                _potionGreen = true;
                Destroy(col.gameObject);
            }
        else if(col.gameObject.name == "PuzzlePotionRed")
            {
                _potionRed = true;
                Destroy(col.gameObject);
            }
        else if(col.gameObject.name.Contains("IvyLeaf")) // Any leaf works
            {
                _ivyLeaf = true;
                Destroy(col.gameObject);
            }
    }
}
