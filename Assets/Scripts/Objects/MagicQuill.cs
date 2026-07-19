using UnityEngine;

public class MagicQuill : MonoBehaviour
{
    // Potion ingredients
    public GameObject magicInkTipPrefab;
    public AudioClip collisionSound; 

    private GameObject _magicInkTip;
    private bool _dippedQuill = false; 
    private bool _hasSpawned = false; // this prevents ink from spawingin more than once
    private AudioSource _audioSource;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // After quill collides with MagicInkwell the ink tip is spawned (once)
        if (_dippedQuill && !_hasSpawned)
        {
            //relative position since the quill moves
            Vector3 spawnPosition = transform.TransformPoint(new Vector3(0f, 0f, 0.045f));

            // instantiate tip
            _magicInkTip = Instantiate(magicInkTipPrefab, spawnPosition, Quaternion.identity);

            // set tip child of quill so they move toghter
            _magicInkTip.transform.SetParent(transform);

            // avoid inf spawn
            _hasSpawned = true;
        }
    }

    // This function checks whether the quill collides with the MagicInkwell 
    void OnCollisionEnter(Collision col) {
        if(col.gameObject.name.Contains("MagicInkwell"))
        {
            Debug.Log("Dipped quill");
            _audioSource.PlayOneShot(collisionSound);
            _dippedQuill = true;
        }
    }
}
