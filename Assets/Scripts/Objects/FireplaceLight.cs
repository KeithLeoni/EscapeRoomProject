using UnityEngine;
using System.Collections;

public class FireplaceLight : MonoBehaviour
{
    // Set light flicker speed
    private float _flickerSpeed = 0.07f;

    // Fireplace light variable
    private Light _light;

    private int _random;

    void Start()
    {
        // Get Fireplace light
        _light = GetComponent<Light>();

        // Start flickering
        StartCoroutine(Flickering());
    }

    // Function to make the fireplace light flicker (use IEnumerator to enable momentarly suspending the execution)
    IEnumerator Flickering()
    {
        while (true)
        {   
            // Change intensity, creates the flickering effect 
            _random = Random.Range(8, 12);
            _light.intensity = _random;
            
            // Use yield statement to make the coroutine pause execution momentarly, also creates the flickering effect 
            yield return new WaitForSeconds(_flickerSpeed);  
        }
        
    }
}