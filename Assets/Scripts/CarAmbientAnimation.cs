using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAmbientAnimation : MonoBehaviour
{
    float bounceScale = .2f;
    float bounceSpeed = 4;
    Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        float noise = Mathf.Abs(Mathf.PerlinNoise(Time.time * bounceSpeed, 0)) * bounceScale;
        transform.localScale = originalScale + new Vector3(.5f*noise, bounceScale - noise, .5f * noise);
    }
}
