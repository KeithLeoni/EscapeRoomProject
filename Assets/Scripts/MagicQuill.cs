using UnityEngine;

public class MagicQuill : MonoBehaviour
{
    // Potion ingredients
    public GameObject magicInkTipPrefab;
    private GameObject magicInkTip;
    private bool dippedQuill = false; // this prevents ink from spawingin more than once
    private bool hasSpawned = false;
    public AudioClip collisionSound; 
    private AudioSource audioSource;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dippedQuill && !hasSpawned)
        {
            //relative position since the quill moves
            Vector3 spawnPosition = transform.TransformPoint(new Vector3(0f, 0f, 0.045f)); 

            // instantiate tip
            magicInkTip = Instantiate(magicInkTipPrefab, spawnPosition, Quaternion.identity);

            // set tip child of quill so they move toghter
            magicInkTip.transform.SetParent(transform);

            // avoid inf spawn
            hasSpawned = true;
        }
    }

    void OnCollisionEnter(Collision col) {
        if(col.gameObject.name.Contains("MagicInkwell"))
        {
            Debug.Log("La piuma è ENTRATA nel trigger");
            audioSource.PlayOneShot(collisionSound);
            dippedQuill = true;
        }
    }
}
