using UnityEngine;
using System.Collections;
using System.Numerics;

public class MagicBook : MonoBehaviour
{  
    private bool writtenWithMagicInk = false; 
    public AudioClip writingSound; 
    private AudioSource audioSource;
    public Transform dest;

    public Transform playerRig; 



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other) 
    {
        // Questo catturerà il trigger anche se l'oggetto è cinematico
        if (other.gameObject.name.Contains("MagicInkTip") || (other.transform.parent != null && other.transform.parent.name.Contains("MagicInkTip")))
        {
            audioSource.PlayOneShot(writingSound);
            Debug.Log("YOU ESCAPED!");

            CharacterController cc = playerRig.GetComponent<CharacterController>();
            if(cc != null) cc.enabled = false;

            // Teleport the rig to the new room's coordinates
            playerRig.position = dest.position;

            // Re-enable character controller
            if(cc != null) cc.enabled = true;

        }
    }


}
