using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    private float _trauma;
    [SerializeField] private GameObject _cameraBody;

    private int _seed1;
    private int _seed2;
    private int _seed3;

    private void Awake()
    {
        _seed1 = Random.Range(0, 5000);
        _seed2 = Random.Range(0, 5000);
        _seed3 = Random.Range(0, 5000);
    }

    public void AddShake(float amount)
    {
        _trauma += amount;
        _trauma = Mathf.Min(_trauma, 2);
    }

    private void Update()
    {

        Vector3 offset = new Vector3(Mathf.PerlinNoise(_seed1 + 25 * Time.time, 0),
            Mathf.PerlinNoise(0, _seed2 + 25 * Time.time),
            Mathf.PerlinNoise(0, _seed3 + 25 * Time.time)
            );

        _cameraBody.transform.localPosition = offset * _trauma * .5f;


        _trauma -= 5.0f * Time.deltaTime;
        _trauma = Mathf.Max(_trauma, 0);
    }
}
