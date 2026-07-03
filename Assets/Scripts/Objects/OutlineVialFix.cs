using UnityEngine;

public class OutlineVialFix : MonoBehaviour
{
    private void Start()
    {
        Invoke(nameof(FixOutline), 1);
    }

    private void FixOutline()
    {
        gameObject.GetComponent<Outline>().enabled = false;
    }
}

