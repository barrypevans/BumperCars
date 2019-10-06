using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField]
    private float RotationSpeed;
    [SerializeField]
    private float RotationWidth;


    void Start()
    {
        
    }


    void Update()
    {
        transform.eulerAngles = new Vector3(0, Mathf.Sin(RotationSpeed*Time.time)*RotationWidth, 0);
    }
}
