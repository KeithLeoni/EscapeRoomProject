using UnityEngine;
using Ubiq.Messaging;

public class ChestTriggerDialogue : MonoBehaviour
{
    private MusicSequencePuzzleManager _musicScript;

    // For dialogue
    private CatSpeech _catSpeechScript;

    // Variable to avoid multiple triggers of the same dialogue
    private bool _talked = false;
    
    // For synch.
    NetworkContext context;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get context
        context = NetworkScene.Register(this);

        // Get script for NPC speech
        _catSpeechScript = FindFirstObjectByType<CatSpeech>();

        // Get script for music puzzle
        _musicScript = FindFirstObjectByType<MusicSequencePuzzleManager>();
    }

    // This function triggers dialogue when player enters in the area
    // It also activates the music puzzle
    void OnTriggerEnter(Collider collider)
    {
        // Check if player collided with the area to trigger the dialogue
        if (collider.gameObject.name.Contains("XR Origin Hands") && !_talked)
        {
            Debug.Log("Triggering chest dialogue");

            // If the NPC hasn't talked yet, trigger dialogue
            if (_catSpeechScript != null)
            {
                _catSpeechScript.Say("Well, it seems you found his chest… I don’t know what gibberish he keeps there, he is sooo unorganized…. But maybe it contains something useful... Anyways, you’ll need the key to open it. I would tell you… but I am sooo bored… why dont you play a my favourite song? Then, I’ll tell you! It's a great deal, besides, you don’t really have a choice, do you?”", 0);
            }

            // Set varibale to true to avoid triggering the dialogue again
            _talked = true;

            // Activate the music puzzle now that the players have the correct NPC instructions
            _musicScript.activateMusicPuzzle = true;

        }
    }

    // Struct to track the activation of the music puzzle
    private struct TrackActivateMusicPuzzle
    {
        // Simple bool to track the state
        public bool activateMusicPuzzle;
    }

    // Send message to track the activation of the music puzzle
    private void SendActivateMusicPuzzleMessage()
    {
        var message = new TrackActivateMusicPuzzle();
        message.activateMusicPuzzle = true;
        context.SendJson(message);
    }

    // Process message to track the activation of the music puzzle
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var m = message.FromJson<TrackActivateMusicPuzzle>();
        _musicScript.activateMusicPuzzle = m.activateMusicPuzzle;
    }
}