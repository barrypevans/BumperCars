using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{

    void Update()
    {
        transform.LookAt(Vector3.zero);
    }
}
