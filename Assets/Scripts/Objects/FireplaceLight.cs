using UnityEngine;
using System.Collections;

public class FireplaceLight : MonoBehaviour
{
    private float _flickerSpeed = 0.07f;
    private Light _light;
    private int _random; 

    void Start()
    {
        _light = GetComponent<Light>();
        StartCoroutine(Flickering());
    }

    // Update is called once per frame
    IEnumerator Flickering()
    {
        while (true)
        {
            _random = Random.Range(8, 12);
            _light.intensity = _random;
            yield return new WaitForSeconds(_flickerSpeed);
        }
        
    }
}
