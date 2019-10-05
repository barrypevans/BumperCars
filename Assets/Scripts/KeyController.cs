using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class KeyController : MonoBehaviour
{
    float rot = 0;
    float speed = 10;
    void Update()
    {
        rot += speed*Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, rot);
    }
}
