using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAmbientAnimation : MonoBehaviour
{
    float bounceScale = .2f;
    float bounceSpeed = 4;
    Vector3 originalScale;
    float seed = 0;
    private void Start()
    {
        seed = Random.Range(-1000.0f, 1000.0f);
        originalScale = transform.localScale;
    }

    void Update()
    {
        float noise = Mathf.Abs(Mathf.PerlinNoise(Time.time * bounceSpeed, seed)) * bounceScale;
        transform.localScale = originalScale + new Vector3(.5f*noise, bounceScale - noise, .5f * noise);
    }
}
