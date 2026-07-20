using UnityEngine;
using Unity.XR.CoreUtils;
using Ubiq.Avatars;
using TMPro;

public class MagicBook : MonoBehaviour
{
    public AudioClip writingSound;

    // Destination of the players after the teleport
    public Transform dest;

    private bool _escaped = false;
    private AudioSource _audioSource;
    public TextMeshPro textToUpdate;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("agdhfgfjs");
        // Get collsion of the MagicInkTip with the MagicBook
        if (col.gameObject.name.Contains("MagicInkTip") || (col.transform.parent != null && col.transform.parent.name.Contains("MagicInkTip")))
        {
            Invoke(nameof(Teleport), 0.5f);
        }
    }

    private void Teleport()
    {
        // Avoid echo of writing sound due to synch.
        if (_escaped) return;
        _escaped = true;

        // Play writing sound
        _audioSource.PlayOneShot(writingSound);

        // Find local avatar and get xrOrigin
        GameObject avatarManager = FindFirstObjectByType<AvatarManager>().gameObject;
        Ubiq.Avatars.Avatar[] avatars = avatarManager.GetComponentsInChildren<Ubiq.Avatars.Avatar>();
        XROrigin xrOrigin = FindFirstObjectByType<XROrigin>();

        // From xrOrigin get CharacterController
        CharacterController cc = xrOrigin.GetComponent<CharacterController>();

        // Momentarly de-activate the CharacterController
        if (cc != null) cc.enabled = false;

        // Add a radom offset to the destination position to avoid all avatars spawning in the same spot
        Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        dest.position = dest.position + randomOffset;

        // Move the avatar to the destination
        xrOrigin.transform.position = dest.position;
        xrOrigin.transform.rotation = dest.rotation;

        // Re-activate the CharacterController
        if (cc != null) cc.enabled = true;
        textToUpdate.text = "YOU ESCAPED!";

    }

}
