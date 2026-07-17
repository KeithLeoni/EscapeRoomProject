using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Ubiq.Messaging;


public class MusicPuzzleButton : MonoBehaviour
{
    [Header("Button Type")]
    public bool isSampleButton = false;

    [Header("Music Button ID")]
    public int buttonId = 0;
    // Mi = 0
    // Do = 1
    // Re = 2

    [Header("Button Sound")]
    public AudioClip buttonSound;
    public AudioSource audioSource;

    [Header("Puzzle Manager")]
    public MusicSequencePuzzleManager puzzleManager;

    [Header("Button Visual Press")]
    public Transform buttonTop;
    public Vector3 pressDirection = new Vector3(0f, -1f, 0f);
    public float pressDepth = 0.03f;
    public float moveSpeed = 12f;

    [Header("Cooldown")]
    public float pressCooldown = 0.3f;

    private XRSimpleInteractable simpleInteractable;
    private Vector3 normalLocalPosition;
    private Vector3 pressedLocalPosition;
    private bool isPressed = false;
    private float nextAllowedPressTime = 0f;

    // For synch.
    NetworkContext context;

    void Start()
    {
        simpleInteractable = GetComponent<XRSimpleInteractable>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (puzzleManager == null)
            puzzleManager = FindFirstObjectByType<MusicSequencePuzzleManager>();

        if (buttonTop == null)
            buttonTop = transform;

        normalLocalPosition = buttonTop.localPosition;
        pressedLocalPosition = normalLocalPosition + pressDirection.normalized * pressDepth;

        if (simpleInteractable != null)
        {
            simpleInteractable.selectEntered.AddListener(OnButtonPressed);
            simpleInteractable.selectExited.AddListener(OnButtonReleased);
        }

        // Get context
        context = NetworkScene.Register(this);
    }

    void Update()
    {
        Vector3 targetPosition = isPressed ? pressedLocalPosition : normalLocalPosition;

        buttonTop.localPosition = Vector3.Lerp(
            buttonTop.localPosition,
            targetPosition,
            Time.deltaTime * moveSpeed
        );
    }

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        if (Time.time < nextAllowedPressTime)
            return;

        nextAllowedPressTime = Time.time + pressCooldown;
        isPressed = true;
        SendTrackPressMessage();

        if (audioSource != null && buttonSound != null)
            audioSource.PlayOneShot(buttonSound);

        if (puzzleManager == null)
            return;

        if (isSampleButton)
        {
            puzzleManager.PlaySample();
            Debug.Log("Sample button pressed.");
        }
        else
        {
            puzzleManager.PressMusicButton(buttonId);
            Debug.Log("Music button pressed: " + buttonId);
        }
    }

    private void OnButtonReleased(SelectExitEventArgs args)
    {
        isPressed = false;
        SendTrackPressMessage();
    }

    private void OnDestroy()
    {
        if (simpleInteractable != null)
        {
            simpleInteractable.selectEntered.RemoveListener(OnButtonPressed);
            simpleInteractable.selectExited.RemoveListener(OnButtonReleased);
        }
    }

    // Struct to track pressing
    private struct TrackPressMsg
    {
        public bool isPressed;
    }

    // Send message to track pressing
    private void SendTrackPressMessage()
    {
        var message = new TrackPressMsg();
        message.isPressed = isPressed;
        context.SendJson(message);
    }

    // Process message to track pressing
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var m = message.FromJson<TrackPressMsg>();
        isPressed = m.isPressed;
        if (isPressed)
        {
            audioSource.PlayOneShot(buttonSound);
            if (isSampleButton)
            {
                puzzleManager.PlaySample();
            }
            else
            {
                puzzleManager.PressMusicButton(buttonId);
            }
        }
    }
}