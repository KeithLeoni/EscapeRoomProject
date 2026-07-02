using UnityEngine;


public class OutlineVialFix : MonoBehaviour
{
    private void Start()
    {
        Invoke(nameof(fixOutline), 1);
    }

    private void fixOutline()
    {
        gameObject.GetComponent<Outline>().enabled = false;
    }
}

