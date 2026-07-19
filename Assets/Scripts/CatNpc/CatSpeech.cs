using UnityEngine;
using TMPro;
using Ubiq.Messaging;

public class CatSpeech : MonoBehaviour
{
    // Bubble text and audio components
    private TextMeshPro _speechBubbleText;
    private AudioSource _speechBubbleAudio;
    
    // Audio clips
    public AudioClip normalSound;
    public AudioClip happySound;
    public AudioClip angrySound;
    
    // For synch.
    NetworkContext context;

    public void Start()
    {
        // Get context
        context = NetworkScene.Register(this);

        // Get bubble TextMeshPro and AudioSource components
        _speechBubbleText = GetComponentInChildren<TextMeshPro>();
        _speechBubbleAudio = GetComponentInChildren<AudioSource>();
    }

    /// <summary>
    /// Function to make cat talk. Includes synchronization.  
    /// </summary>
    /// <param name="text"> What you want the cat to say. </param>
    /// <param name="mode"> Modality: 0-normal (default), 1-happy, 2-angry </param>
    public void Say(string text, int mode)
    {
        SayLocal(text, mode);

        SendTrackDialogueMessage(text, mode);
    }

    /// <summary>
    /// Function to make cat talk locally. Excludes synchronization (e.g. to be used in ProcessMessage to avoid "Say" loops)
    /// </summary>
    public void SayLocal(string text, int mode)
    {
        // Change text
        _speechBubbleText.text = text;

        // Select correct audio based on mode
        AudioClip audioClip;
        switch (mode)
        {
            case 0: audioClip = normalSound; break;
            case 1: audioClip = happySound; break;
            case 2: audioClip = angrySound; break;
            default: audioClip = normalSound; break;
        }

        // Play sound
        _speechBubbleAudio.clip = audioClip;
        _speechBubbleAudio.Play();
        
        Debug.Log("SayLocal");
    }
    
    // Struct to track dialogue
    private struct TrackDialogue
    {
        public string text;
        public int mode;
    }

    // Send message to track dialogue
    private void SendTrackDialogueMessage(string text, int mode)
    {
        var message = new TrackDialogue();
        message.text = text;
        message.mode = mode;
        context.SendJson(message);
    }

    // Process message to track dialogue
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var m = message.FromJson<TrackDialogue>();
        SayLocal(m.text, m.mode);
    }

}
