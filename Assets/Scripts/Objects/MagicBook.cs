using UnityEngine;
using System.Collections;
using Unity.XR.CoreUtils;
using Ubiq.Avatars;


public class MagicBook : MonoBehaviour
{


    public AudioClip writingSound;

    // Destination of the players after the teleport
    public Transform dest;
    
    private bool _escaped = false;
    private AudioSource _audioSource;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    // void Update()
    // {
    // }

    void OnTriggerEnter(Collider col) 
    {
        // Get collsion of the MagicInkTip with the MagicBook
        if (col.gameObject.name.Contains("MagicInkTip") || (col.transform.parent != null && col.transform.parent.name.Contains("MagicInkTip")))
        {
            // Avoid echo of writing sound due to synch.
            if (_escaped) return;
            _escaped = true;
            
            Debug.Log("YOU ESCAPED!");

            // Play writing sound
            _audioSource.PlayOneShot(writingSound);
            
            // Find local avatar and get xrOrigin
            GameObject avatarManager = FindFirstObjectByType<AvatarManager>().gameObject;
            Ubiq.Avatars.Avatar[] avatars = avatarManager.GetComponentsInChildren<Ubiq.Avatars.Avatar>();
            XROrigin xrOrigin = FindFirstObjectByType<XROrigin>();
            //Debug.Log(xrOrigin);

            // From xrOrigin get CharacterController
            CharacterController cc = xrOrigin.GetComponent<CharacterController>();

            // Momentarly de-activate the CharacterController
            if (cc != null) cc.enabled = false;
            //Debug.Log(cc);

            // Add a radom offset to the destination position to avoid all avatars spawning in the same spot
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            dest.position = dest.position + randomOffset;

            // Move the avatar to the destination
            xrOrigin.transform.position = dest.position;
            xrOrigin.transform.rotation = dest.rotation;

            // Re-activate the CharacterController
            if(cc != null) cc.enabled = true;
               
        }
    }


}
