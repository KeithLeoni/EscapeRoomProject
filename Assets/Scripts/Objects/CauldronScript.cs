using UnityEngine;

public class CauldronScript : MonoBehaviour
{
    // Potion ingredients
    private bool potionGreen, potionRed, ivyLeaf = false;    
    public GameObject magicInkPrefab;
    [SerializeField] private float spawnHeightOffset = 0.2f; // y offset
    private GameObject magicInk;
    private bool hasSpawned = false; // this prevents ink from spawingin more than once


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (potionGreen && potionRed && ivyLeaf && !hasSpawned)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0f, spawnHeightOffset, 0f);
            magicInk = Instantiate(magicInkPrefab, spawnPosition, Quaternion.identity);
            hasSpawned = true;
        }
    }

    void OnCollisionEnter(Collision col) {
        if(col.gameObject.name == "PuzzlePotionGreen")
            {   
                potionGreen = true;
                Destroy(col.gameObject);
            }
        else if(col.gameObject.name == "PuzzlePotionRed")
            {
                potionRed = true;
                Destroy(col.gameObject);
            }
        else if(col.gameObject.name.Contains("IvyLeaf"))
            {
                ivyLeaf = true;
                Destroy(col.gameObject);
            }
    }
}
