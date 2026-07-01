using UnityEngine;

public class MagicBook : MonoBehaviour
{  
    private bool writtenWithMagicInk = false; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other) 
{
    // Questo catturerà il trigger anche se l'oggetto è cinematico
    if (other.gameObject.name.Contains("MagicInkTip") || 
        (other.transform.parent != null && other.transform.parent.name.Contains("MagicInkTip")))
    {
        Debug.Log("YOU ESCAPED!");
    }
}
}
