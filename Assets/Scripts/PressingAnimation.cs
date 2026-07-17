using UnityEngine;

public class PressingAnimation : MonoBehaviour
{
    public float pressDepth = 0.02f;
    public float pressSpeed = 15f;
    public float returnSpeed = 10f;
    public float pressTime = 0.15f;

    private Vector3 originalPosition;
    private Vector3 pressedPosition;

    private bool isPressed = false;

    void Start()
    {
        originalPosition = transform.localPosition;
        pressedPosition = originalPosition - new Vector3(0, pressDepth, 0);
    }

    void Update()
    {
        Vector3 target = isPressed ? pressedPosition : originalPosition;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            target,
            Time.deltaTime * (isPressed ? pressSpeed : returnSpeed)
        );
    }

    public void PressButton()
    {
        isPressed = true;
        Invoke(nameof(ReleaseButton), pressTime);
    }

    private void ReleaseButton()
    {
        isPressed = false;
    }
}