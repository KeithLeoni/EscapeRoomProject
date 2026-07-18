using UnityEngine;
using System.Collections;
using System.Numerics;
using Unity.XR.CoreUtils;
using Ubiq.Avatars;


public class MagicBook : MonoBehaviour
{  
    private bool writtenWithMagicInk = false; 
    public AudioClip writingSound; 
    private AudioSource audioSource;
    public Transform dest;
    private bool _escaped = false;



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
            // Avoid echo
            if (_escaped) return;
            _escaped = true;
        
            audioSource.PlayOneShot(writingSound);
            Debug.Log("YOU ESCAPED!");

            // Find Local Avatar
            GameObject avatarManager = FindFirstObjectByType<AvatarManager>().gameObject;
            Ubiq.Avatars.Avatar[] avatars = avatarManager.GetComponentsInChildren<Ubiq.Avatars.Avatar>();
            XROrigin xrOrigin = FindFirstObjectByType<XROrigin>();
            Debug.Log(xrOrigin);

           
            CharacterController cc = xrOrigin.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            Debug.Log(cc);

            xrOrigin.transform.position = dest.position;
            xrOrigin.transform.rotation = dest.rotation;

            if(cc != null) cc.enabled = true;
               
        }
    }


}
