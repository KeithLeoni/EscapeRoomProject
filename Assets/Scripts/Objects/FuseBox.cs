using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Behaviour of fusebox to control lights based on inserted vials  
/// </summary>
public class FuseBox : MonoBehaviour
{
    private MeshRenderer _caseMeshRenderer;
    [Header("Light colors indicators")]
    public Material colorOn;
    public Material colorOff;
    public Transform grateDoor;
    // Vial sockets
    private GameObject _socket1;
    private GameObject _socket2;
    private GameObject _socket3;
    private GameObject _socket4;
    // Expected vials for everything to work
    private string[] _acceptedVials = { "τ ζ", "Δ ϡ", "ψ ϖ", "27" };
    // Track which lights are correct
    private bool[] _lights = { true, true, true, false };
    // Grate part
    private MeshRenderer _indicatorMeshRenderer;

    // Audio clips
    public AudioClip gateSoundEffect;

    private void Start()
    {
        _caseMeshRenderer = GetComponent<MeshRenderer>();
        // Find all sockets
        GameObject parentObject = gameObject.transform.parent.gameObject;
        _socket1 = parentObject.GetNamedChild("FuelHolder1");
        _socket2 = parentObject.GetNamedChild("FuelHolder2");
        _socket3 = parentObject.GetNamedChild("FuelHolder3");
        _socket4 = parentObject.GetNamedChild("FuelHolder4");
        // Add socket logic
        _socket1.GetComponent<XRSocketInteractor>().selectEntered.AddListener(UpdateLights1);
        _socket2.GetComponent<XRSocketInteractor>().selectEntered.AddListener(UpdateLights2);
        _socket3.GetComponent<XRSocketInteractor>().selectEntered.AddListener(UpdateLights3);
        _socket4.GetComponent<XRSocketInteractor>().selectEntered.AddListener(UpdateLights4);
        _socket1.GetComponent<XRSocketInteractor>().selectExited.AddListener(OnDeselect1);
        _socket2.GetComponent<XRSocketInteractor>().selectExited.AddListener(OnDeselect2);
        _socket3.GetComponent<XRSocketInteractor>().selectExited.AddListener(OnDeselect3);
        _socket4.GetComponent<XRSocketInteractor>().selectExited.AddListener(OnDeselect4);

        // Get Grate and Grate light indicator reference
        _indicatorMeshRenderer = (parentObject.transform.parent.gameObject).GetNamedChild("LightSignals").GetComponent<MeshRenderer>();
    }

    private void UpdateLights1(SelectEnterEventArgs args)
    {
        GameObject snappedObject = args.interactableObject.transform.gameObject;
        UpdateLights(snappedObject, 1);
    }

    private void UpdateLights2(SelectEnterEventArgs args)
    {
        GameObject snappedObject = args.interactableObject.transform.gameObject;
        UpdateLights(snappedObject, 2);
    }

    private void UpdateLights3(SelectEnterEventArgs args)
    {
        GameObject snappedObject = args.interactableObject.transform.gameObject;
        UpdateLights(snappedObject, 3);
    }

    private void UpdateLights4(SelectEnterEventArgs args)
    {
        GameObject snappedObject = args.interactableObject.transform.gameObject;
        UpdateLights(snappedObject, 4);
    }

    /// <summary>
    /// Change light indicators based on text on the snapped vial and socket number
    /// </summary>
    /// <param name="target"> Game object representing the snapped vial </param>
    /// <param name="socketNumber"> Number of socket the vial was snapped to </param>
    private void UpdateLights(GameObject target, int socketNumber)
    {
        string targetLabel = target.GetComponentInChildren<TextMeshProUGUI>().text;

        if (targetLabel != null && targetLabel == _acceptedVials[(socketNumber - 1)])
        {
            // correct option inserted
            ChangeMaterial(socketNumber, colorOn);
            _lights[socketNumber - 1] = true;
        }
        else
        {
            ChangeMaterial(socketNumber, colorOff);
            _lights[socketNumber - 1] = false;
        }

        // Check if all lights are on:
        if (_lights[0] && _lights[1] && _lights[2] && _lights[3])
        {
            // Open Grate
            grateDoor.GetComponent<AudioSource>().PlayOneShot(gateSoundEffect);
            grateDoor.GetComponent<Animator>().Play("OpenGate");
            // Change indicator colors
            Material[] materials = _indicatorMeshRenderer.materials;
            materials[1] = colorOn;
            _indicatorMeshRenderer.materials = materials;
        }
    }

    private void OnDeselect1(SelectExitEventArgs args)
    {
        ChangeMaterial(1, colorOff);
        _lights[0] = false;
    }

    private void OnDeselect2(SelectExitEventArgs args)
    {
        ChangeMaterial(2, colorOff);
        _lights[1] = false;
    }

    private void OnDeselect3(SelectExitEventArgs args)
    {
        ChangeMaterial(3, colorOff);
        _lights[2] = false;
    }

    private void OnDeselect4(SelectExitEventArgs args)
    {
        ChangeMaterial(4, colorOff);
        _lights[3] = false;
    }

    /// <summary>
    /// This method changes material of the fuse box mesh renderer
    /// </summary>
    /// <param name="index"> Index in mesh renderer of the material </param>
    /// <param name="newMaterial"> Material to change the light to </param>
    private void ChangeMaterial(int index, Material newMaterial)
    {
        Material[] materials = _caseMeshRenderer.materials;
        materials[(2 + index)] = newMaterial;
        _caseMeshRenderer.materials = materials;
    }
}

