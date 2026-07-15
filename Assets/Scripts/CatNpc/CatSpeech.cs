using UnityEngine;
using TMPro;
public class CatSpeech : MonoBehaviour
{
    private TextMeshPro _speechBubbleText;
    private  AudioSource _speechBubbleAudio;
    public AudioClip normalSound;
    public AudioClip happySound;
    public AudioClip angrySound;
    
    /// <summary>
    /// Function to make cat talk.   
    /// </summary>
    /// <param name="text"> What you want the cat to say. </param>
    /// <param name="mode"> Modality: 0-normal (default), 1-happy, 2-angry </param>
    public void Say(string text, int mode)
    {   
        // Get bubble TextMeshPro and AudioSource components
        _speechBubbleText = GetComponentInChildren<TextMeshPro>();
        _speechBubbleAudio =  GetComponentInChildren<AudioSource>();

        // Change text
        _speechBubbleText.text = text;

        // Select correct audio
        AudioClip audioClip;
        switch(mode){
            case 0: audioClip = normalSound; break;
            case 1: audioClip = happySound; break;
            case 2: audioClip = angrySound; break;
            default: audioClip = normalSound; break;
        }

        // Play sound
        _speechBubbleAudio.clip = audioClip;
        _speechBubbleAudio.Play();
    }
}
