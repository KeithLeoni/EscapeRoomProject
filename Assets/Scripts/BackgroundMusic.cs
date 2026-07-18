using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioClip tensionMusic; 
    public AudioClip initialMusic;
    private AudioSource audioSource;

    public bool roomChanged = true;
    public bool mainRoom = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (roomChanged)
        {
            roomChanged = false;

            // Stop music
            audioSource.Stop();

            audioSource.loop = true;

            // Switch music for different scenarios
            if (mainRoom)
            {
                audioSource.clip = tensionMusic;
                audioSource.Play();
            }
            else
            {
                audioSource.clip = initialMusic;
                audioSource.Play();
            }
        }
    }
}
