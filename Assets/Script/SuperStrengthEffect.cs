using UnityEngine;

public class SuperStrengthEffect : MonoBehaviour
{
    public ScenePowerManager powerManager;
    public Transform targetToScale;

    public Vector3 normalScale = Vector3.one;
    public Vector3 strengthScale = new Vector3(2f, 2f, 2f);

    public Vector3 normalLocalPosition;
    public Vector3 strengthLocalPosition = new Vector3(0f, 0.4f, 0f);

    public float scaleSpeed = 3f;

    void Start()
    {
        if (powerManager == null)
        {
            powerManager = FindFirstObjectByType<ScenePowerManager>();
        }

        if (targetToScale == null)
        {
            targetToScale = transform;
        }

        normalLocalPosition = targetToScale.localPosition;
    }

    void Update()
    {
        if (powerManager == null || targetToScale == null) return;

        bool active = powerManager.superStrength;

        Vector3 targetScale = active ? strengthScale : normalScale;
        Vector3 targetPosition = active
            ? normalLocalPosition + strengthLocalPosition
            : normalLocalPosition;

        targetToScale.localScale = Vector3.Lerp(
            targetToScale.localScale,
            targetScale,
            Time.deltaTime * scaleSpeed
        );

        targetToScale.localPosition = Vector3.Lerp(
            targetToScale.localPosition,
            targetPosition,
            Time.deltaTime * scaleSpeed
        );
    }
}
