using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DrawableArea : MonoBehaviour
{
    public AudioClip writingSoundEffect1;
    public AudioClip writingSoundEffect2;
    public AudioClip writingSoundEffect3;
    public Material flyingMaterial;
    public Material jellyMaterial;
    public Material sizeMaterial;

    private AudioSource _audioSource;

    // Marker drawing settings
    private bool _hasEntered = false;
    private PaperSheet _paperSheet;
    private MeshRenderer _meshRenderer;
    private Material _drawingToShow;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _paperSheet = gameObject.GetComponentInParent<PaperSheet>();
        // Change Draw material to visualize the drawing
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    // Define Drawing collider behaviour
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.EndsWith("Pen") && !_hasEntered)
        {
            // Check that marker is being grabbed
            XRGrabInteractable interactable = other.gameObject.GetComponent<XRGrabInteractable>();
            if (interactable == null || !interactable.isSelected)
            {
                // Marker is not being grabbed
                return;
            }
            _hasEntered = true;
            // Check which highlighter it is
            if (other.gameObject.name.StartsWith("Blue"))
            {
                // Play sound effect
                _audioSource.PlayOneShot(writingSoundEffect1);
                // Assign right material
                // Register selected power
                _paperSheet.selectedPower = ScenePowerManager.Power.flyingPower;
                _drawingToShow = flyingMaterial;
                _meshRenderer.material = flyingMaterial;
                Debug.Log("Fly Selected");

            }
            else if (other.gameObject.name.StartsWith("Pink"))
            {
                // Play sound effect
                _audioSource.PlayOneShot(writingSoundEffect2);
                _paperSheet.selectedPower = ScenePowerManager.Power.sizeManipulationPower;
                _drawingToShow = sizeMaterial;
                Debug.Log("Size Selected");
            }
            else if (other.gameObject.name.StartsWith("Green"))
            {
                // Play sound effect
                _audioSource.PlayOneShot(writingSoundEffect3);
                _paperSheet.selectedPower = ScenePowerManager.Power.jellyVision;
                _drawingToShow = jellyMaterial;
                Debug.Log("Jelly Selected");
            }
            // Delay showing to draw
            Invoke(nameof(ShowDrawing), 0.5f);
            // Communicate with notepad to coordinate remote copies
            _paperSheet.notepad.SendUpdateMessage();
        }
    }

    private void ShowDrawing()
    {
        _meshRenderer.material = _drawingToShow;
    }

    void OnTriggerExit(Collider other)
    {
        // Reset variable
        _hasEntered = false;
    }
}