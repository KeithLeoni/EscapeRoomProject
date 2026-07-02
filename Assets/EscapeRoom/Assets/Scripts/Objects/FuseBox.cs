using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class FuseBox : MonoBehaviour
{
    private MeshRenderer caseMeshRenderer;
    [Header("Light colors indicators")]
    public Material colorOn;
    public Material colorOff;
    private GameObject socket1;
    private GameObject socket2;
    private GameObject socket3;
    private GameObject socket4;
    // Expected vials for everything to work
    private string[] acceptedVials = { "τ ζ", "Δ ϡ", "ψ ϖ", "27" };
    // Track which lights are correct
    private bool[] lights = { true, true, true, false };
    // Grate part
    private MeshRenderer indicatorMeshRenderer;
    private Transform grateDoor;
    private bool animateGrate = false;
    // Grate opening variables
    private Vector3 rotationPoint;
    private Quaternion finalRotationTarget;
    private float angularSpeed = -100f;
    private float prevDiff = 190f;

    private void Start()
    {
        caseMeshRenderer = GetComponent<MeshRenderer>();
        // Find all sockets
        GameObject parentObject = gameObject.transform.parent.gameObject;
        socket1 = parentObject.GetNamedChild("FuelHolder1");
        socket2 = parentObject.GetNamedChild("FuelHolder2");
        socket3 = parentObject.GetNamedChild("FuelHolder3");
        socket4 = parentObject.GetNamedChild("FuelHolder4");
        // Add socket logic
        socket1.GetComponent<XRSocketInteractor>().selectEntered.AddListener(updateLights1);
        socket2.GetComponent<XRSocketInteractor>().selectEntered.AddListener(updateLights2);
        socket3.GetComponent<XRSocketInteractor>().selectEntered.AddListener(updateLights3);
        socket4.GetComponent<XRSocketInteractor>().selectEntered.AddListener(updateLights4);
        socket1.GetComponent<XRSocketInteractor>().selectExited.AddListener(onDeselect1);
        socket2.GetComponent<XRSocketInteractor>().selectExited.AddListener(onDeselect2);
        socket3.GetComponent<XRSocketInteractor>().selectExited.AddListener(onDeselect3);
        socket4.GetComponent<XRSocketInteractor>().selectExited.AddListener(onDeselect4);

        // Get Grate and Grate light indicator reference
        grateDoor = (parentObject.transform.parent.gameObject).GetNamedChild("Portcullis_Metal").GetComponent<Transform>();
        indicatorMeshRenderer = (parentObject.transform.parent.gameObject).GetNamedChild("LightSignals").GetComponent<MeshRenderer>();

        rotationPoint = grateDoor.gameObject.GetNamedChild("pivotPoint").transform.position;
        finalRotationTarget = Quaternion.Euler(0, 190f, 0);
    }

    void Update()
    {
        if (animateGrate)
        {
            // Animate grate opening
            float angleDiff = Quaternion.Angle(grateDoor.transform.rotation, finalRotationTarget);
            // Snap into position
            if (angleDiff < 5f || prevDiff < angleDiff)
            {
                grateDoor.transform.rotation = finalRotationTarget;
                animateGrate = false;
                return;
            }
            grateDoor.transform.RotateAround(rotationPoint, Vector3.up, angularSpeed * Time.deltaTime);
            prevDiff = angleDiff;
        }
    }

    private void updateLights1(SelectEnterEventArgs args)
    {
        GameObject snappedObject = args.interactableObject.transform.gameObject;
        updateLights(snappedObject, 1);

    }

    private void updateLights2(SelectEnterEventArgs args)
    {
        GameObject snappedObject = args.interactableObject.transform.gameObject;
        updateLights(snappedObject, 2);

    }

    private void updateLights3(SelectEnterEventArgs args)
    {
        GameObject snappedObject = args.interactableObject.transform.gameObject;
        updateLights(snappedObject, 3);

    }

    private void updateLights4(SelectEnterEventArgs args)
    {
        GameObject snappedObject = args.interactableObject.transform.gameObject;
        updateLights(snappedObject, 4);

    }

    private void updateLights(GameObject target, int socketNumber)
    {
        string targetLabel = target.GetComponentInChildren<TextMeshProUGUI>().text;

        if (targetLabel != null && targetLabel == acceptedVials[(socketNumber - 1)])
        {
            // correct option inserted
            changeMaterial(socketNumber, colorOn);
            lights[socketNumber - 1] = true;
        }
        else
        {
            changeMaterial(socketNumber, colorOff);
            lights[socketNumber - 1] = false;
        }

        // Check if all lights are on:
        if (lights[0] && lights[1] && lights[2] && lights[3]) {
            // Open Grate
            animateGrate = true;
            // Change indicator colors
            Material[] materials = indicatorMeshRenderer.materials;
            materials[1] = colorOn;
            indicatorMeshRenderer.materials = materials;
        }
    }

    private void onDeselect1( SelectExitEventArgs args) {
        changeMaterial(1, colorOff);
        lights[0] = false;
    }
    
    private void onDeselect2(SelectExitEventArgs args)
    {
        changeMaterial(2, colorOff);
        lights[1] = false;
    }

    private void onDeselect3(SelectExitEventArgs args)
    {
        changeMaterial(3, colorOff);
        lights[2] = false;
    }

    private void onDeselect4(SelectExitEventArgs args)
    {
        changeMaterial(4, colorOff);
        lights[3] = false;
    }

    private void changeMaterial(int index, Material newMaterial) {
        Material[] materials = caseMeshRenderer.materials;
        materials[(2 + index)] = newMaterial;
        caseMeshRenderer.materials = materials;
    }
}

