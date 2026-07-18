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

            // Find Local Avatar
            GameObject avatarManager = FindFirstObjectByType<AvatarManager>().gameObject;
            Ubiq.Avatars.Avatar[] avatars = avatarManager.GetComponentsInChildren<Ubiq.Avatars.Avatar>();
            XROrigin xrOrigin = FindFirstObjectByType<XROrigin>();
            
            foreach (var avatar in avatars)
            {
                // Move only local avatar to avoid conflict
                if (avatar.IsLocal)
                {
                    CharacterController cc = xrOrigin.GetComponent<CharacterController>();
                    if(cc != null) cc.enabled = false;

                    xrOrigin.transform.position = dest.position;
                    xrOrigin.transform.rotation = dest.rotation;

                    if(cc != null) cc.enabled = true;

                    break;
                }
            }            

        }
    }


}
