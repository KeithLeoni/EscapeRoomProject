using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class ChestTriggerDialogue : MonoBehaviour
{
    private CatSpeech _catSpeechScript;
    private MusicSequencePuzzleManager _musicScript;
    private bool talked = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _catSpeechScript = FindFirstObjectByType<CatSpeech>();
        _musicScript = FindFirstObjectByType<MusicSequencePuzzleManager>();
    }

    void OnTriggerEnter(Collider collider)
    {
        // Check if player collided with the area to trigger the dialogue
        if(collider.gameObject.name.Contains("XR Origin Hands") && !talked)
        {
            Debug.Log("Triggering chest dialogue");

            if (_catSpeechScript != null)
            {
                _catSpeechScript.Say("Well, it seems you found his chest… I don’t know what gibberish he keeps there, he is sooo unorganized…. But maybe it contains something useful... Anyways, you’ll need the key to open it. I would tell you… but I am sooo bored… why dont you play a my favourite song? Then, I’ll tell you! It's a great deal, besides, you don’t really have a choice, do you?”", 0);
            }

            talked = true;
            _musicScript.activateMusicPuzzle = true;

        }
    }
}