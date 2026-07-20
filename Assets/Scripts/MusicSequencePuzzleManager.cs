using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MusicSequencePuzzleManager : MonoBehaviour
{
    [Header("Sample Audio Sequence")]
    public AudioSource audioSource;
    public AudioClip miClip;
    public AudioClip doClip;
    public AudioClip reClip;
    public float delayBetweenClips = 0.35f;

    [Header("Result Sounds")]
    public AudioClip successSound;
    public AudioClip failSound;

    [Header("Object Unlocked When Solved")]
    public XRGrabInteractable objectToEnableGrab;
    public Rigidbody objectRigidbody;
    public Collider objectCollider;

    private int currentStep = 0;
    private bool isPlayingSample = false;
    private bool sampleFinished = false;
    private bool puzzleSolved = false;

    private int[] correctOrder = { 1, 2, 3 };
    // 0 = Mi
    // 1 = Do
    // 2 = Re

    // For dialogue
    private CatSpeech _catSpeechScript;

    // To avoid puzzle being solved before cat dialogue explaining the puzzle
    public bool activateMusicPuzzle;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (objectToEnableGrab != null)
            objectToEnableGrab.enabled = false;

        if (objectCollider != null)
            objectCollider.enabled = true;
        
        _catSpeechScript = FindFirstObjectByType<CatSpeech>();
    }

    public void PlaySample()
    {
        if (isPlayingSample || puzzleSolved)
            return;

        if (activateMusicPuzzle)
        {
            StartCoroutine(PlaySampleRoutine());
        }
    }

    private IEnumerator PlaySampleRoutine()
    {
        isPlayingSample = true;
        sampleFinished = false;
        currentStep = 0;

        Debug.Log("Playing sample: Mi Do Re");

        yield return PlayClip(miClip);
        yield return new WaitForSeconds(delayBetweenClips);

        yield return PlayClip(doClip);
        yield return new WaitForSeconds(delayBetweenClips);

        yield return PlayClip(reClip);

        isPlayingSample = false;
        sampleFinished = true;

        Debug.Log("Sample finished. Player can now press Mi Do Re.");
    }

    private IEnumerator PlayClip(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void PressMusicButton(int buttonId)
    {
        if (puzzleSolved)
            return;

        if (isPlayingSample)
        {
            Debug.Log("Ignored: sample is still playing.");
            return;
        }

        if (!sampleFinished)
        {
            Debug.Log("Ignored: press sample button first.");
            return;
        }

        int expectedButton = correctOrder[currentStep];

        Debug.Log("Pressed: " + buttonId + " Expected: " + expectedButton);

        if (buttonId == expectedButton)
        {
            currentStep++;

            if (currentStep >= correctOrder.Length)
            {
                SolvePuzzle();
            }
        }
        else
        {
            FailPuzzle();
        }
    }

    private void FailPuzzle()
    {
        currentStep = 0;

        _catSpeechScript.SayLocal("NO! That’s not it!", 2);
    }

    private void SolvePuzzle()
    {
        puzzleSolved = true;

        if (audioSource != null && successSound != null)
            audioSource.PlayOneShot(successSound);

        if (objectToEnableGrab != null)
            objectToEnableGrab.enabled = true;

        if (objectCollider != null)
            objectCollider.enabled = true;

        Debug.Log("Music puzzle solved. Object is now grabbable.");

        _catSpeechScript.SayLocal("Yes! Thank you, that was amazing! You can find the key under the other chair.", 1);
    }
}